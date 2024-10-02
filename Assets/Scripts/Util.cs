using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Util
{
	// occupied type
	// update
	public enum ContentType
	{
		None,
		Crate,
		Accessory,
		Load
	}
	public enum Content
	{
		// crate type
		WoodenCrate,
		IronCrate,

		// accessory
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
		public CratePreview? crate;
		public AccessoryPreview? accessory;
		public LoadPreview? load;
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

}
