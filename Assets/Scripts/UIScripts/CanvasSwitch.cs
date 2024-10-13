using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasSwitch : MonoBehaviour
{
    public GameObject introCanvas;
    public GameObject buildCanvas;
    public GameObject playCanvas;
    public GameObject outroCanvas;
	private void Start()
	{
        EventBus.Subscribe<GameStateChangedEvent>(OnGameStateChanged);
        introCanvas.SetActive(false);
		buildCanvas.SetActive(false);
		playCanvas.SetActive(false);
		outroCanvas.SetActive(false);
	}
	void OnGameStateChanged(GameStateChangedEvent e)
    {
        switch (e.state)
        {
            case Util.GameStateType.Intro:
                introCanvas.SetActive(true);
				buildCanvas.SetActive(false);
				playCanvas.SetActive(false);
				outroCanvas.SetActive(false);
                break;
			case Util.GameStateType.Build:
				introCanvas.SetActive(false);
				buildCanvas.SetActive(true);
                playCanvas.SetActive(false);
                outroCanvas.SetActive(false);
                EventBus.Publish(new GenerateItemsEvent());
                EventBus.Publish(new TrashEvent());
                break;
            case Util.GameStateType.Play:
				introCanvas.SetActive(false);
				playCanvas.SetActive(true);
                buildCanvas.SetActive(false);
				outroCanvas.SetActive(false);
				break;
            case Util.GameStateType.Outro:
				introCanvas.SetActive(false);
				playCanvas.SetActive(false);
				buildCanvas.SetActive(false);
				outroCanvas.SetActive(true);
                break;
			default:
				introCanvas.SetActive(false);
				playCanvas.SetActive(false );
                buildCanvas.SetActive(false );
				outroCanvas.SetActive(false);
				break;
        }
    }
}
