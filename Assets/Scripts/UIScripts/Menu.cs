using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
	public float minScale = 0.8f;
	public float maxScale = 1.2f;
	public float scaleSpeed = 5.0f;
	Image image;
	private void OnEnable()
	{
		image = GetComponent<Image>();
		StartCoroutine(Scale());
	}
	IEnumerator Scale()
	{
		while (!GameState.shown_menu)
		{
			float scale = (Mathf.Sin(Time.time * scaleSpeed) + 1.0f) / 2.0f * (maxScale - minScale) + minScale;
			// Debug.Log($"Scale: {scale}");
			image.rectTransform.localScale = new Vector3(scale, scale, scale);
			yield return null;
		}
		image.rectTransform.localScale = Vector3.one;
	}

	public GameObject mainMenu;
    public void OnMenuClicked()
    {
        mainMenu.SetActive(true);
		GameState.shown_menu = true;
    }
}
