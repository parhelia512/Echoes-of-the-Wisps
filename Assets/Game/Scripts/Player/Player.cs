using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.SceneManagement;

public class Player: MonoBehaviour
{
    public static Player instance;

    [SerializeField] private Animator anim;



    private float horizontalInput;
    [SerializeField] private float movementSpeed;
    [SerializeField] private float jumpForce;
    private bool isFacingRight = true;

    private Rigidbody2D rb;
    public Rigidbody2D Rb => rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask geistAllowingTerrainLayer;
    [SerializeField] private LayerMask geistBlockingTerrainLayer;
    [SerializeField] private LayerMask movingPlatformLayer;


    [SerializeField] private float baseCastDist; //how far the moving platform ray check goes


    [SerializeField] private GameObject geistPrefab;
    [SerializeField] private Transform geistSpawnTransform;
    bool geistIsActive = false;
    public bool GeistIsActive => geistIsActive;


    [SerializeField] private int maxMana;
    private int currentMana;
    public int CurrentMana => currentMana;
    [SerializeField] private Animator manaAnim;
    [SerializeField] private GameObject manaHud;


    private bool isJumping;
    private bool isSummoningFromIdle;
    private bool isDying;
    [HideInInspector] public bool isFreezed;

    private AnimatorClipInfo[] currentClipInfo;
    private string currentClipName;
    private int geistSummonCost = 1;

    private bool leavesCoroutineAllowed = true;
    [SerializeField] private GameObject leavesVfx;
    [SerializeField] private Transform leavesVfxSpawnPoint;

    [SerializeField] private Transform startPoint;

    [SerializeField] private AudioSource walkOnGrassSfx;
    [SerializeField] private AudioSource unsummonSfx;

    private bool godModeIsActive;
    [SerializeField] private bool canUseGodMode;

    [SerializeField] private GameObject godModeText;
    [SerializeField] private bool canUseManaCheat;




    private void OnEnable()
    {
        Actions.OnGeistUnsummoned += ReactToGeistUnsummoned;
        Actions.OnPlayerDeath += ReactToPlayerDeath;
        Actions.OnEnemySpawning += ReactToEnemySpawning;
        Actions.OnTeleportUsed += ReactToTeleportUsed;
        Actions.OnSmallNoiseUsed += ReactToSmallNoiseUsed;
    }


    private void OnDisable()
    {
        Actions.OnGeistUnsummoned -= ReactToGeistUnsummoned;
        Actions.OnPlayerDeath -= ReactToPlayerDeath;
        Actions.OnEnemySpawning -= ReactToEnemySpawning;
        Actions.OnTeleportUsed -= ReactToTeleportUsed;
        Actions.OnSmallNoiseUsed -= ReactToSmallNoiseUsed;
    }

    private void Awake()
    {
        instance = this;
    }


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentMana = CheckpointController.instance.PlayerManaWhenReloading;
        UpdateManaVisuals();

