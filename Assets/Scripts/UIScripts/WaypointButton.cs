using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaypointButton : MonoBehaviour
{
    public Util.WaypointName WaypointName { get; set; }
	public Checkpoint Checkpoint { get; set; }
	Image image;
	private void Start()
	{
		image = GetComponent<Image>();
		Debug.Assert(image != null);

	}
	public void OnClick()
	{
		MapCanvas.Inst.Deactivate();
		BigMapCamera.Inst.Deactivate();
		GameState.Inst.GoToCheckpoint(WaypointName);
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
