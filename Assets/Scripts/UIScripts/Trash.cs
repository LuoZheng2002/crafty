using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Trash : MonoBehaviour
{
	ButtonScale buttonScale;
	public static Trash Inst
	{
		get { Debug.Assert(inst != null);return inst; }
	}
	static Trash inst;
	private void Start()
	{
		Debug.Assert(inst == null);
		inst = this;
	}
	private void OnEnable()
	{
		buttonScale = GetComponent<ButtonScale>();
		Debug.Assert(buttonScale != null);
		Util.Delay(this, () =>
		{
			if (!GameState.shown_trashcan && GameState.Inst.Components.Count > 0)
			{
				buttonScale.ScaleStart();
			}
		});		
	}
	private void OnDestroy()
	{
		inst = null;
	}
	public void StartScale()
	{
		buttonScale.ScaleStart();
	}
	public void EndScale()
	{
		buttonScale.ScaleStop();
	}
	public void OnImageClick()
    {
		Debug.Log("Trashcan clicked");
		GridMatrix.Inst.Dump();
		
		GameState.shown_trashcan = true;
		buttonScale.ScaleStop();
    }
}
