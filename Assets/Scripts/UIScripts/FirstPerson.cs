using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FirstPerson : MonoBehaviour
{
	ButtonScale buttonScale;
	private void OnEnable()
	{
		buttonScale = GetComponent<ButtonScale>();
		if (!GameState.shown_third_person)
		{
			buttonScale.ScaleStart();
		}
	}
	public void ToggleFirstPerson()
    {
        GameState.Inst.FirstPerson = !GameState.Inst.FirstPerson;
		CanvasDrag.Inst.OnFirstPersonChanged();
        GameState.shown_third_person = true;
		buttonScale.ScaleStop();
    }
}
