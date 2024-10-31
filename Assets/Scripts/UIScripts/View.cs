using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class View : MonoBehaviour
{
	ButtonScale buttonScale;
	private void OnEnable()
	{
		buttonScale = GetComponent<ButtonScale>();
		if (!GameState.shown_view)
		{
			buttonScale.ScaleStart();
		}
	}
	public void OnViewClicked()
	{
		CustomCursor.Inst.ResetCursor();
		GameState.shown_view = true;
		buttonScale.ScaleStop();
	}
}
