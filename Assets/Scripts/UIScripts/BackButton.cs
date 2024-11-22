using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackButton : MonoBehaviour
{
    static BackButton inst;
	public static BackButton Inst
	{
		get
		{
			Debug.Assert(inst != null);
			return inst;
		}
	}
	private void Start()
	{
		Debug.Assert(inst == null);
		inst = this;
		Debug.LogError("Deprecated");
	}
	private void OnDestroy()
	{
		inst = null;
	}
	public void OnClick()
    {
        GameState.Inst.GoBack();
    }
	public void Show()
	{
		gameObject.SetActive(true);
	}
	public void Hide()
	{
		gameObject.SetActive(false);
	}
}
