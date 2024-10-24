using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Util
{
	// occupied type
	// update
	public enum GameStateType
	{
		Intro,
		Build,
		Play,
		Outro
	}

	// Set the layer of the GameObject and all its children
	public static void SetLayerRecursively(GameObject obj, string newLayerName)
	{
		// If the object is null, return
		if (obj == null)
		{
			return;
		}

		// Set the layer of the current object
		obj.layer = LayerMask.NameToLayer(newLayerName);

		// Loop through and set the layer for all the children
		foreach (Transform child in obj.transform)
		{
			SetLayerRecursively(child.gameObject, newLayerName);
		}
	}

	public static List<(Quaternion, (int, int, int))> WheelRotations = new ()
	{
		(Quaternion.Euler(0f, 0f, 0f), (1, 0, 0)),
		(Quaternion.Euler(0f, 90.0f, 0f),(1, 0, 0)),
		(Quaternion.Euler(0.0f, 0f, 90.0f),(0, -1, 0)),
		(Quaternion.Euler(90.0f, 0.0f, 90.0f),(0, -1, 0)),
		(Quaternion.Euler(0.0f, 0.0f, 270.0f),(0, 1, 0)),
		(Quaternion.Euler(90.0f, 0.0f, 270.0f),(0, 1, 0)),
		(Quaternion.Euler(0.0f, 90.0f, 90.0f),(0, 0, 1)),
		(Quaternion.Euler(90.0f, 90.0f, 90.0f),(0, 0, 1)),
		(Quaternion.Euler(0.0f, -90.0f, 90.0f),(0, 0, -1)),
		(Quaternion.Euler(90.0f, -90.0f, 90.0f),(0, 0, -1)),
		(Quaternion.Euler(0.0f, 0.0f, 180.0f),(-1, 0, 0)),
		(Quaternion.Euler(0.0f, 90.0f, 180.0f),(-1, 0, 0)),
	};

	public static Dictionary<int, List<(Content,int)>> LevelItems = new()
	{
		{1, new(){(Content.Pig, 1), (Content.WoodenCrate, 6), (Content.Wheel, 4)} },
		{2, new(){(Content.Pig, 1), (Content.WoodenCrate, 9), (Content.Wheel, 4)} },
		{3, new(){(Content.Pig, 1), (Content.WoodenCrate, 9), (Content.Wheel, 4), (Content.TurnWheel, 2)} },
		{4, new (){(Content.Pig, 1), (Content.WoodenCrate, 9), (Content.Wheel, 4), (Content.MotorWheel, 2)} },
		{5, new (){(Content.Pig, 1), (Content.WoodenCrate, 9), (Content.Wheel, 4), (Content.MotorWheel, 2)} },
	};

	public static Dictionary<Content, ContentType> ContentInfos = new()
	{
		{Content.Pig, ContentType.Load }, // content preview, sprite
		{Content.WoodenCrate, ContentType.Crate },
		{Content.Wheel, ContentType.Accessory },
		{Content.MotorWheel, ContentType.Accessory },
		{Content.TurnWheel, ContentType.Accessory },
	};
	/// <summary>
	/// Delay 1 frame to execute the function
	/// </summary>
	static IEnumerator DelayHelper(Action func, int length)
	{
		for(int i =0; i < length;i++)
		{
			yield return null;
		}
		func();
	}
	public static void Delay(MonoBehaviour m, int length, Action func)
	{
		m.StartCoroutine(DelayHelper(func, length));
	}
	public static void Delay(MonoBehaviour m, Action func)
	{
		Delay(m, 1, func);
	}
	public enum ContentType
	{
		None,
		Crate,
		Accessory,
		Load,
		Erase
	}
	public enum TutorialType
	{
		Space,
		Drag,
		DragScreen,
		ThirdPerson
	}

	public enum NewTutorialType
	{
		Build,
		Wide,
		Turn,
		Power,
		Momentum
	}
	public enum Content
	{
		None,
		// crate type
		WoodenCrate,
		SteelCrate,

		// accessory
		Wheel,
		TurnWheel,
		MotorWheel,
		Propeller,
		Fan,

		// load
		Pig,
		Motor,
		Engine,
		QueenPig,
	}
	// IAccessoryPreview
	// ILoadPreview
	// 
	public struct GridContentInfo
	{
		public CrateBase crate;
		public AccessoryComponent accessory;
		public LoadComponent load;
		public bool Occupied { get { return crate != null || load != null || accessory != null; } }
		public bool AllowAccessory { get { return crate == null && load == null && accessory == null; } }
		public bool AllowCrate { get { return crate == null && accessory == null; } }
		public bool AllowLoad { get { return load == null && accessory == null; } }
	}
	public static float GetDistanceFromRayToPoint(Ray ray, Vector3 point)
	{
		// Get the direction of the ray
		Vector3 rayDirection = ray.direction;

		// Get the vector from the ray's origin to the point
		Vector3 originToPoint = point - ray.origin;

		// Project the point onto the ray's direction
		float projectionLength = Vector3.Dot(originToPoint, rayDirection);
		Vector3 closestPointOnRay = ray.origin + rayDirection * projectionLength;

		// Calculate the distance from the closest point on the ray to the point
		float distance = Vector3.Distance(closestPointOnRay, point);
		return distance;
	}
	public static void CreateJoint(MonoBehaviour a, MonoBehaviour b, float position_spring, float position_damper)
	{
		Debug.Log("Added a configurable joint");
		ConfigurableJoint configurableJoint = a.AddComponent<ConfigurableJoint>();
		configurableJoint.connectedBody = b.GetComponent<Rigidbody>();
		JointDrive drive = new JointDrive();
		drive.positionSpring = position_spring;
		drive.positionDamper = position_damper;
		drive.maximumForce = 1000000;
		configurableJoint.xDrive = drive;
		configurableJoint.yDrive = drive;
		configurableJoint.zDrive = drive;
		configurableJoint.rotationDriveMode = RotationDriveMode.XYAndZ;
		configurableJoint.angularXDrive = drive;
		configurableJoint.angularYZDrive = drive;
	}

	public static Quaternion QuaternionSmoothDamp(Quaternion current, Quaternion target, ref Quaternion velocity, float smoothTime, float deltaTime)
	{
		// Smoothly interpolate towards the target rotation
		float angle = Quaternion.Angle(current, target);

		if (angle > 0f)
		{
			float t = Mathf.SmoothDampAngle(0f, angle, ref velocity.w, smoothTime, Mathf.Infinity, deltaTime);
			Quaternion result = Quaternion.Slerp(current, target, t / angle);

			// Approximate velocity to track angular velocity
			velocity = Quaternion.Euler(
				Mathf.SmoothDampAngle(current.eulerAngles.x, target.eulerAngles.x, ref velocity.x, smoothTime),
				Mathf.SmoothDampAngle(current.eulerAngles.y, target.eulerAngles.y, ref velocity.y, smoothTime),
				Mathf.SmoothDampAngle(current.eulerAngles.z, target.eulerAngles.z, ref velocity.z, smoothTime)
			);

			return result;
		}

		return target;
	}
}
