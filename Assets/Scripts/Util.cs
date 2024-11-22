using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RotationInfo
{
	public Vec3 attach_dir;
    public RotationInfo()
    {
        
    }
    public RotationInfo(Vec3 attach_dir)
    {
        this.attach_dir = attach_dir;
    }
}

public static class Util
{
	public static float position_spring = 500000.0f;
	public static float position_damper = 50000.0f;
	// occupied type
	// update
	public enum ChoiceName
	{
		None,
		DontNeedHelp,
		NeedHelp,
		DontCare
	}
	public enum BuildInfo
	{
		None,
		NeedHelp,
		DontNeedHelp,
		DontNeedHelpButRetry
	}
	public class ChoiceObj
	{
		public ChoiceName choice_name;
        public ChoiceObj(ChoiceName choice_name)
        {
            this.choice_name = choice_name;
        }
        public ChoiceObj()
        {
            choice_name = ChoiceName.None;
        }
    }
	public enum TRefName
	{
		Origin,
		PigPrestory1,
		PartnerPrestory1,
		PigPrestory2,
		PartnerPrestory2,
		CameraPrestory1_1,
		CameraPrestory1_2,
		CameraPrestory2_1,
		CameraPrestory2_2,
		CameraPrestory2_3,
		CameraPrestory2_4,
		CameraInTown1,
		CameraInTown2,
		PigTownW,
		PartnerTownW,
		CameraTownW1,
		CameraTownW2,
		PartnerC1S1,
		CameraC1S1,
		PartnerC1S2,
		PiggyC1S2,
		NPC1C1S2,
		NPC2C1S2,
		NPC3C1S2,
		CameraC1S2_1,
		CameraC1S2_2,
		CameraC1S2_3,
		CameraC1S2_4
	}
	public enum CharacterName
	{
		Piggy,
		Partner,
		NPC1,
		NPC2,
		NPC3,
	}
	public enum GameStateType
	{
		Intro,
		Build,
		Play,
		Outro
	}

	public enum CursorMode
	{
		Idle,
		AddComponent,
		ChangeDirection,
		Erase
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

	public static List<(Quaternion, RotationInfo)> WheelRotations = new ()
	{
		(Quaternion.Euler(0f, 0f, 0f), new RotationInfo(new Vec3(1, 0, 0))),
		(Quaternion.Euler(0f, 90.0f, 0f),new RotationInfo(new Vec3(1, 0, 0))),
		(Quaternion.Euler(0.0f, 0f, 90.0f), new RotationInfo(new Vec3(0, -1, 0))),
		(Quaternion.Euler(90.0f, 0.0f, 90.0f),new RotationInfo(new Vec3(0, -1, 0))),
		(Quaternion.Euler(0.0f, 0.0f, 270.0f),new RotationInfo(new Vec3(0, 1, 0))),
		(Quaternion.Euler(90.0f, 0.0f, 270.0f),new RotationInfo(new Vec3(0, 1, 0))),
		(Quaternion.Euler(0.0f, 90.0f, 90.0f),new RotationInfo(new Vec3(0, 0, 1))),
		(Quaternion.Euler(90.0f, 90.0f, 90.0f),new RotationInfo(new Vec3(0, 0, 1))),
		(Quaternion.Euler(0.0f, -90.0f, 90.0f),new RotationInfo(new Vec3(0, 0, -1))),
		(Quaternion.Euler(90.0f, -90.0f, 90.0f),new RotationInfo(new Vec3(0, 0, -1))),
		(Quaternion.Euler(0.0f, 0.0f, 180.0f),new RotationInfo(new Vec3(-1, 0, 0))),
		(Quaternion.Euler(0.0f, 90.0f, 180.0f),new RotationInfo(new Vec3(-1, 0, 0))),
	};
	public static List<(Quaternion, RotationInfo)> UmbrellaRotations = new()
	{
		(Quaternion.Euler(0f, 0f, 0f), new RotationInfo(new Vec3(-1, 0, 0))),
		(Quaternion.Euler(0.0f, 0f, 90.0f),new RotationInfo(new Vec3(0, -1, 0))),
		(Quaternion.Euler(0.0f, 0.0f, 270.0f),new RotationInfo(new Vec3(0, 1, 0))),
		(Quaternion.Euler(0.0f, 90.0f, 90.0f),new RotationInfo(new Vec3(0, 0, 1))),
		(Quaternion.Euler(0.0f, -90.0f, 90.0f),new RotationInfo(new Vec3(0, 0, -1))),
		(Quaternion.Euler(0.0f, 0.0f, 180.0f),new RotationInfo(new Vec3(1, 0, 0))),
	};
	public static List<(Quaternion, RotationInfo)> BoosterRotations = new()
    {
		(Quaternion.Euler(0.0f, 90.0f, 90.0f),new RotationInfo(new Vec3(0, 0, 1))),
		(Quaternion.Euler(0f, 0f, 0f), new RotationInfo(new Vec3(-1, 0, 0))),
        (Quaternion.Euler(0.0f, 0f, 90.0f),new RotationInfo(new Vec3(0, -1, 0))),
        (Quaternion.Euler(0.0f, 0.0f, 270.0f),new RotationInfo(new Vec3(0, 1, 0))),
        (Quaternion.Euler(0.0f, -90.0f, 90.0f),new RotationInfo(new Vec3(0, 0, -1))),
        (Quaternion.Euler(0.0f, 0.0f, 180.0f),new RotationInfo(new Vec3(1, 0, 0))),
    };

