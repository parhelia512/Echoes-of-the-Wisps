using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEnd : MonoBehaviour
{
    [SerializeField] private GameObject victoryMenu;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        victoryMenu.SetActive(true);
        Player.instance.isFreezed = true;
    }
}
