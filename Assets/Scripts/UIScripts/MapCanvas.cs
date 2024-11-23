using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCanvas : MonoBehaviour
{
    static MapCanvas inst;
	public WaypointButton waypoint_prefab;
	public Transform container;
	public bool ScaleTownWaypoint { get; set; } = false;
	public List<Util.WaypointName> PermWaypoints { get; } = new();
	public static MapCanvas Inst
	{
		get
		{
            Debug.Assert(inst != null, "MapCanvas.Inst is null");
			return inst;
		}
	}
	void Start()
    {
        Debug.Assert(inst == null, "MapCanvas.Inst is not null");
		inst = this;
	}
	private void OnDestroy()
	{
		inst = null;
	}
	public void Activate()
	{
		foreach(var checkpoint in PermWaypoints)
		{
			Debug.Assert(Checkpoint.Checkpoints.ContainsKey(checkpoint), $"Checkpoint {checkpoint} not found");

			WaypointButton button = Instantiate(waypoint_prefab.gameObject, container).GetComponent<WaypointButton>();
			Debug.Assert(button != null);
			button.WaypointName = checkpoint;
			button.Checkpoint = Checkpoint.Get(checkpoint);
			if (ScaleTownWaypoint&& checkpoint == Util.WaypointName.TownWaypoint)
			{
				button.StartScale();
			}
		}
		if (GameSave.CurrentCheckpoint != Util.WaypointName.None)
		{
			Debug.Assert(Checkpoint.Checkpoints.ContainsKey(GameSave.CurrentCheckpoint), $"Checkpoint {GameSave.CurrentCheckpoint} not found");
			WaypointButton button = Instantiate(waypoint_prefab.gameObject, container).GetComponent<WaypointButton>();
			Debug.Assert(button != null);
			button.WaypointName = GameSave.CurrentCheckpoint;
			button.Checkpoint = Checkpoint.Get(GameSave.CurrentCheckpoint);
		}
		ShowBack();		
	}
	public GameObject back;
	public void Deactivate()
	{
		foreach(Transform child in container)
		{
			Destroy(child.gameObject);
		}
		HideBack();
	}
	// Update is called once per frame
	void Update()
    {
        
    }
	public void HideBack()
	{
		back.SetActive(false);
	}
	public void ShowBack()
	{
		back.SetActive(true);
	}
	public void OnBack()
	{
		MainCamera.Inst.Stop();
		MainCamera.Inst.MoveAndStickTo(CarCore.Inst.CameraEnd);
		Deactivate();
		BigMapCamera.Inst.Deactivate();
		PlayCanvas.Inst.Show();
		ScaleTownWaypoint = false;
	}
}