    public static Dictionary<WaypointName, List<(Component,int)>> WaypointItems = new()
	{
		{WaypointName.PreStory1, new(){(Component.Pig, 1), (Component.Partner, 1), (Component.WoodenCrate, 6), (Component.Wheel, 4)} },
		//{WaypointName.PreStory2, new(){(Component.Pig, 1), (Component.Partner, 1), (Component.WoodenCrate, 9), (Component.TurnWheel, 2), (Component.MotorWheel, 2)} },
		//{WaypointName.C1S1, new(){ (Component.Pig, 1), (Component.Partner, 1), (Component.WoodenCrate, 9), (Component.TurnWheel, 2), (Component.MotorWheel, 2) } },
		//{WaypointName.Volcano, new(){ (Component.Pig, 1), (Component.Partner, 1), (Component.WoodenCrate, 9), (Component.TurnWheel, 2), (Component.MotorWheel, 2), (Component.Umbrella, 4), (Component.Rocket, 6) } },
		//{WaypointName.VolcBottom, new(){ (Component.Pig, 1), (Component.Partner, 1), (Component.WoodenCrate, 9), (Component.TurnWheel, 2), (Component.MotorWheel, 2), (Component.Umbrella, 4), (Component.Rocket, 6) } },
		//{WaypointName.VolcTop, new(){ (Component.Pig, 1), (Component.Partner, 1), (Component.WoodenCrate, 9), (Component.TurnWheel, 2), (Component.MotorWheel, 2), (Component.Umbrella, 4), (Component.Rocket, 6) } },

	};

	public static Dictionary<Component, ComponentType> ContentInfos = new()
	{
		{Component.Pig, ComponentType.Load }, // content preview, sprite
		{Component.Partner, ComponentType.Load},
		{Component.WoodenCrate, ComponentType.Crate },
		{Component.Wheel, ComponentType.Accessory },
		{Component.MotorWheel, ComponentType.Accessory },
		{Component.TurnWheel, ComponentType.Accessory },
	};
	// crate, accessory, load

	public enum QuestName
	{
		None,
		Volcano,
		Quest1,
		Quest2,
	}

	public enum StoryName
	{
		None,
		Crash,
		Intro,
		FallOffCliff,
		InTown,
		TownWaypoint,
		C1S1,
		C1S2,

	}

	public enum GoalName
	{
		None,
		PreStory1,
		FallOffCliff,
		TurnWheel,
		PreStory2,
		Umbrella,
		Rocket,
		Town,
		C1S1,
		C1S2,
		Volcano,
		VolcBottom,
		VolcTop,
		VolcAfter
	}

	public enum WaypointName
	{
		None,
		PreStory1,
		Cliff,
		MotorWheel,
		TurnWheel,
		Turn1,
		Turn2,
		Turn3,
		Lake,
		Island,
		TownEntrance,
		TownWaypoint,
		Gate,
		Cliff2,
		Whirl,
		VolcanoGate,
		VolcTop,
		VolcPlatform,
		VolcPlatform2,
		VolcEntrance,
		VolcRoom1_1,
		VolcRoom1_2,
		VolcRoom2_1,
		VolcRoom3,
		VolcRoom4,
		VolcRoom1_3,
	}

