using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCanvas : MonoBehaviour
{
    static MapCanvas inst;
	public WaypointButton waypoint_prefab;
	public Transform container;
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
		foreach(var checkpoint in Checkpoint.Checkpoints)
		{
			WaypointButton button = Instantiate(waypoint_prefab.gameObject, container).GetComponent<WaypointButton>();
			Debug.Assert(button != null);
			button.WaypointName = checkpoint.Key;
			button.Checkpoint = checkpoint.Value;
		}
	}
	public void Deactivate()
	{
		foreach(Transform child in container)
		{
			Destroy(child.gameObject);
		}
	}
	// Update is called once per frame
	void Update()
    {
        
    }
}
