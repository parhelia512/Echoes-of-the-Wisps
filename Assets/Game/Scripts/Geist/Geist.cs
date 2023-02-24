using UnityEngine;
using System;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class Geist: MonoBehaviour
{
    public static Geist instance;


    private float horizontalInput;
    [SerializeField] private float movementSpeed = 7.0f;

    private float verticalInput;
    private Rigidbody2D rb;

    private bool isInsideMovementRange = true;
    [SerializeField] private float ghostRange = 7f;


    [SerializeField] private GameObject smallNoiseShaderVfx;
    [SerializeField] private float smallNoiseRange;
    public float SmallNoiseRange => smallNoiseRange;

    [SerializeField] private GameObject bigNoiseShaderVfx;
    [SerializeField] private float bigNoiseRange;
    public float BigNoiseRange => bigNoiseRange;

    [SerializeField] private LayerMask terrainAllowGeist;

    [SerializeField] private Animator anim;



    private bool hasOutsideOfRangePenality = false;
    [SerializeField] private float outsideOfRangeWaitPenality = 0.5f;
    private float outsideOfRangeWaitPenalityCounter;

    [SerializeField] private int noiseCost;
    [SerializeField] private int teleportCost;
    [SerializeField] private int ghostDestroyRefund;

    private bool manaRefunded;
    private int refundedTimes;



    [SerializeField] private GameObject light;
    private Color lightColor;
    [Range(0f, 1f)] [SerializeField] private float lightCorrection; 
    [Range(0f, 1f)] [SerializeField] private float fadeDistanceCorrection; //how fast the light fades out


    [SerializeField] private GameObject follower;

    [SerializeField] private AudioSource movementSfx;
    [SerializeField] private AudioSource summonSfx;
    [SerializeField] private AudioSource teleportForbiddenSfx;
    [SerializeField] private AudioSource hitMaxRangeSfx;







    private void OnEnable()
    {
        Actions.OnGeistDeath += ReactToGeistDeath;
    }


    private void OnDisable()
    {
        Actions.OnGeistDeath -= ReactToGeistDeath;
    }



    private void Awake()
    {
        instance = this;
        var newFollower = Instantiate(follower, transform.position, Quaternion.identity);
        newFollower.GetComponent<Follower>().target = transform;
    }


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        outsideOfRangeWaitPenalityCounter = outsideOfRangeWaitPenality;
        lightColor = light.GetComponent<Light2D>().color;
    }




    void Update()
    {
        lightColor.a = Mathf.Lerp(lightCorrection, 0f, Vector3.Distance(Player.instance.transform.position, transform.position) * fadeDistanceCorrection);
        light.GetComponent<Light2D>().color = lightColor;

        CheckIfGhostIsInsideMovementRange();
        if (!isInsideMovementRange)
        {
            hitMaxRangeSfx.Stop();
            hitMaxRangeSfx.Play();
        }
    }



    private void FixedUpdate()
    {
        if (isInsideMovementRange && !hasOutsideOfRangePenality)
        {
            rb.velocity = new Vector2(horizontalInput * movementSpeed, verticalInput * movementSpeed);
            if (!movementSfx.isPlaying)
                movementSfx.Play();
        }
        else
        {
            if(outsideOfRangeWaitPenalityCounter > 0f)
            {
                outsideOfRangeWaitPenalityCounter -= Time.deltaTime;
                rb.velocity = new Vector2(0, 0);
                transform.position = Vector2.MoveTowards(transform.position, Player.instance.transform.position, movementSpeed / 2 * Time.deltaTime);
            }
            else
            {
                hasOutsideOfRangePenality = false;
                outsideOfRangeWaitPenalityCounter = outsideOfRangeWaitPenality;
            }

        }
    }



    public void Move(InputAction.CallbackContext context)
    {
        horizontalInput = context.ReadValue<Vector2>().x;
        verticalInput = context.ReadValue<Vector2>().y;
    }




    public void DestroyGeist(InputAction.CallbackContext context)
    {
        if (!PauseMenu.isPaused)
        {
            refundedTimes = refundedTimes + 1;
            if (refundedTimes == 1)
            {
                Player.instance.RefundMana(ghostDestroyRefund);
            }

            Actions.OnGeistUnsummoned?.Invoke();
            Destroy(gameObject);
            if (refundedTimes == 2)
            {
                refundedTimes = 0;
            }
        }

    }




    public void TeleportPlayerToGeist(InputAction.CallbackContext context)
    {
        var pressedTheTpBtn = context.performed;
        var weHaveEnoughMana = Player.instance.CurrentMana >= teleportCost;
        var geistIsNotOnGeistAllowingTerrain = !Physics2D.OverlapCircle(transform.position, 0.1f, terrainAllowGeist);
        var geistIsOnGeistAllowingTerrain = Physics2D.OverlapCircle(transform.position, 0.1f, terrainAllowGeist);

        if (pressedTheTpBtn && weHaveEnoughMana && geistIsNotOnGeistAllowingTerrain && !PauseMenu.isPaused && CheckpointController.instance.hasUnlockedTp)
        {
            StartCoroutine(TeleportCoroutine());
        }
        else if (geistIsOnGeistAllowingTerrain && pressedTheTpBtn && !PauseMenu.isPaused)
        {
            teleportForbiddenSfx.Play();
            anim.SetTrigger("CantTeleport");
        }
    }

    IEnumerator TeleportCoroutine()
    {
        Player.instance.UseMana(teleportCost);
        Actions.OnTeleportUsed?.Invoke();
        yield return new WaitForSeconds(0f); //to give the dissolve animation the time to play
        Player.instance.gameObject.transform.position = transform.position;
        Player.instance.Rb.velocity = new Vector2(Player.instance.Rb.velocity.x, 0f);
        Instantiate(bigNoiseShaderVfx, transform.position, Quaternion.identity);
        Actions.OnNoiseMade?.Invoke(bigNoiseRange, transform.position);
        Actions.OnTeleportUsed?.Invoke();


        Destroy(gameObject);
    }


    public void MakeSmallNoise(InputAction.CallbackContext context)
    {
        if (context.performed && Player.instance.CurrentMana >= noiseCost && !PauseMenu.isPaused)
        {
            Player.instance.UseMana(noiseCost);
            Instantiate(smallNoiseShaderVfx, transform.position, Quaternion.identity);
            Actions.OnNoiseMade?.Invoke(smallNoiseRange, transform.position);
            Actions.OnSmallNoiseUsed?.Invoke();



            Destroy(gameObject);
        }
    }


    private void CheckIfGhostIsInsideMovementRange()
    {
        if(Vector3.Distance(Player.instance.transform.position, transform.position) < ghostRange)
        {

            isInsideMovementRange = true;
        }
        else
        {
            isInsideMovementRange = false;
            hasOutsideOfRangePenality = true;
        }
    }


    private void ReactToGeistDeath()
    {
        Actions.OnGeistUnsummoned?.Invoke();
        Destroy(gameObject);
    }



    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, bigNoiseRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, smallNoiseRange);
    }

}
