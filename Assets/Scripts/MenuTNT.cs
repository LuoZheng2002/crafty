using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuExplodeEvent
{

}

public class MenuTNT : MonoBehaviour
{
	bool exploded = false;
	public Collider box_collider;
	public SphereCollider sphere_collider;
	public ParticleSystem particle_system;
	private void Start()
	{
		
	}
	void Explode()
	{
		Debug.Log("Exploded!");
		box_collider.enabled = false;
		exploded = true;
		gameObject.layer = 14;
		sphere_collider.enabled = true;
		sphere_collider.radius = 0.1f;
		transform.AddComponent<FixedJoint>();
		foreach(Transform child in transform)
		{
			if (child.GetComponent<ParticleSystem>() == null)
				Destroy(child.gameObject);
		}
		StartCoroutine(GrowSphere());
		EventBus.Publish(new MenuExplodeEvent());
		particle_system.Play();
		AudioPlayer.Inst.Explode();
	}
	IEnumerator Countdown()
	{
		yield return new WaitForSeconds(2.0f);
		SceneManager.LoadScene(3);
	}
	IEnumerator GrowSphere()
	{
		float start_time = Time.time;
		float duration = 1.0f;
		while(Time.time - start_time < duration)
		{
			sphere_collider.radius = Mathf.Lerp(0.1f, 7.0f, (Time.time - start_time) / duration);
			yield return null;
		}
		sphere_collider.enabled = false;
		StartCoroutine(Countdown());
	}
	private void OnTriggerEnter(Collider other)
	{
		if (!exploded)
		{
			Explode();
		}
	}
	private void OnCollisionEnter(Collision collision)
	{
		if (!exploded)
		{
			Explode();
		}
	}

}
