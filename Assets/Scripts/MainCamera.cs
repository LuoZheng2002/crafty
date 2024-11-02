using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    // Start is called before the first frame update
    public static MainCamera Inst
    {
        get { Debug.Assert(inst != null, "Main Camera Not Set"); return inst; }
    }
    static MainCamera inst;
    void Start()
    {
        Debug.Assert(inst == null, "Main Camera Already Set");
        inst = this;
    }

    // Update is called once per frame

    Transform transformToFollow;
    void Update()
    {
        if (transformToFollow != null)
        {
            transform.position = transformToFollow.position;
            transform.rotation = transformToFollow.rotation;
        }
    }
    public void MoveAndStickToGridMatrix(float rotate1_time, float move_time, float rotate2_time)
    {
        StartCoroutine(MoveAndStickToGridMatrixHelper(rotate1_time, move_time, rotate2_time));
    }
    IEnumerator MoveAndStickToGridMatrixHelper(float rotate1_time, float move_time, float rotate2_time)
    {
        Transform target_transform = GridMatrix.Current.DummyCamera;
        transformToFollow = null;
        float start_time = Time.time;
        Quaternion initial_rotation = transform.rotation;
        Vector3 dir = target_transform.position - transform.position;
        Quaternion target_rotation = Quaternion.LookRotation(dir);
        while (Time.time - start_time < rotate1_time)
        {
            transform.rotation = Quaternion.Slerp(initial_rotation, target_rotation, (Time.time - start_time) / rotate1_time);
            yield return null;
        }
        transform.rotation = target_rotation;
        Vector3 initial_position = transform.position;
        Vector3 target_position = target_transform.position;
        start_time = Time.time;
        while(Time.time - start_time < move_time)
        {
            transform.position = Vector3.Lerp(initial_position, target_position, (Time.time - start_time) / move_time);
            yield return null;
        }
        transform.position = target_position;
		initial_rotation = transform.rotation;
		target_rotation = target_transform.rotation;
		start_time = Time.time;
		while (Time.time - start_time < rotate2_time)
		{
			transform.rotation = Quaternion.Slerp(initial_rotation, target_rotation, (Time.time - start_time) / rotate2_time);
			yield return null;
		}
		transform.rotation = target_rotation;
        transformToFollow = target_transform;
	}
    public void FollowStory()
    {
        transformToFollow = StoryAnimation.Inst.AnimationCamera;
    }
    public void MoveAndStickToPig(float move_to_pig_time, float camera_rotation_time)
    {
        StartCoroutine(MoveAndStickToPigHelper(move_to_pig_time, camera_rotation_time));
    }
    public void Stop()
    {
        transformToFollow = null;
    }
    IEnumerator MoveAndStickToPigHelper(float move_to_pig_time, float camera_rotation_time)
    {
        transformToFollow = null;
		Debug.Assert(GameState.Inst.Piggy != null);
		float start_time = Time.time;
		float end_time = start_time + move_to_pig_time;
		Vector3 start_position = transform.position;
		// force pivot to move to place immediately
		PiggyCameraPivot.Inst.transform.position = GameState.Inst.Piggy.transform.position;
		PiggyCameraPivot.Inst.transform.rotation = GameState.Inst.Piggy.transform.rotation;
		while (Time.time < end_time)
		{
			if ((Time.time - start_time) / move_to_pig_time > 0.8)
			{
				GameState.Inst.PiggyPermitInvisible = true;
				if (GameState.Inst.FirstPerson)
				{
					EventBus.Publish(new InvisibleStateUpdateEvent());
				}
			}
			transform.LookAt(PiggyCameraPivot.Inst.CameraRef, Vector3.up);
			transform.position = Vector3.Lerp(start_position, PiggyCameraPivot.Inst.CameraRef.position, (Time.time - start_time) / move_to_pig_time);
			yield return null;
		}
		transform.position = PiggyCameraPivot.Inst.CameraRef.position;

		// rotate to piggyCameraPivot's transform
		start_time = Time.time;
		end_time = start_time + camera_rotation_time;
		Quaternion cameraStartRotation = transform.rotation;
		while (Time.time < end_time)
		{
			transform.rotation = Quaternion.Slerp(cameraStartRotation, PiggyCameraPivot.Inst.CameraRef.rotation, (Time.time - start_time) / camera_rotation_time);
			yield return null;
		}
		transform.rotation = PiggyCameraPivot.Inst.CameraRef.rotation;
        transformToFollow = PiggyCameraPivot.Inst.CameraRef;
	}
}
