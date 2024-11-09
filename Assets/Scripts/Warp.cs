using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Warp : MonoBehaviour
{
	public void WarpTo(MonoBehaviour target)
	{
		transform.position = target.transform.position;
		transform.rotation = target.transform.rotation;
	}
	public void Transition(MonoBehaviour a, MonoBehaviour b, float time)
	{
		StartCoroutine(TransitionHelper(a, b, time));
	}
	IEnumerator TransitionHelper(MonoBehaviour a, MonoBehaviour b, float time)
	{
		float start_time = Time.time;
		Vector3 start_pos = a.transform.position;
		Vector3 end_pos = b.transform.position;
		Quaternion start_rotation = a.transform.rotation;
		Quaternion end_rotation = b.transform.rotation;
		while (Time.time - start_time < time)
		{
			transform.position = Vector3.Lerp(start_pos, end_pos, (Time.time - start_time) / time);
			transform.rotation = Quaternion.Slerp(start_rotation, end_rotation, (Time.time - start_time) / time);
			yield return null;
		}
		transform.position = end_pos;
		transform.rotation = end_rotation;
	}
}
