using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapImageClickedEvent { }

public class MapImage : MonoBehaviour
{
    static MapImage inst;
	ButtonScale button_scale;
	public static MapImage Inst
	{
		get { Debug.Assert(inst != null); return inst; }
	}
	private void Start()
	{
		Debug.Assert(inst == null);
		inst = this;
		button_scale = GetComponent<ButtonScale>();
	}
	private void OnDestroy()
	{
		inst = null;
	}
	public void OnClick()
    {
        GameState.Inst.GoToMap();
		EventBus.Publish(new MapImageClickedEvent());
	}
	public void Show()
	{
		gameObject.SetActive(true);
	}
	public void Hide()
	{
		gameObject.SetActive(false);
	}
	public void StartScale()
	{
		button_scale.ScaleStart();
	}
}
