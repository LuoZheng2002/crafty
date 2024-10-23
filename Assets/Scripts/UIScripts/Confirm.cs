using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PiggyRemovedEvent
{

}
public class Confirm : MonoBehaviour
{
	Image image;
	bool has_piggy = false;
	Color transparentColor;
	Color solidColor;
	private void Start()
	{
		image = GetComponent<Image>();
		EventBus.Subscribe<ResetCountEvent>(OnTrash);
		EventBus.Subscribe<PiggyInstantiatedEvent>(OnPiggyInstantiated);
		EventBus.Subscribe<PiggyRemovedEvent>(OnPiggyRemoved);
		transparentColor = new Color(1, 1, 1, 0.2f);
		solidColor = Color.white;
		image.color = transparentColor;
	}
	void OnTrash(ResetCountEvent e)
	{
		image.color = transparentColor;
		has_piggy = false;
	}
	void OnPiggyRemoved(PiggyRemovedEvent e)
	{
		image.color = transparentColor;
		has_piggy = false;
	}
	void OnPiggyInstantiated(PiggyInstantiatedEvent e)
	{
		image.color = solidColor;
		has_piggy = true;
	}

	public float minScale = 0.8f;
	public float maxScale = 1.2f;
	public float scaleSpeed = 5.0f;
	private void OnEnable()
	{
		image = GetComponent<Image>();
		StartCoroutine(Scale());
	}
	IEnumerator Scale()
	{
		while (!has_piggy)
		{
			yield return null;
		}
		while (!GameState.shown_confirm)
		{
			float scale = (Mathf.Sin(Time.time * scaleSpeed) + 1.0f) / 2.0f * (maxScale - minScale) + minScale;
			// Debug.Log($"Scale: {scale}");
			image.rectTransform.localScale = new Vector3(scale, scale, scale);
			yield return null;
		}
		image.rectTransform.localScale = Vector3.one;
	}


	public void OnConfirmClicked()
    {
		if (has_piggy)
		{
			GameState.Inst.TransitionToPlay();
		}
		GameState.shown_confirm = true;
    }
}
