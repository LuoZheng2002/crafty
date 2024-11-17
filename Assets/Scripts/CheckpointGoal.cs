using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointGoal : MonoBehaviour
{
    // Start is called before the first frame update
    Checkpoint checkpoint;
    void Start()
    {
        checkpoint = transform.parent.GetComponent<Checkpoint>();
	}
	private void OnTriggerEnter(Collider other)
	{
		checkpoint.OnCheckpointGoalReached();
		gameObject.SetActive(false);
		Debug.Log($"{checkpoint.waypoint_name} reached and collider deactivated!");
	}
}
