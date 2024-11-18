using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Retry : MonoBehaviour
{
	static Retry inst;
	public static Retry Inst
	{
		get { Debug.Assert(inst != null); return inst; }
	}
	private void Start()
	{
		Debug.Assert(inst == null);
		inst = this;
	}
	private void OnDestroy()
	{
		inst = null;
	}
	public void Show()
	{
		gameObject.SetActive(true);
	}
	public void Hide()
	{
		gameObject.SetActive(false);
	}
	public void OnRetry()
    {
		GameState.Inst.OnRetry();
		GameState.shown_retry = true;
		RebuildButton.Inst.CarBroken = false;
    }
}
