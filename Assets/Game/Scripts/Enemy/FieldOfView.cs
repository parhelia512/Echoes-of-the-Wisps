using UnityEngine;
using UnityEngine.Events;

public class FieldOfView : MonoBehaviour
{
    #region events

    public UnityEvent OnPlayerSpotted;
    public UnityEvent OnGeistSpotted;

    #endregion


    [SerializeField] private LayerMask terrainAllowGeistLayerMask; //we want the fov to get blocked by both type of ground
    [SerializeField] private LayerMask terrainBlockGeistLayerMask;
    [SerializeField] private LayerMask movingPlatformLayerMask;
    [SerializeField] private LayerMask playerLayerMask;
    [SerializeField] private LayerMask geistLayerMask;

    [SerializeField] private LayerMask IgnoreMe; //we want to ignore the Enemy layer. otherwise when an enemy shoots a raycast it detects himself


    private Mesh mesh;

    private Vector3 origin; //origin of the fov
    private float startingAngle;
    [SerializeField] private float fov;
    [SerializeField] private float viewDistance;

    [SerializeField] private ParticleSystem shootVfx;
    [SerializeField] private AudioSource spottedSfx;


    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        origin = Vector3.zero;
    }

    private void LateUpdate()
    {
        UpdateVisionCone();
    }

    private void UpdateVisionCone()
    {
        int rayCount = 150; //nr of rays. the more we have, the more defined our fov will be. We increase the rayCount to smooth out the rotation, so it looks rounder
        float currentAngle = fov/2; //currentAngle, that increases in our cycle
        float angleIncrease = fov / rayCount; //how much we are going to increase the angle in each cycle in the loop



        Vector3[] vertices = new Vector3[rayCount + 1 + 1]; //rayCount does not include the "zero" ray. So we have as many vertices as rayCount, +1 for the origin + 1 for the zero ray
        Vector2[] uv = new Vector2[vertices.Length]; //everytime we define a vertex define here a uv coordinate
        int[] triangles = new int[rayCount * 3]; //we have as many triangles as we have rays. Each triangle contains the three vertices (the clockwise ordering to see the front face)

        vertices[0] = Vector3.zero;

        int vertexIndex = 1; //we start at index 1, since vertices[0] is the origin
        int triangleIndex = 0;

        var originWs = transform.TransformPoint(Vector3.zero); //transform origin coordinates from local to global


        for (int i = 0; i <= rayCount; i++) //every mini triangle shoots 3 rays
        {
            Vector3 vertex;

            var directionWs = transform.TransformDirection(GetVectorFromAngle(currentAngle));


            //check both type of terrainLayermaks OR playerLayerMask
            RaycastHit2D raycastHit2DTerrain = Physics2D.Raycast(originWs, directionWs, viewDistance, terrainAllowGeistLayerMask | terrainBlockGeistLayerMask | movingPlatformLayerMask);
            RaycastHit2D raycastHit2DPlayer = Physics2D.Raycast(originWs, directionWs, viewDistance, playerLayerMask);
            RaycastHit2D raycastHit2DGeist = Physics2D.Raycast(originWs, directionWs, viewDistance, geistLayerMask);



            if (raycastHit2DTerrain.collider == null)
            {
                vertex = originWs + directionWs * viewDistance; //vertex gets placed at max distance  
            }
            else //terrain hit
            {
                vertex = raycastHit2DTerrain.point;
            }
            vertices[vertexIndex] = transform.InverseTransformPoint(vertex);

            if (raycastHit2DPlayer.collider != null) //if the player was hit
            {
                if (raycastHit2DTerrain.collider == null || raycastHit2DPlayer.distance < raycastHit2DTerrain.distance)
                {
                    if (!spottedSfx.isPlaying)
                    {
                        spottedSfx.Play();
                    }
                    OnPlayerSpotted?.Invoke();
                }
                else
                {
                }
            }

            if (raycastHit2DGeist.collider != null) //if the geist was hit
            {
                if (raycastHit2DTerrain.collider == null || raycastHit2DGeist.distance < raycastHit2DTerrain.distance)
                {
                    if (!spottedSfx.isPlaying)
                    {
                        spottedSfx.Play();
                    }
                    OnGeistSpotted?.Invoke();
                }
                else
                {
                }
            }



            //generate triangle
            if (i > 0)
            {
                triangles[triangleIndex + 0] = 0; //we start every triangle from the origin
                triangles[triangleIndex + 1] = vertexIndex - 1; //then we go to the previous vertex -> will give an error when we are in the first vertex, since it has no previous vertices -> if loop
                triangles[triangleIndex + 2] = vertexIndex; //then to the current vertex

                triangleIndex += 3;
            }

            vertexIndex++;
            currentAngle -= angleIncrease; //when you increase an angle in unity you are going counterclockwise. So we have to decrease to go clockwise
        }


        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
        mesh.bounds = new Bounds(origin, Vector3.one * 1000f); //so that the fov doesn't disappear when the unit goes very far away
    }

  


    //converts an angle into a Vector3
    public static Vector3 GetVectorFromAngle(float angle)
    {
        float angleRad = angle * (Mathf.PI / 180f);
        return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
    }


    public static float GetAngleFromVectorFloat(Vector3 dir)
    {
        dir = dir.normalized;
        float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (n < 0) n += 360;

        return n;
    }

}
