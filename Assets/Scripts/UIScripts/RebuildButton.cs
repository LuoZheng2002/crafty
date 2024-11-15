using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RebuildButton : MonoBehaviour
{
    static RebuildButton inst;
	public Image image;
    public static RebuildButton Inst
    {
        get { Debug.Assert(inst != null); return inst; }
    }
	private void Start()
	{
		Debug.Assert(inst == null);
		inst = this;
		EventBus.Subscribe<ScanFailEvent>(OnScanFail);
		EventBus.Subscribe<ScanSuccessEvent>(OnScanSuccess);
	}
	private void OnDestroy()
	{
		inst = null;
	}
	void SetAlpha(float alpha)
	{
		Color c = image.color;
		c.a = alpha;
		image.color = c;
	}
	bool can_click = true;
	public void OnClick()
	{
		if (can_click)
		{
			GameState.Inst.TryScan();
			SetAlpha(0.3f);
			can_click = false;
		}
	}
	void OnScanFail(ScanFailEvent e)
	{
		can_click = true;
		SetAlpha(1);
	}
	void OnScanSuccess(ScanSuccessEvent e)
	{
		can_click = true;
		SetAlpha(1);
	}
}
