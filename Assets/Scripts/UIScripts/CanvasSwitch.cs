using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasSwitch : MonoBehaviour
{
    public GameObject buildCanvas;
    public GameObject playCanvas;
    public GameObject outroCanvas;
	private void Start()
	{
        EventBus.Subscribe<GameStateChangedEvent>(OnGameStateChanged);
		buildCanvas.SetActive(false);
		playCanvas.SetActive(false);
		outroCanvas.SetActive(false);
	}
	void OnGameStateChanged(GameStateChangedEvent e)
    {
        switch (e.state)
        {
            case Util.GameStateType.Build:
                buildCanvas.SetActive(true);
                playCanvas.SetActive(false);
                outroCanvas.SetActive(false);
                EventBus.Publish(new GenerateItemsEvent());
                EventBus.Publish(new TrashEvent());
                break;
            case Util.GameStateType.Play:
                playCanvas.SetActive(true);
                buildCanvas.SetActive(false);
				outroCanvas.SetActive(false);
				break;
            case Util.GameStateType.Outro:
				playCanvas.SetActive(false);
				buildCanvas.SetActive(false);
				outroCanvas.SetActive(true);
                break;
			default:
                playCanvas.SetActive(false );
                buildCanvas.SetActive(false );
				outroCanvas.SetActive(false);
				break;
        }
    }
}
