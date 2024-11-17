using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

public class MainCamera : Warp
{
    // Start is called before the first frame update
	public Camera Camera { get; set; }
    public static MainCamera Inst
    {
        get { Debug.Assert(inst != null, "Main Camera Not Set"); return inst; }
    }
    static MainCamera inst;
    void Start()
    {
        Debug.Assert(inst == null, "Main Camera Already Set");
        inst = this;
		Camera = GetComponent<Camera>();
		Debug.Assert(Camera != null);
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
	public void MoveAndStickTo(Transform target_transform)
	{
		float time = 1.0f;
		StartCoroutine(MoveAndStickToHelper(time, target_transform));
	}
	IEnumerator MoveAndStickToHelper(float time, Transform target_transform)
	{
		// Transform target_transform = CarCore.Inst.CameraEnd;
		transformToFollow = null;
		float start_time = Time.time;
		Vector3 initial_position = transform.position;
		Quaternion initial_rotation = transform.rotation;
		Quaternion target_rotation = target_transform.rotation;
		while (Time.time - start_time < time)
		{
			transform.position = Vector3.Lerp(initial_position, target_transform.position, (Time.time - start_time) / time);
			transform.rotation = Quaternion.Slerp(initial_rotation, target_rotation, (Time.time - start_time) / time);
			yield return null;
		}
		transform.position = target_transform.position;
		transform.rotation = target_rotation;
		transformToFollow = target_transform;
	}
	//   public void MoveAndStickToGridMatrix(float rotate1_time, float move_time, float rotate2_time)
	//   {
	//       StartCoroutine(MoveAndStickToGridMatrixHelper(rotate1_time, move_time, rotate2_time));
	//   }
	//   IEnumerator MoveAndStickToGridMatrixHelper(float rotate1_time, float move_time, float rotate2_time)
	//   {
	//	yield return null;
	// //      Transform target_transform = GridMatrix.Current.DummyCamera;
	// //      transformToFollow = null;
	// //      float start_time = Time.time;
	// //      Quaternion initial_rotation = transform.rotation;
	// //      Vector3 dir = target_transform.position - transform.position;
	// //      Quaternion target_rotation = Quaternion.LookRotation(dir);
	// //      while (Time.time - start_time < rotate1_time)
	// //      {
	// //          transform.rotation = Quaternion.Slerp(initial_rotation, target_rotation, (Time.time - start_time) / rotate1_time);
	// //          yield return null;
	// //      }
	// //      transform.rotation = target_rotation;
	// //      Vector3 initial_position = transform.position;
	// //      Vector3 target_position = target_transform.position;
	// //      start_time = Time.time;
	// //      while(Time.time - start_time < move_time)
	// //      {
	// //          transform.position = Vector3.Lerp(initial_position, target_position, (Time.time - start_time) / move_time);
	// //          yield return null;
	// //      }
	// //      transform.position = target_position;
	//	//initial_rotation = transform.rotation;
	//	//target_rotation = target_transform.rotation;
	//	//start_time = Time.time;
	//	//while (Time.time - start_time < rotate2_time)
	//	//{
	//	//	transform.rotation = Quaternion.Slerp(initial_rotation, target_rotation, (Time.time - start_time) / rotate2_time);
	//	//	yield return null;
	//	//}
	//	//transform.rotation = target_rotation;
	// //      transformToFollow = target_transform;
	//}
		//public void MoveAndStickToPig(float move_to_pig_time, float camera_rotation_time)
  //  {
  //      StartCoroutine(MoveAndStickToPigHelper(move_to_pig_time, camera_rotation_time));
  //  }
    public void FollowStory()
    {
        transformToFollow = StoryAnimation.Inst.StoryCamera.transform;
    }
    public void Stop()
    {
        transformToFollow = null;
    }
 
}
