using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Retry : MonoBehaviour
{
	ButtonScale buttonScale;
	private void OnEnable()
	{
		buttonScale = GetComponent<ButtonScale>();
		if (!GameState.shown_retry)
		{
			buttonScale.ScaleStart();
		}
	}
	public void OnRetry()
    {
		GameState.Inst.Retry();
		GameState.shown_retry = true;
		buttonScale.ScaleStop();
    }
}
