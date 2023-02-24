using UnityEngine;
using UnityEngine.UI;

public class PickupManager : MonoBehaviour
{

    [SerializeField] private Image[] images;


    private void OnEnable()
    {
        Actions.OnPickupCollected += DisplayPickupImage;

    }


    private void OnDisable()
    {
        Actions.OnPickupCollected -= DisplayPickupImage;
    }


    private void DisplayPickupImage(int id)
    {
        var imageColor = images[id].color;
        imageColor.a = 1f;

        images[id].color = imageColor;
    }
}
