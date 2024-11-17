using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoalCanvas : MonoBehaviour
{
	static GoalCanvas inst;
	public Image goal_icon;
	public Image up_image;
	public Image down_image;
	public Image left_image;
	public Image right_image;
	public static GoalCanvas Inst
	{
		get { Debug.Assert(inst != null, "Goal Canvas Not Set"); return inst; }
	}
	Util.WaypointName checkpoint_name_to_follow = Util.WaypointName.None;
	Checkpoint checkpoint_to_follow = null;
	public Util.WaypointName CheckpointToFollow
	{
		get { return checkpoint_name_to_follow; }
		set
		{
			checkpoint_name_to_follow = value;
			if (checkpoint_name_to_follow != Util.WaypointName.None)
			{
				checkpoint_to_follow = Checkpoint.Get(checkpoint_name_to_follow);
				goal_icon.gameObject.SetActive(true);
			}
			else
			{
				checkpoint_to_follow = null;
				goal_icon.gameObject.SetActive(false);
			}
		}
	}
	private void Start()
	{
		Debug.Assert(inst == null, "Goal Canvas Already Set");
		inst = this;
		goal_icon.gameObject.SetActive(false);
	}
	private void OnDestroy()
	{
		inst = null;
	}
	void Update()
	{
		if (checkpoint_to_follow == null)
		{
			return;
		}
		// World position of the object
		Vector3 worldPosition = checkpoint_to_follow.transform.position;

		// Convert to screen space
		Vector3 screenPosition = MainCamera.Inst.Camera.WorldToScreenPoint(worldPosition);

		// Screen dimensions
		float screenWidth = Screen.width;
		float screenHeight = Screen.height;

		// Normalize to [-1, 1]
		float normalizedX = (screenPosition.x / screenWidth) * 2 - 1;
		float normalizedY = (screenPosition.y / screenHeight) * 2 - 1;
		float width = goal_icon.rectTransform.rect.width;
		float height = goal_icon.rectTransform.rect.height;
		float clamped_x = Mathf.Clamp(screenPosition.x, width / 2, Screen.width - width / 2);
		float clamped_y = Mathf.Clamp(screenPosition.y, height / 2, Screen.height - height / 2);

		if ((screenPosition.z >=0&& normalizedX < -1.0f) || (screenPosition.z < 0 && normalizedX >= 0.0f))
		{
			left_image.enabled = true;
			clamped_x = width / 2;
		}
		else
		{
			left_image.enabled = false;
		}
		if ((screenPosition.z >=0&& normalizedX > 1.0f )|| (screenPosition.z < 0 && normalizedX < 0.0f))
		{
			right_image.enabled = true;
			clamped_x = Screen.width - width / 2;
		}
		else
		{
			right_image.enabled = false;
		}
		if ((screenPosition.z >= 0&& normalizedY < -1.0f)|| (screenPosition.z < 0 && normalizedY < -1.0f))
		{
			// if (screenPosition.z > 0 && normalizedY < -1.0f)
			down_image.enabled = true;
			// clamped_y = height / 2;
		}
		else
		{
			down_image.enabled = false;
		}
		if ((screenPosition.z >= 0&& normalizedY > 1.0f) || (screenPosition.z < 0 && normalizedY >= 1.0f))
		{
			// if (screenPosition.z > 0 && normalizedY > 1.0f)
			up_image.enabled = true;
			// clamped_y = Screen.height - height / 2;
		}
		else
		{
			up_image.enabled = false;
		}

		
		goal_icon.rectTransform.position = new Vector3(clamped_x, clamped_y, 0);

		// Flip Y-axis
		normalizedY = -normalizedY;

		// Debug.Log("Normalized X: " + normalizedX + ", Normalized Y: " + normalizedY);
	}
}
