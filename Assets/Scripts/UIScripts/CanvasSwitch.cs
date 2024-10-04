using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasSwitch : MonoBehaviour
{
    public GameObject buildCanvas;
    public GameObject playCanvas;
	private void Start()
	{
        EventBus.Subscribe<GameStateChangedEvent>(OnGameStateChanged);
	}
	void OnGameStateChanged(GameStateChangedEvent e)
    {
        switch (e.state)
        {
            case Util.GameStateType.Build:
                buildCanvas.SetActive(true);
                playCanvas.SetActive(false);
                break;
            case Util.GameStateType.Play:
                playCanvas.SetActive(true);
                buildCanvas.SetActive(false);
                break;
            default:
                playCanvas.SetActive(false );
                buildCanvas.SetActive(false );
                break;
        }
    }
}