        if (!CheckpointController.instance.weLaunchedTheGameAlready)
        {
            transform.position = startPoint.position;
            CheckpointController.instance.weLaunchedTheGameAlready = true;
        }
        else
        {
            transform.position = CheckpointController.instance.LastCheckPointPos;
        }
    }




    void Update()
    {

        if (!geistIsActive && !isDying && !isFreezed && !PauseMenu.isPaused)
        {
            if (isFacingRight && horizontalInput < 0f || !isFacingRight && horizontalInput > 0f)
            {
                Flip();
            }
        }



        if(isSummoningFromIdle && IsFalling())
        {
            anim.SetTrigger("summonSpirit"); //to avoid getting stucked in the falling animation when summoning spirit during falling
        }


        if (IsGrounded())
        {
            if (geistIsActive) //to avoid player "slipping" on the ground when landing from a jump and having summoned geist while jumping
            {
                rb.velocity = new Vector2(0f, 0f);
            }

            if (IsSummoning())
            {
                anim.SetBool("isFalling", false);
                anim.SetBool("isMovingSpiritFromIdle", true);
            }

            if (IsMovingSpiritFromIdle())
            {

                anim.SetBool("isFalling", false);
                isSummoningFromIdle = false;
            }


            if (-0.1f < rb.velocity.x && rb.velocity.x < 0.1f)
            {
                if (!isJumping && !isSummoningFromIdle && !IsMovingSpiritFromIdle() && !IsTeleporting() && !isDying)
                {
                    anim.SetBool("isMovingSpiritFromIdle", false);
                    anim.SetBool("isRunning", false);
                    anim.SetBool("isFalling", false);
                    anim.SetBool("isIdling", true);
                }
            }
            else
            {
                if (!isJumping && !geistIsActive && !IsMovingSpiritFromIdle() && !isDying)
                {
                    anim.SetBool("isMovingSpiritFromIdle", false);
                    anim.SetBool("isIdling", false);
                    anim.SetBool("isFalling", false);
                    anim.SetBool("isRunning", true);
                    
                    if(!walkOnGrassSfx.isPlaying) 
                        walkOnGrassSfx.Play();

                    if (leavesCoroutineAllowed)
                    {
                        StartCoroutine(LeavesVfxCoroutine());
                        leavesCoroutineAllowed = false;
                    }
                }
            }
        }

        if (rb.velocity.y < 0.5f && !IsGrounded() && !isDying)
        {
            anim.SetBool("isFalling", true);
        }


        if (IsPlayerOnMovingPlatform())
        {
            var allMovingPlatforms = FindObjectsOfType<MovingPlatform>();
            foreach (var platform in allMovingPlatforms)
            {
                if(Vector2.Distance(groundCheck.position, platform.transform.position) < 2f)
                {
                    transform.SetParent(platform.transform);
                }
            }

            var allMovingPlatformsWithStop = FindObjectsOfType<MovingPlatformWithStop>();
            foreach (var platform in allMovingPlatformsWithStop)
            {
                if (Vector2.Distance(groundCheck.position, platform.transform.position) < 2f)
                {
                    transform.SetParent(platform.transform);
                }
            }

            var allMovingPlatformsWithStopAndBlockAtLastCp = FindObjectsOfType<MovingPlatformWithStopAndBlockAtLastCp>();
            foreach (var platform in allMovingPlatformsWithStopAndBlockAtLastCp)
            {
                if (Vector2.Distance(groundCheck.position, platform.transform.position) < 2f)
                {
                    transform.SetParent(platform.transform);
                }
            }
        }
        else
        {
            transform.SetParent(null);
        }

    }



    private bool IsPlayerOnMovingPlatform()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, movingPlatformLayer);
    }





    private void FixedUpdate()
    {
        if (isFreezed || isDying)
        {
            rb.velocity = new Vector2(0f, 0f);
        }


        if (!geistIsActive && !isFreezed && !isDying)
        {
            rb.velocity = new Vector2(horizontalInput * movementSpeed, rb.velocity.y);
        }

    }


    IEnumerator LeavesVfxCoroutine()
    {
        Instantiate(leavesVfx, leavesVfxSpawnPoint.transform.position, Quaternion.identity);
        yield return new WaitForSeconds(0.2f);
        leavesCoroutineAllowed = true;
    }


    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed && IsGrounded() && !geistIsActive && !isDying && !PauseMenu.isPaused)
        {
            StartCoroutine(JumpCoroutine());
            anim.SetBool("isRunning", false);
            anim.SetBool("isIdling", false);
            anim.SetBool("isFalling", false);
            anim.SetTrigger("isJumping");
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }

    IEnumerator JumpCoroutine()
    {
        isJumping = true;
        yield return new WaitForSeconds(0.3f); //to avoid that the running anim triggers the frame after I started jumping, because of the IsGrounded method, since OverlapCircle still detects ground
        isJumping = false;
    }


    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.1f, geistAllowingTerrainLayer | geistBlockingTerrainLayer | movingPlatformLayer);
    }


    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;
    }



    public void Move(InputAction.CallbackContext context)
    {
        horizontalInput = context.ReadValue<Vector2>().x;
    }


    public void SummonGeist(InputAction.CallbackContext context)
    {
        if (context.performed && !geistIsActive && currentMana > 0 && !PauseMenu.isPaused)
        {
            if (IsRunning())
            {
                Player.instance.UseMana(geistSummonCost);
                if (!isJumping) //we want the body to keep flying in jump direction, when summoning geist one microsec after starting jump
                {
                    rb.velocity = new Vector2(0f, rb.velocity.y);
                }
            }
            else 
            {
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y);
                Player.instance.UseMana(geistSummonCost);
            }
            isSummoningFromIdle = true;
            anim.SetBool("isIdling", false);
            anim.SetBool("isRunning", false);
            anim.SetBool("isFalling", false);
            anim.SetTrigger("summonSpirit");
            Instantiate(geistPrefab, geistSpawnTransform.position, Quaternion.identity); // Instantiate at position (0, 0, 0) and zero rotation.
            geistIsActive = true;
        }
    }


    private void ReactToGeistUnsummoned()
    {
        geistIsActive = false;
        unsummonSfx.Play();
        isSummoningFromIdle = false;
        anim.SetBool("isMovingSpiritFromIdle", false);
        anim.SetBool("isIdling", true);
    }

    private void ReactToTeleportUsed()
    {
        geistIsActive = false;
        isSummoningFromIdle = false;
        anim.SetBool("isMovingSpiritFromIdle", false);
        anim.SetBool("isIdling", true);
    }

    private void ReactToSmallNoiseUsed()
    {
        geistIsActive = false;
        isSummoningFromIdle = false;
        anim.SetBool("isMovingSpiritFromIdle", false);
        anim.SetBool("isIdling", true);
    }



    private void ReactToEnemySpawning()
    {
    }



    private void ReactToPlayerDeath()
    {
        if (!godModeIsActive)
        {
            isDying = true;
            anim.SetBool("isRunning", false);
            anim.SetBool("isIdling", false);
            anim.SetBool("isFalling", false);


            anim.SetTrigger("dissolve");
            StartCoroutine(DeathCoroutine());
        }
    }

    IEnumerator DeathCoroutine()
    {
        yield return new WaitForSeconds(0.8f);
        manaHud.SetActive(false);
        if (geistIsActive)
        {
            Destroy(Geist.instance.gameObject);
        }
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("FinalesLvl");
    }

    public void UseMana(int mana)
    {
        currentMana -= mana;
        UpdateManaVisuals();
    }
    public void RefundMana(int mana)
    {
        currentMana += mana;
        UpdateManaVisuals();
    }

    public void SetMana(int mana)
    {
        currentMana = mana;
        UpdateManaVisuals();
    }

    private void UpdateManaVisuals()
    {
        switch (currentMana)
        {
            case 5:
                manaAnim.SetInteger("Mana", 5);
                break;
            case 4:
                manaAnim.SetInteger("Mana", 4);
                break;
            case 3:
                manaAnim.SetInteger("Mana", 3);
                break;
            case 2:
                manaAnim.SetInteger("Mana", 2);
                break;
            case 1:
                manaAnim.SetInteger("Mana", 1);
                break;
            case 0:
                manaAnim.SetInteger("Mana", 0);
                break;
        }
    }


    public void FullManaCheat(InputAction.CallbackContext context)
    {
        if (canUseManaCheat)
        {
            if (context.performed && !PauseMenu.isPaused)
            {
                currentMana = maxMana;
                UpdateManaVisuals();
            }
        }
    }


    private bool IsIdling()
    {
        currentClipInfo = anim.GetCurrentAnimatorClipInfo(0);
        currentClipName = currentClipInfo[0].clip.name;
        var isIdling = currentClipName == "LynaIdle";



        if (isIdling)
        {
            return true;
        }
        else
        {
            return false;
        }
    }



    private bool IsRunning()
    {
        currentClipInfo = anim.GetCurrentAnimatorClipInfo(0);
        currentClipName = currentClipInfo[0].clip.name;
        var isRunning = currentClipName == "newrun";



        if (isRunning)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool IsTeleporting()
    {
        currentClipInfo = anim.GetCurrentAnimatorClipInfo(0);
        currentClipName = currentClipInfo[0].clip.name;
        var isTeleporting = currentClipName == "Dissolve";



        if (isTeleporting)
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    private bool IsFalling()
    {
        currentClipInfo = anim.GetCurrentAnimatorClipInfo(0);
        currentClipName = currentClipInfo[0].clip.name;
        var isFalling = currentClipName == "Falling";



        if (isFalling)
        {
            return true;
        }
        else
        {
            return false;
        }
    }



    private bool IsSummoning()
    {
        currentClipInfo = anim.GetCurrentAnimatorClipInfo(0);
        currentClipName = currentClipInfo[0].clip.name;
        var isSummoning = currentClipName == "castSpiritF";


        
        if (isSummoning)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool IsMovingSpiritFromIdle()
    {
        currentClipInfo = anim.GetCurrentAnimatorClipInfo(0);
        currentClipName = currentClipInfo[0].clip.name;
        var isMovingSpiritFromIdle = currentClipName == "duringCasting";



        if (isMovingSpiritFromIdle)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void ToggleGodMode(InputAction.CallbackContext context)
    {
        if (context.performed && !PauseMenu.isPaused && canUseGodMode)
        {
            if (godModeIsActive)
            {
                godModeIsActive = false;
                godModeText.SetActive(false);
            }
            else
            {
                godModeIsActive = true;
                godModeText.SetActive(true);
            }
        }
    }

}
