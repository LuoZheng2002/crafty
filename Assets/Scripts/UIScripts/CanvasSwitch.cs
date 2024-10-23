using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasSwitch : MonoBehaviour
{
    public GameObject introCanvas;
    public GameObject buildCanvas;
    public GameObject playCanvas;
    public GameObject outroCanvas;
	static CanvasSwitch inst;
	public static CanvasSwitch Inst
	{
		get { Debug.Assert(inst != null, "Canvas Switch not set"); return inst; }
	}
	private void Start()
	{
		Debug.Assert(inst == null, "Canvas Switch already instantiated");
		inst = this;
        introCanvas.SetActive(false);
		buildCanvas.SetActive(false);
		playCanvas.SetActive(false);
		outroCanvas.SetActive(false);
	}

	public void TransitionToIntro()
	{
		introCanvas.SetActive(true);
		buildCanvas.SetActive(false);
		playCanvas.SetActive(false);
		outroCanvas.SetActive(false);
	}
	public void TransitionToBuild()
	{
		introCanvas.SetActive(false);
		buildCanvas.SetActive(true);
		playCanvas.SetActive(false);
		outroCanvas.SetActive(false);
		EventBus.Publish(new GenerateItemsEvent());
	}
	public void TransitionToPlay()
	{
		introCanvas.SetActive(false);
		playCanvas.SetActive(true);
		buildCanvas.SetActive(false);
		outroCanvas.SetActive(false);
	}
	public void TransitionToOutro()
	{
		introCanvas.SetActive(false);
		playCanvas.SetActive(false);
		buildCanvas.SetActive(false);
		outroCanvas.SetActive(true);
	}
}
