using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ButtonScale : MonoBehaviour
{
	public float minScale = 0.8f;
	public float maxScale = 1.2f;
	public float scaleSpeed = 5.0f;
	Image image;

	IEnumerator coroutine = null;
	private void OnEnable()
	{
		image = GetComponent<Image>();
		image.rectTransform.localScale = Vector3.one;
	}
	public void ScaleStart()
	{
		if (coroutine == null && gameObject.activeInHierarchy)
		{
			coroutine = Scale();
			StartCoroutine(coroutine);
		}
		else
		{
			coroutine = Scale();
			StartCoroutine(coroutine);
		}
	}
	public void ScaleStop()
	{
		if (coroutine != null && gameObject.activeInHierarchy)
		{
			StopCoroutine(coroutine);
			coroutine = null;
		}
		if (image != null)
		{
			image.rectTransform.localScale = Vector3.one;
		}
	}
	private void OnDisable()
	{
		ScaleStop();
	}
	IEnumerator Scale()
	{
		yield return null;
		while (true)
		{
			float scale = (Mathf.Sin(Time.time * scaleSpeed) + 1.0f) / 2.0f * (maxScale - minScale) + minScale;
			image.rectTransform.localScale = new Vector3(scale, scale, scale);
			yield return null;
		}
	}
}
