using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Layers : MonoBehaviour
{
	ButtonScale buttonScale;
	private void OnEnable()
	{
		buttonScale = GetComponent<ButtonScale>();
		if (!GameState.shown_layers)
		{
			buttonScale.ScaleStart();
		}
	}
	public void OnSwitchLayer()
    {
		// GridMatrix.Current.TrySwitchLayer();
        ToastManager.Toast("Hotkey: Space");
		GameState.shown_layers = true;
		buttonScale.ScaleStop();
    }
}
