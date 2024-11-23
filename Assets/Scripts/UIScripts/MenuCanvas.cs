using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class MenuCanvas : MonoBehaviour
{
	public Animator animator;
 //   IEnumerator GoToGameHelper()
 //   {
 //       yield return BlackoutCanvas.Inst.Blackout(1.5f, 0.0f, 1.0f);
	//	SceneManager.LoadScene(1);
	//}
    public void OnPlayClicked()
    {
		StartCoroutine(OnPlayClickedHelper());
	}
	IEnumerator OnPlayClickedHelper()
	{
		animator.SetTrigger("play");
		float start_time = Time.time;
		float duration = 1.0f;
		while (Time.time - start_time < duration)
		{
			float alpha = Mathf.Lerp(1.0f, 0.0f, (Time.time - start_time) / duration);
			SetNameAlpha(alpha);
			SetPlayImageAlpha(alpha);
			SetPlayTextAlpha(alpha);
			SetQuitImageAlpha(alpha);
			SetQuitTextAlpha(alpha);
			yield return null;
		}
		SetNameAlpha(0);
		SetPlayImageAlpha(0);
		SetPlayTextAlpha(0);
		SetQuitImageAlpha(0);
		SetQuitTextAlpha(0);
	}
	private void Start()
	{
		SetNameAlpha(0);
		SetPlayImageAlpha(0);
		SetPlayTextAlpha(0);
		SetQuitImageAlpha(0);
		SetQuitTextAlpha(0);
		StartCoroutine(StartHelper());
	}
	public Text name;
	public Text play_text;
	public Image play_image;
	public Text quit_text;
	public Image quit_image;
	public void SetNameAlpha(float alpha)
	{
		Color c = name.color;
		c.a = alpha;
		name.color = c;
	}
	public void SetPlayImageAlpha(float alpha)
	{
		Color c = play_image.color;
		c.a = alpha;
		play_image.color = c;
	}
	public void SetPlayTextAlpha(float alpha)
	{
		Color c = play_text.color;
		c.a = alpha;
		play_text.color = c;
	}
	public void SetQuitImageAlpha(float alpha)
	{
		Color c = quit_image.color;
		c.a = alpha;
		quit_image.color = c;
	}
	public void SetQuitTextAlpha(float alpha)
	{
		Color c = quit_text.color;
		c.a = alpha;
		quit_text.color = c;
	}
	IEnumerator StartHelper()
	{
		yield return null;
		yield return BlackoutCanvas.Inst.Blackout(1.0f, 1.0f, 0.0f);
		float start_time = Time.time;
		float name_duration = 1.0f;
		while(Time.time - start_time < name_duration)
		{
			float alpha = Mathf.Lerp(0.0f, 1.0f, (Time.time - start_time) / name_duration);
			SetNameAlpha(alpha);
			yield return null;
		}
		start_time = Time.time;
		float play_duration = 1.0f;
		while (Time.time - start_time < play_duration)
		{
			float alpha1 = Mathf.Lerp(0.0f, 1.0f, (Time.time - start_time) / play_duration);
			float alpha2 = Mathf.Lerp(0.0f, 0.3f, (Time.time - start_time) / play_duration);
			SetPlayImageAlpha(alpha2);
			SetPlayTextAlpha(alpha1);
			yield return null;
		}
		start_time = Time.time;
		float quit_duration = 1.0f;
		while (Time.time - start_time < quit_duration)
		{
			float alpha1 = Mathf.Lerp(0.0f, 1.0f, (Time.time - start_time) / quit_duration);
			float alpha2 = Mathf.Lerp(0.0f, 0.3f, (Time.time - start_time) / quit_duration);
			SetQuitImageAlpha(alpha2);
			SetQuitTextAlpha(alpha1);
			yield return null;
		}
	}
}
