using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCanvasSwitch : MonoBehaviour
{
	public static bool menu_select_level = false;
    public GameObject menuCanvas;
    public GameObject levelCanvas;
	private void Start()
	{
		if (!menu_select_level)
		{
			menu_select_level = true;
			menuCanvas.SetActive(true);
			levelCanvas.SetActive(false);
		}
		else
		{
			menuCanvas.SetActive(false);
			levelCanvas.SetActive(true);
		}
	}
	public void SwitchToMenu()
	{
		menuCanvas.SetActive(true );
		levelCanvas.SetActive(false);
	}
	public void SwitchToLevel()
	{
		menuCanvas.SetActive(false );
		levelCanvas.SetActive(true);
	}
}
