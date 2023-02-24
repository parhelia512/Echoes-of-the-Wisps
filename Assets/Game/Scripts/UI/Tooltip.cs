using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class Tooltip : MonoBehaviour

{
    public string message;

    public void OnMouseEnter()

    {
        TooltipManager._instance.SetAndShowToolTip(message);
    }

    public void OnMouseExit()

    {
        TooltipManager._instance.HideToolTip();
    }

    public void OnPointerClick(PointerEventData eventData)

    {
        TooltipManager._instance.SetAndShowToolTip(message);
    }
}