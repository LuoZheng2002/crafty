using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Retry : MonoBehaviour
{
	static Retry inst;
	ButtonScale button_scale;
	private bool can_retry = true;
	Image image;
	void SetAlpha(float alpha)
	{
		Color color = image.color;
		color.a = alpha;
		image.color = color;
	}
	public bool CanRetry
	{
		get { return can_retry; }
		set
		{
			can_retry = value;
			if (can_retry)
			{
				button_scale.ScaleStart();
				SetAlpha(1.0f);
			}
			else
			{
				button_scale.ScaleStop();
				SetAlpha(0.3f);
			}
		}
	}
	public static Retry Inst
	{
		get { Debug.Assert(inst != null); return inst; }
	}
	private void Start()
	{
		Debug.Assert(inst == null);
		inst = this;
		button_scale = GetComponent<ButtonScale>();
		image = GetComponent<Image>();
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
		if (!can_retry)
			return;
		GameState.Inst.OnRetry();
		GameState.shown_retry = true;
		RebuildButton.Inst.CarBroken = false;
    }
	public void StartScale()
	{
		button_scale.ScaleStart();
	}
}
