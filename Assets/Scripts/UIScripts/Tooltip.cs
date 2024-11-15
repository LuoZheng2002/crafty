using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tooltip : MonoBehaviour
{
    public string message;

    private void OnMouseEnter()
    {
        print("OnMouseEnter");
        TooltipManager._instance.SetAndShowToolTip(message);
    }

    public void OnMouseExit()
    {
        TooltipManager._instance.HideToolTip();
    }
}
