using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiggyNose : MonoBehaviour
{
	public float rotationSmoothSpeed = 0.125f;
	private Quaternion rotationVelocity = Quaternion.identity;
	MeshRenderer meshRenderer;
	private void Start()
	{
		meshRenderer = transform.GetChild(0).GetComponent<MeshRenderer>();
		EventBus.Subscribe<PiggyDestroyEvent>(OnPiggyDestroy);
		EventBus.Subscribe<InvisibleStateUpdateEvent>(OnInvisibleChanged);
		meshRenderer.enabled = false;
	}
	void OnPiggyDestroy(PiggyDestroyEvent e)
	{
		meshRenderer.enabled = false;
	}
	void OnInvisibleChanged(InvisibleStateUpdateEvent e)
	{
		if (GameState.Inst.IsFirstPerson && GameState.Inst.PiggyPermitInvisible)
		{
			meshRenderer.enabled = true;
		}
		else
		{
			meshRenderer.enabled = false;
		}
	}
	private void LateUpdate()
	{
		transform.position = Camera.main.transform.position;
		transform.rotation = Util.QuaternionSmoothDamp(transform.rotation, Camera.main.transform.rotation, ref rotationVelocity, rotationSmoothSpeed, Time.deltaTime);
	}
}
