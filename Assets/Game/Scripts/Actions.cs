using UnityEngine;
using System;

public static class Actions
{
    public static Action <float, Vector2> OnNoiseMade; //range, position of big noise

    public static Action OnSmallNoiseUsed;

    public static Action <Vector2, int> OnCheckpointReached; //position of playerSpawnPos

    public static Action OnGeistUnsummoned;

    public static Action OnTeleportUsed;

    public static Action OnPlayerDeath;

    public static Action OnGeistDeath;

    public static Action OnEnemySpawning;

    public static Action <int> OnPickupCollected;



}
