using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
	public bool scalable = false;
	ButtonScale buttonScale;
	private void OnEnable()
	{
		if (scalable && !GameState.shown_menu)
		{
			buttonScale = GetComponent<ButtonScale>();
			Debug.Assert(buttonScale != null);
			buttonScale.ScaleStart();
		}
	}
    public void OnMenuClicked()
    {
		if (scalable)
		{
			GameState.shown_menu = true;
			if (buttonScale != null)
			{
				buttonScale.ScaleStop();
			}
		}
    }
}
