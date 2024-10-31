using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasSwitch : MonoBehaviour
{
    public GameObject buildCanvas;
    public GameObject playCanvas;
	static CanvasSwitch inst;
	public static CanvasSwitch Inst
	{
		get { Debug.Assert(inst != null, "Canvas Switch not set"); return inst; }
	}
	private void Start()
	{
		Debug.Assert(inst == null, "Canvas Switch already instantiated");
		inst = this;
		// buildCanvas.SetActive(false);
		playCanvas.SetActive(false);
	}
	private void OnDestroy()
	{
		inst = null;
	}
	public void TransitionToIntro()
	{
		buildCanvas.SetActive(false);
		playCanvas.SetActive(false);
	}
	public void TransitionToBuild()
	{
		buildCanvas.SetActive(true);
		playCanvas.SetActive(false);
	}
	public void TransitionToPlay()
	{
		playCanvas.SetActive(true);
		buildCanvas.SetActive(false);
	}
	public void TransitionToOutro()
	{
		playCanvas.SetActive(false);
		buildCanvas.SetActive(false);
	}
}
