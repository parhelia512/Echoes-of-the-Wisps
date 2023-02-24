using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovingStatic : MonoBehaviour
{

    private Rigidbody2D rb;
    [SerializeField] private float moveSpeed;
    private float currentMoveSpeed;

    [SerializeField] private Transform castPos;
    [SerializeField] private LayerMask terrainBlockGeistLayerMask;
    [SerializeField] private LayerMask terrainAllowGeistLayerMask;


    [SerializeField] private float floorBaseCastDist; //how far the enemy can see. This nr should be positive. use RANGE
    [SerializeField] private float wallBaseCastDist; //how far the enemy can see. This nr should be positive. use RANGE

    private const string LEFT = "left";
    private const string RIGHT = "right";
    private string facingDirection;




    public Animator anim;
    public Animator smallAnim;

    private bool isMoving = true;


    private Transform player;
    [SerializeField] private Transform bulletSpawnPoint;
    [SerializeField] private GameObject bullet;
    [SerializeField] private GameObject bulletForBigNoise;
    [SerializeField] private GameObject bulletSearchGeist;

    [SerializeField] private ParticleSystem shootVfx;



    private Vector3 playerPositionOnTeleport;
    [SerializeField] private float delayBeforeShootWhenHearingBigNoise;
    private GameObject currentBullet;



    public bool BulletIsFlying => currentBullet != null;

    private AnimatorClipInfo[] currentClipInfo;
    private string currentClipName;

    [SerializeField] private List<Transform> positioningToolPoints;
    [SerializeField] private GameObject noiseDetector;



    [SerializeField] private AudioSource shootSfx;
    [SerializeField] private AudioSource bigShootSfx;




    private void OnEnable()
    {
        Actions.OnNoiseMade += GetDistractedByNoise;
    }


    private void OnDisable()
    {
        Actions.OnNoiseMade -= GetDistractedByNoise;
    }





    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        facingDirection = LEFT;
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        if (IsTurning())
        {
            if(facingDirection == LEFT)
            {
                SetCurrentMoveSpeed(moveSpeed);
            }
            else
            {
                SetCurrentMoveSpeed(-moveSpeed);
            }
        }


        else if (IsSpawning())
        {
            SetCurrentMoveSpeed(0f);
        }


        else if (IsDistracted())
        {
            SetCurrentMoveSpeed(0f);
        }


        else if (IsAtHole())
        {
            SetCurrentMoveSpeed(0f);
        }

        else if (IsLoadingBigShoot())
        {
            SetCurrentMoveSpeed(0f);
        }


        //only here so that it starts moving after having spawned
        else if (IsPatrolling() && !IsAtHole())
        {
            if (facingDirection == LEFT)
            {
                SetCurrentMoveSpeed(moveSpeed);
            }
            else
            {
                SetCurrentMoveSpeed(-moveSpeed);
            }
        }

    }


    private void FixedUpdate()
    {
        float velX = currentMoveSpeed;

        if (facingDirection == LEFT)
        {
            velX = currentMoveSpeed;
        }


        //move the enemy
        if (isMoving)
        {
            rb.velocity = new Vector2(velX, 0);
        }
        else
        {
            rb.velocity = new Vector2(0, 0);
        }



        if (IsNearEdge())
        {
            if (facingDirection == LEFT)
            {
                ChangeFacingDirectionSimple(RIGHT);
                StartCoroutine(DeactivateCastPosCoroutine());
            }
            else
            {
                ChangeFacingDirectionSimple(LEFT);
                StartCoroutine(DeactivateCastPosCoroutine());
            }
        }


        if (IsHittingWall())
        {
            if (facingDirection == LEFT)
            {
                ChangeFacingDirectionSimple(RIGHT);
                anim.SetTrigger("TurnRight");
            }
            else
            {
                ChangeFacingDirectionSimple(LEFT);
                anim.SetTrigger("TurnLeft");
            }

            SetCurrentMoveSpeed(-currentMoveSpeed);

        }
    }



    private void GetDistractedByNoise(float noiseRange, Vector2 noisePos)
    {
        for (int i = 0; i < noiseDetector.gameObject.transform.childCount; i++)
        {
            if(Vector2.Distance(noiseDetector.gameObject.transform.GetChild(i).gameObject.transform.position, Geist.instance.transform.position) < noiseRange)
            {
                ReactToNoise(noiseRange);
                return;
            }
        }
    }




    private void ReactToNoise(float noiseRange)
    {
        if (IsLoadingBigShoot()) return;

        if (IsDistracted()) return;

        if (IsSpawning()) return;



        if (!IsLoadingBigShoot() && !IsDistracted() && !IsSpawning()) 
        {
            Debug.Log("reacted to small noise");

            var spiritIsOnTheRxOfTheEnemy = Geist.instance.transform.position.x > transform.position.x;
            var spiritIsOnTheLxOfTheEnemy = Geist.instance.transform.position.x < transform.position.x;

            if (Geist.instance.SmallNoiseRange - 0.1f <= noiseRange && noiseRange <= Geist.instance.SmallNoiseRange + 0.1f)
            {
                Debug.Log("reacted to small noise");

                Debug.Log(facingDirection);

                var distractedFromRx = spiritIsOnTheRxOfTheEnemy
                                       && ((facingDirection == LEFT && !IsAtHole())
                                           || (facingDirection == RIGHT && IsAtHole()));


                var distractedFromLx = spiritIsOnTheLxOfTheEnemy
                                       && ((facingDirection == RIGHT && !IsAtHole())
                                           || (facingDirection == LEFT && IsAtHole()));
                                           

                if (distractedFromRx) //facingDirection == LEFT && spiritIsOnTheRxOfTheEnemy
                {
                    Debug.Log("DistractedFromRx");
                    ChangeFacingDirectionSimple(RIGHT);
                    anim.SetTrigger("DistractedFromRx");
                    anim.SetFloat("Direction", 1.0f);
                }


                else if (distractedFromLx)
                {
                    ChangeFacingDirectionSimple(LEFT);
                    anim.SetTrigger("DistractedFromLx");
                    anim.SetFloat("Direction", -1.0f);
                }
            }



            if (Geist.instance.BigNoiseRange - 0.1f <= noiseRange && noiseRange <= Geist.instance.BigNoiseRange + 0.1f)
            {
                if (facingDirection == LEFT && spiritIsOnTheRxOfTheEnemy)
                {
                    ChangeFacingDirectionSimple(RIGHT);
                    anim.SetFloat("Direction", 1.0f);
                    StartCoroutine(ShootOnBigNoiseWithTurnCoroutine());
                }


                else if (facingDirection == RIGHT && spiritIsOnTheLxOfTheEnemy)
                {
                    ChangeFacingDirectionSimple(LEFT);
                    anim.SetFloat("Direction", -1.0f);
                    StartCoroutine(ShootOnBigNoiseWithTurnCoroutine());
                }



                else if (facingDirection == LEFT && spiritIsOnTheLxOfTheEnemy)
                {
                    SetCurrentMoveSpeed(0f);
                    anim.SetFloat("Direction", 1.0f);
                    StartCoroutine(ShootOnBigNoiseWithNoTurnCoroutine());
                }


                else if (facingDirection == RIGHT && spiritIsOnTheRxOfTheEnemy)
                {
                    SetCurrentMoveSpeed(0f);
                    anim.SetFloat("Direction", -1.0f);
                    StartCoroutine(ShootOnBigNoiseWithNoTurnCoroutine());
                }
            }
        }
    }




    void ChangeFacingDirectionSimple(string newDirection)
    {
        facingDirection = newDirection;
    }




    private bool IsHittingWall()
    {
        bool retVal = false;

        float castDist = wallBaseCastDist;
        //define the cast distance
        if(facingDirection == LEFT)
        {
            castDist = wallBaseCastDist; //so that the ray shoots left
        }
        else
        {
            castDist = -wallBaseCastDist;
        }

        //determine the target destination based on the cast distance
        Vector3 targetPos = castPos.position; //to set the "targetPos" (endpoint of our ray) we start at castPos
        targetPos.x += castDist; //and then we move along the x axis for a distance equal to castDist (the default ray length)




        if (castPos.gameObject.activeInHierarchy) //dont shoot the blue cast, if the castPos object is inactive
        {
            Debug.DrawLine(castPos.position, targetPos, Color.blue);

            //draw the ray and look for the walls
            if (Physics2D.Linecast(castPos.position, targetPos, terrainAllowGeistLayerMask) || Physics2D.Linecast(castPos.position, targetPos, terrainBlockGeistLayerMask)) //shoots a ray from castPos to targetPos and looks if we intersect the "Terrain" layer
            {
                retVal = true;
            }
        }
        else
        {
            retVal = false;
        }

        return retVal;
    }




    private bool IsNearEdge()
    {
        float castDist = floorBaseCastDist;

        //determine the target destination based on the cast distance
        Vector3 targetPos = castPos.position; //to set the "targetPos" (endpoint of our ray) we start at castPos
        targetPos.y += castDist; //and then we move downwards along the y axis for a distance equal to castDist (the default ray length)


        if (castPos.gameObject.activeInHierarchy)
        {
            Debug.DrawLine(castPos.position, targetPos, Color.red);

            //draw the ray and look for a hole
            var hitInfo = Physics2D.Linecast(castPos.position, targetPos, terrainAllowGeistLayerMask | terrainBlockGeistLayerMask);


            if(hitInfo.collider == null) //if we are at an edge
            {

                currentClipInfo = anim.GetCurrentAnimatorClipInfo(0);
                currentClipName = currentClipInfo[0].clip.name;
                var isLookingAtHoleLx = currentClipName == "MoorCrawlerEnemyAWatchOutHoleStatic";
                var isLookingAtHoleRx = currentClipName == "MoorCrawlerEnemyAWatchOutHoleRxStatic";
                var isLookingAtHole = isLookingAtHoleLx || isLookingAtHoleRx;


                if (facingDirection == LEFT && !isLookingAtHole && !IsTurning() && !IsDistracted())
                {
                    anim.SetTrigger("HoleLx");
                }
                if (facingDirection == RIGHT && !isLookingAtHole && !IsTurning() && !IsDistracted())  
                {
                    anim.SetTrigger("HoleRx");
                }
                SetCurrentMoveSpeed(0f);
            }


            return hitInfo.collider == null;
        }

        else
        {
            return false;
        }
    }


    private bool IsSpawning()
    {
        currentClipInfo = anim.GetCurrentAnimatorClipInfo(0);
        currentClipName = currentClipInfo[0].clip.name;
        var isSpawning = currentClipName == "MoorCrawlerEnemyASpawnStatic";

        if (isSpawning)
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    private bool IsPatrolling()
    {
        currentClipInfo = anim.GetCurrentAnimatorClipInfo(0);
        currentClipName = currentClipInfo[0].clip.name;
        var isPatrollingToLx = currentClipName == "MoorCrawlerEnemyARunLoopStatic";
        var isPatrollingToRx = currentClipName == "MoorCrawlerEnemyARunLoopReverseStatic";
        var isPatrolling = isPatrollingToLx || isPatrollingToRx;


        if (isPatrolling)
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    private bool IsTurning()
    {
        currentClipInfo = anim.GetCurrentAnimatorClipInfo(0);
        currentClipName = currentClipInfo[0].clip.name;
        var isTurning = currentClipName == "MoorCrawlerEnemyATurnStatic";

        if (isTurning)
        {
            return true;
        }
        else 
        { 
            return false; 
        }
    }


    private bool IsDistracted()
    {
        currentClipInfo = anim.GetCurrentAnimatorClipInfo(0);
        currentClipName = currentClipInfo[0].clip.name;
        var isDistractedFromLx = currentClipName == "DistractedFromLxStatic";
        var isDistractedFromRx = currentClipName == "DistractedFromRxStatic";
        var isDistracted = isDistractedFromLx || isDistractedFromRx;



        if (isDistracted)
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    private bool IsAtHole()
    {
        currentClipInfo = anim.GetCurrentAnimatorClipInfo(0);
        currentClipName = currentClipInfo[0].clip.name;
        var isAtHoleLx = currentClipName == "MoorCrawlerEnemyAWatchOutHoleStatic";
        var isAtHoleRx = currentClipName == "MoorCrawlerEnemyAWatchOutHoleRxStatic";
        var isAtHole = isAtHoleLx || isAtHoleRx;



        if (isAtHole)
        {
            return true;
        }
        else
        {
            return false;
        }
    }



    private bool IsLoadingBigShoot()
    {
        currentClipInfo = anim.GetCurrentAnimatorClipInfo(0);
        currentClipName = currentClipInfo[0].clip.name;

        var isLoadingBigShootRx = currentClipName == "LoadShootRxStatic";
        var isLoadingBigShootLx = currentClipName == "LoadShootLxStatic";
        var isLoadingBigShoot = isLoadingBigShootLx || isLoadingBigShootRx;



        if (isLoadingBigShoot)
        {
            return true;
        }
        else
        {
            return false;
        }
    }


   




    IEnumerator ShootOnBigNoiseWithTurnCoroutine()
    {
        playerPositionOnTeleport.x = GameObject.FindGameObjectWithTag("Player").transform.position.x;
        playerPositionOnTeleport.y = GameObject.FindGameObjectWithTag("Player").transform.position.y;
        playerPositionOnTeleport.z = 0f;


        var spiritIsOnTheRxOfTheEnemy = Geist.instance.transform.position.x > transform.position.x;
        var spiritIsOnTheLxOfTheEnemy = Geist.instance.transform.position.x < transform.position.x;


        //THIS WORKS WHEN BIG NOISE TURNED HIM
        //we write RIGHT, but its easy to think "LEFT" should be written here. But notice: in the GetDistractedByNoise method we already used the ChangeFacingDirectionSimple method!!!! So its the other way around!!!!!
        if (facingDirection == RIGHT && spiritIsOnTheRxOfTheEnemy)
        {
            anim.SetTrigger("ShootRx");
        }

        else if (facingDirection == LEFT && spiritIsOnTheLxOfTheEnemy)
        {
            anim.SetTrigger("ShootLx");
        }



        yield return new WaitForSeconds(3.5f);

        TryShootOnBigNoise();

        if (facingDirection == LEFT)
        {
            SetCurrentMoveSpeed(moveSpeed);
            anim.SetTrigger("ExitShootGoLx");

        }
        else if (facingDirection == RIGHT)
        {
            SetCurrentMoveSpeed(-moveSpeed);
            anim.SetTrigger("ExitShootGoRx");
        }
    }




    IEnumerator ShootOnBigNoiseWithNoTurnCoroutine()
    {
        playerPositionOnTeleport.x = GameObject.FindGameObjectWithTag("Player").transform.position.x;
        playerPositionOnTeleport.y = GameObject.FindGameObjectWithTag("Player").transform.position.y;
        playerPositionOnTeleport.z = 0f;


        var spiritIsOnTheRxOfTheEnemy = Geist.instance.transform.position.x > transform.position.x;
        var spiritIsOnTheLxOfTheEnemy = Geist.instance.transform.position.x < transform.position.x;



        if (facingDirection == RIGHT && spiritIsOnTheRxOfTheEnemy)
        {
            anim.SetTrigger("ShootRx");
        }

        else if (facingDirection == LEFT && spiritIsOnTheLxOfTheEnemy)
        {
            anim.SetTrigger("ShootLx");
        }



        //yield return new WaitForSeconds(delayBeforeShootWhenHearingBigNoise); //4 4 is the length of the load big shoot animation, but its wrong here to set 4
        yield return new WaitForSeconds(3.5f); //this have to be LOWER than the length of the load big shoot animation. This controls when he exits the load animation and goes back to patrol. This works when delayBeforeShootWhenHearingBigNoise is 4 by inspector

        TryShootOnBigNoise();

        if (facingDirection == LEFT)
        {
            SetCurrentMoveSpeed(moveSpeed);
            anim.SetTrigger("ExitShootGoLx");
        }
        else if (facingDirection == RIGHT)
        {
            SetCurrentMoveSpeed(-moveSpeed);
            anim.SetTrigger("ExitShootGoRx");
        }
    }






    IEnumerator DeactivateCastPosCoroutine()
    {
        castPos.gameObject.SetActive(false);

        yield return new WaitForSeconds(4f);

        castPos.gameObject.SetActive(true);
    }






    private void SetCurrentMoveSpeed(float speed)
    {
        currentMoveSpeed = speed;
    }



    public void TryShoot()
    {
        if (BulletIsFlying)
        {
            return;
        }
        currentBullet = Instantiate(bullet, bulletSpawnPoint.position, Quaternion.identity);
        shootVfx.Play();
        shootSfx.Play();
        }

    public void TryShootAtGeist()
    {
        if (BulletIsFlying)
        {
            return;
        }
        currentBullet = Instantiate(bulletSearchGeist, bulletSpawnPoint.position, Quaternion.identity);
        shootVfx.Play();
    }

    public void TryShootOnBigNoise()
    {
        if (BulletIsFlying)
        {
            return;
        }

        bigShootSfx.Play();
        currentBullet = Instantiate(bulletForBigNoise, bulletSpawnPoint.position, Quaternion.identity);
        currentBullet.GetComponent<BulletForBigNoise>().SetTarget(playerPositionOnTeleport);
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        Gizmos.DrawLine(positioningToolPoints[0].position, positioningToolPoints[1].position);
    }


}