	public static (Component[,,], Component[,,], Component[,,]) DesignPrestory1()
	{
		Component[,,] crates = new Component[2, 2, 3];
		Component[,,] loads = new Component[2, 2, 3];
		Component[,,] accessories = new Component[2, 2, 3];
		crates[1, 0, 0] = Component.WoodenCrate;
		crates[1, 0, 1] = Component.WoodenCrate;
		crates[1, 0, 2] = Component.WoodenCrate;
		crates[1, 1, 0] = Component.WoodenCrate;
		crates[1, 1, 1] = Component.WoodenCrate;
		crates[1, 1, 2] = Component.WoodenCrate;
		loads[1, 1, 2] = Component.Pig;
		loads[1, 0, 2] = Component.Partner;
		accessories[0, 0, 0] = Component.Wheel;
		accessories[0, 0, 2] = Component.Wheel;
		accessories[0, 1, 0] = Component.Wheel;
		accessories[0, 1, 2] = Component.Wheel;
		return (crates, accessories, loads);
	}
	public static (Component[,,], Component[,,], Component[,,]) DesignPrestory2()
	{
		Component[,,] crates = new Component[2, 3, 3];
		Component[,,] loads = new Component[2, 3, 3];
		Component[,,] accessories = new Component[2, 3, 3];
		crates[1, 0, 0] = Component.WoodenCrate;
		crates[1, 0, 1] = Component.WoodenCrate;
		crates[1, 0, 2] = Component.WoodenCrate;
		crates[1, 1, 0] = Component.WoodenCrate;
		crates[1, 1, 1] = Component.WoodenCrate;
		crates[1, 1, 2] = Component.WoodenCrate;
		crates[1, 2, 0] = Component.WoodenCrate;
		crates[1, 2, 1] = Component.WoodenCrate;
		crates[1, 2, 2] = Component.WoodenCrate;
		loads[1, 1, 2] = Component.Pig;
		loads[1, 0, 2] = Component.Partner;
		accessories[0, 0, 0] = Component.MotorWheel;
		accessories[0, 0, 2] = Component.TurnWheel;
		accessories[0, 2, 0] = Component.MotorWheel;
		accessories[0, 2, 2] = Component.TurnWheel;
		return (crates, accessories, loads);
	}

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
	public enum ComponentType
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
	public enum Component
	{
		None,
		Pig,
		Partner,
		// crate type
		WoodenCrate,
		SteelCrate,

		// accessory
		Wheel,
		TurnWheel,
		MotorWheel,
		Umbrella,
		Propeller,
		Fan,
		Rocket,

		// load
		Motor,
		Engine,
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
	public static void CreateJoint(MonoBehaviour a, MonoBehaviour b, float break_force, float break_torque)
	{
		// Debug.Log("Added a configurable joint");
		ConfigurableJoint configurableJoint = a.AddComponent<ConfigurableJoint>();
		configurableJoint.connectedBody = b.GetComponent<Rigidbody>();
		JointDrive drive = new JointDrive();
		drive.positionSpring = position_spring;
		drive.positionDamper = position_damper;
		drive.maximumForce = Mathf.Infinity;
		configurableJoint.xMotion = ConfigurableJointMotion.Limited;
		configurableJoint.yMotion = ConfigurableJointMotion.Limited;
		configurableJoint.zMotion = ConfigurableJointMotion.Limited;
		configurableJoint.angularXMotion = ConfigurableJointMotion.Limited;
		configurableJoint.angularYMotion = ConfigurableJointMotion.Limited;
		configurableJoint.angularZMotion = ConfigurableJointMotion.Limited;
		configurableJoint.linearLimit = new SoftJointLimit { limit = 0.1f };
		configurableJoint.highAngularXLimit = new SoftJointLimit { limit = 0.1f };
		configurableJoint.lowAngularXLimit = new SoftJointLimit { limit = 0.1f };
		configurableJoint.angularYLimit = new SoftJointLimit { limit = 0.1f };
		configurableJoint.angularZLimit = new SoftJointLimit { limit = 0.1f };
		//configurableJoint.linearLimitSpring = new SoftJointLimitSpring { spring = position_spring, damper = position_damper };
		//configurableJoint.angularXLimitSpring = new SoftJointLimitSpring { spring = position_spring, damper = position_damper };
		//configurableJoint.angularYZLimitSpring = new SoftJointLimitSpring { spring = position_spring, damper = position_damper };
		configurableJoint.xDrive = drive;
		configurableJoint.yDrive = drive;
		configurableJoint.zDrive = drive;
		configurableJoint.rotationDriveMode = RotationDriveMode.XYAndZ;
		configurableJoint.angularXDrive = drive;
		configurableJoint.angularYZDrive = drive;

		configurableJoint.breakForce = break_force;
		configurableJoint.breakTorque = break_torque;
		if (unbreakable)
		{
			configurableJoint.breakForce = Mathf.Infinity;
			configurableJoint.breakTorque = Mathf.Infinity;
		}
	}
	public static bool unbreakable = false;
	public static float break_force = 10000.0f;
	public static float break_torque = 10000.0f;

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
