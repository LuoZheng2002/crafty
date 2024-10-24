using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpenTutorialEvent
{

}
public class Help : MonoBehaviour
{
	ButtonScale buttonScale;
	private void OnEnable()
	{
		buttonScale = GetComponent<ButtonScale>();
		if (!GameState.shown_help)
		{
			buttonScale.ScaleStart();
		}
	}
	public void OpenTutorial()
    {
        EventBus.Publish(new OpenNewTutorialEvent());
		GameState.shown_help = true;
		buttonScale.ScaleStop();
    }
}
