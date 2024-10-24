using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasSwitch : MonoBehaviour
{
    public GameObject introCanvas;
    public GameObject buildCanvas;
    public GameObject playCanvas;
    public GameObject outroCanvas;
	
	public GameObject dragScreenCanvas;
	public GameObject menuConfirmCanvas;
	public GameObject viewCanvas;

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
		dragScreenCanvas.SetActive(false);
		menuConfirmCanvas.SetActive(false);
		viewCanvas.SetActive(false);
	}
	private void OnDestroy()
	{
		inst = null;
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
	IEnumerator ShowDragScreenHelper()
	{
		yield return new WaitForSeconds(1.0f);
		dragScreenCanvas.SetActive(true);
		yield return new WaitForSeconds(3.0f);
		dragScreenCanvas.SetActive(false);
	}
	public void ShowDragScreen()
	{
		StartCoroutine(ShowDragScreenHelper());
	}
	public void ShowMenuConfirm()
	{
		menuConfirmCanvas.SetActive(true);
	}
	public void ShowViewCanvas()
	{
		viewCanvas.SetActive(true);
	}
}
