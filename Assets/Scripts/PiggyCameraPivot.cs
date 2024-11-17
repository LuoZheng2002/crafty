using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public class PiggyCameraPivot : MonoBehaviour
//{
//	Transform piggyTransform;         // The object the camera will follow
//	public float smoothSpeed = 0.125f;  // Speed of the smoothing
//	public float rotationSmoothSpeed = 0.125f;
//	public float min_dist = 3.0f;
//	public float max_dist = 15.0f;
//	public float zoom_speed = 0.25f;
//	public float default_dist = 7.0f;
//	float current_dist = 0.0f;
//	public Vector3 dragEulerAngle;
//	public Transform CameraRef { get;private set; }
//	Transform cameraRefTarget;
//	public float CurrentDist {
//		get
//		{
//			return current_dist;
//		}
//		set
//		{
//			current_dist = value;
//			if (cameraEndTransform == null)
//			{
//				cameraEndTransform = transform.GetChild(0);
//			}
//			cameraEndTransform.localPosition = new Vector3(0, 0, -current_dist);
//		}
//	}
//	Transform cameraEndTransform;

//	private Vector3 velocity = Vector3.zero;  // Velocity reference for SmoothDamp
//	private Quaternion rotationVelocity = Quaternion.identity;
//	bool following = false;
//	static PiggyCameraPivot inst = null;
//	public static PiggyCameraPivot Inst
//	{
//		get
//		{
//			Debug.Assert(inst != null, "Piggy Camera Pivot instance not set");
//			return inst;
//		}
//	}
//	private void Start()
//	{
//		Debug.Assert(inst == null, "Piggy Camera Pivot instance already set");
//		inst = this;
//		cameraEndTransform = transform.Find("CameraEnd");
//		Debug.Assert(cameraEndTransform != null, "camera end transform is null");
//		dragEulerAngle = new Vector3 (0, 0, 0);
//		CurrentDist = default_dist;
//		cameraRefTarget = transform;
//		CameraRef = transform.Find("CameraRef");
//		Debug.Assert(CameraRef != null);
//	}
//	private void OnDestroy()
//	{
//		inst = null;
//	}
//	public void StartFollow(PiggyPreview piggyPreview)
//	{
//		following = true;
//		piggyTransform = piggyPreview.transform;
//	}
//	public void EndFollow()
//	{
//		following= false;
//		piggyTransform = null;
//		dragEulerAngle = Vector3.zero;
//	}
	
//	public void OnFirstPersonChanged(bool first_person)
//	{
//		if (first_person)
//		{
//			cameraRefTarget = transform;
//		}
//		else
//		{
//			cameraRefTarget = cameraEndTransform;
//		}
//	}
//	private void Update()
//	{
//		CameraRef.position = cameraRefTarget.position;
//		CameraRef.rotation = cameraRefTarget.rotation;
//	}
//	void LateUpdate()
//	{
//		// Calculate the target position with the offset
//		if (following)
//		{
//			Vector3 targetPosition = piggyTransform.position;
//			// Smoothly move the camera towards the target position
//			// transform.position = piggyTransform.position;
//			transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothSpeed);
//			Quaternion draggedRotation = piggyTransform.rotation * Quaternion.Euler(dragEulerAngle);
//			transform.rotation = Util.QuaternionSmoothDamp(transform.rotation, draggedRotation, ref rotationVelocity, rotationSmoothSpeed, Time.deltaTime);
//			// transform.rotation = piggyTransform.rotation;
//		}
//		if (Input.mouseScrollDelta.y != 0)
//		{
//			CurrentDist = Mathf.Clamp(CurrentDist - Input.mouseScrollDelta.y * zoom_speed, min_dist, max_dist);
//		}
//	}
//}
