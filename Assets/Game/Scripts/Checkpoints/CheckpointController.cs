using UnityEngine;
using UnityEngine.SceneManagement;


public class CheckpointController : MonoBehaviour
{
    public static CheckpointController instance;


    [SerializeField] private Checkpoint startingCheckpoint;

    private Vector2 lastCheckPointPos;
    public Vector2 LastCheckPointPos => lastCheckPointPos;

    [SerializeField] private int playerManaWhenReloading;
    public int PlayerManaWhenReloading => playerManaWhenReloading;


    //if we are starting the game the first time > player should spawn at startPoint.
    //else player should start at lastCheckPointPos

    [HideInInspector] public bool weLaunchedTheGameAlready;

    [HideInInspector] public bool hasUnlockedTp;



    private void OnEnable()
    {
        Actions.OnCheckpointReached += UpdateCheckpoint;
    }

    private void OnDisable()
    {
        Actions.OnCheckpointReached -= UpdateCheckpoint;
    }



    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance);
        }
        else
        {
            Destroy(gameObject);
        }

    }




    private void Start()
    {
        lastCheckPointPos = startingCheckpoint.PlayerSpawnPoint.position;
    }

    private void Update()
    {
        Scene scene = SceneManager.GetActiveScene();
        if (scene.name == "MenuScene")
        {
            // Destroy the gameobject this script is attached to
            Destroy(gameObject);
        }
    }

    private void UpdateCheckpoint(Vector2 position, int manaValue)
    {
        lastCheckPointPos = position;
        playerManaWhenReloading = manaValue;
    }

}
