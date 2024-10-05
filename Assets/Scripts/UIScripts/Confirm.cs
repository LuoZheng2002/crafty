using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Confirm : MonoBehaviour
{
	Image image;
	bool has_piggy = false;
	Color transparentColor;
	Color solidColor;
	private void Start()
	{
		image = GetComponent<Image>();
		EventBus.Subscribe<TrashEvent>(OnTrash);
		EventBus.Subscribe<PiggyInstantiatedEvent>(OnPiggyInstantiated);
		transparentColor = new Color(1, 1, 1, 0.2f);
		solidColor = Color.white;
		image.color = transparentColor;
	}
	void OnTrash(TrashEvent e)
	{
		image.color = transparentColor;
		has_piggy = false;
	}
	void OnPiggyInstantiated(PiggyInstantiatedEvent e)
	{
		image.color = solidColor;
		has_piggy = true;
	}
	public void OnConfirmClicked()
    {
		if (has_piggy)
		{
			EventBus.Publish(new GameStateChangedEvent(Util.GameStateType.Play, 0));
		}
    }
}
