using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaypointButton : MonoBehaviour
{
	Util.WaypointName waypoint_name;
	ButtonScale button_scale;
    public Util.WaypointName WaypointName
	{
		get
		{
			return waypoint_name;
		}
		set
		{
			waypoint_name = value;
			text.text = waypoint_name.ToString();
		}
	}
	public Checkpoint Checkpoint { get; set; }
	public Text text;
	Image image;
	public void StartScale()
	{
		button_scale = GetComponent<ButtonScale>();
		button_scale.ScaleStart();
	}
	private void Start()
	{
		image = GetComponent<Image>();
		button_scale = GetComponent<ButtonScale>();
		Debug.Assert(image != null);
	}
	public void OnClick()
	{
		MapCanvas.Inst.Deactivate();
		BigMapCamera.Inst.Deactivate();
		GameState.Inst.GoToCheckpointAsync(WaypointName);
		button_scale.ScaleStop();
	}
	private void Update()
	{
		if (Checkpoint != null)
		{
			Vector3 worldPosition = Checkpoint.transform.position;

			// Convert to screen space
			Vector3 screenPosition = MainCamera.Inst.Camera.WorldToScreenPoint(worldPosition);
			transform.position = screenPosition;
		}
	}
}
