using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiggyInstantiatedEvent
{
	public PiggyPreview piggyPreview;
	
    public PiggyInstantiatedEvent(PiggyPreview piggyPreview)
    {
        this.piggyPreview = piggyPreview;
    }
}
public class GameStateChangedEvent
{
	public Util.GameStateType state;
	public int level_num;
    public GameStateChangedEvent(Util.GameStateType state, int level_num)
    {
		this.state = state;
		this.level_num = level_num;
    }
}
public class FirstPersonChangedEvent
{
	public bool first_person;
    public FirstPersonChangedEvent(bool first_person)
    {
		this.first_person = first_person;
    }
}

public class GameState : MonoBehaviour
{
    PiggyPreview piggyPreview;
	public PiggyCameraPivot piggyCameraPivot;
	Transform piggyCameraEndTransform;
	bool camera_follow_pig = false;
	public int current_level_num = 1;
	public float move_to_pig_time = 0.5f;
	public float camera_rotation_time = 0.5f;
	public float retry_move_time = 0.5f;
	public float rise_height = 10.0f;
	public float rise_time = 2.0f;
	bool first_person = true;
	Transform cameraRefTransform;
	bool piggy_permit_invisible = false;

	Level CurrentLevel { get
		{
			Level level = GameObject.Find($"Level{current_level_num}").GetComponent<Level>();
			Debug.Assert(level != null);
			return level;
		} }
	private void Start()
	{
		piggyCameraPivot = GameObject.Find("PiggyCameraPivot").GetComponent<PiggyCameraPivot>();
		Debug.Assert(piggyCameraPivot != null);
		piggyCameraEndTransform = piggyCameraPivot.transform.GetChild(0);
		cameraRefTransform = piggyCameraPivot.transform;
		EventBus.Subscribe<PiggyInstantiatedEvent>(OnPiggyInstantiated);
		EventBus.Subscribe<GameStateChangedEvent>(OnGameStateChanged);
		EventBus.Subscribe<FirstPersonChangedEvent>(OnFirstPersonChanged);
		EventBus.Subscribe<RetryEvent>(OnRetry);
	}
	void OnRetry(RetryEvent e)
	{
		piggyCameraPivot.EndFollow();
		camera_follow_pig = false;
		piggy_permit_invisible = false;
		StartCoroutine(MoveCameraToGrid());
	}
	IEnumerator MoveCameraToGrid()
	{
		float startTime = Time.time;
		float endTime = startTime + retry_move_time;
		Transform cameraTransform = Camera.main.transform;
		Vector3 startPosition = cameraTransform.position;
		Quaternion startRotation = cameraTransform.rotation;
		Transform dummyCameraTransform = CurrentLevel.transform.Find("GridMatrix").Find("DummyCamera");
		Debug.Assert(dummyCameraTransform != null, "dummy camera transform is null");
		while (Time.time < endTime)
		{
			float progress = (Time.time - startTime) / retry_move_time;
			cameraTransform.position = Vector3.Lerp(startPosition, dummyCameraTransform.position, progress);
			cameraTransform.rotation = Quaternion.Slerp(startRotation, dummyCameraTransform.rotation, progress);
			yield return null;
		}
		cameraTransform.position = dummyCameraTransform.position;
		cameraTransform.rotation = dummyCameraTransform.rotation;
		// grid matrix will be enabled at transition to build
		EventBus.Publish(new GameStateChangedEvent(Util.GameStateType.Build, current_level_num));
	}
	IEnumerator CameraToPig()
	{
		Debug.Assert(piggyPreview != null);
		float start_time = Time.time;
		float end_time = start_time + move_to_pig_time;
		Transform cameraTransform = Camera.main.transform;
		Vector3 cameraStartPos = cameraTransform.position;
		// force pivot to move to place immediately
		piggyCameraPivot.transform.position = piggyPreview.transform.position;
		piggyCameraPivot.transform.rotation = piggyPreview.transform.rotation;
		while (Time.time < end_time)
		{
			if ((Time.time - start_time) / move_to_pig_time > 0.8)
			{
				piggy_permit_invisible = true;
				if (first_person)
				{
					piggyPreview.SetInvisible(true);
				}
			}
			cameraTransform.LookAt(cameraRefTransform, Vector3.up);
			cameraTransform.position = Vector3.Lerp(cameraStartPos, cameraRefTransform.position, (Time.time - start_time) / move_to_pig_time);
			yield return null;
		}
		cameraTransform.position = cameraRefTransform.position;

		// rotate to piggyCameraPivot's transform
		start_time = Time.time;
		end_time = start_time + camera_rotation_time;
		Quaternion cameraStartRotation = cameraTransform.rotation;
		while (Time.time < end_time)
		{
			// Debug.Log("Rotating");
			cameraTransform.rotation = Quaternion.Slerp(cameraStartRotation, cameraRefTransform.rotation, (Time.time - start_time) / camera_rotation_time);
			yield return null;
		}
		cameraTransform.rotation = cameraRefTransform.rotation;
		camera_follow_pig = true; // start constantly follow pig
	}
	void OnPiggyInstantiated(PiggyInstantiatedEvent e)
	{
		piggyPreview = e.piggyPreview;
	}
	void OnFirstPersonChanged(FirstPersonChangedEvent e)
	{
		first_person = e.first_person;
		if (first_person)
		{
			cameraRefTransform = piggyCameraPivot.transform;
		}
		else
		{
			cameraRefTransform = piggyCameraEndTransform;
		}
		if (piggyPreview != null)
		{
			if (first_person)
			{
				piggyPreview.SetInvisible(true);
			}
			else if (piggy_permit_invisible)
			{
				piggyPreview.SetInvisible(false);
			}
		}
	}
	private void Update()
	{
        if (camera_follow_pig)
		{
			Camera.main.transform.position = cameraRefTransform.position;
			Camera.main.transform.rotation = cameraRefTransform.rotation;
		}
    }
	void OnGameStateChanged(GameStateChangedEvent e)
	{
		// Debug.Log("Game state changed event fired");
		switch(e.state)
		{
			case Util.GameStateType.Intro:
				TransitionToIntro(e.level_num);
				break;
			case Util.GameStateType.Build:
				TransitionToBuild();
				break;
			case Util.GameStateType.Play:
				TransitionToPlay();
				break;
			case Util.GameStateType.Outro:
				TransitionToOutro();
				break;
		}
	}
	void TransitionToIntro(int level_num)
	{
		piggyCameraPivot.EndFollow();
		piggyCameraPivot.dragEulerAngle = Vector3.zero;
	}
	void TransitionToBuild()
	{
		piggy_permit_invisible = false;
		piggyCameraPivot.EndFollow();
		piggyCameraPivot.dragEulerAngle = Vector3.zero;
		GridMatrix gridMatrix = CurrentLevel.transform.Find("GridMatrix").GetComponent<GridMatrix>();
		Debug.Assert(gridMatrix != null, "grid matrix is null");
		gridMatrix.enabled = true;
		// Debug.Log("transition to build enabled");

	}
	void TransitionToPlay()
	{
		// Debug.Log("Transitioning to play");
		piggyCameraPivot.StartFollow(piggyPreview);
		// coroutine that moves camera to position
		StartCoroutine(CameraToPig());
		// start updating camera position according to piggyCameraPivot
	}
	void TransitionToOutro()
	{
		camera_follow_pig = false;
		StartCoroutine(RiseAndWatch());
	}
	IEnumerator RiseAndWatch()
	{
		float start_time = Time.time;
		float end_time = start_time + rise_time;
		Transform cameraTransform = Camera.main.transform;
		Vector3 initialPos = cameraTransform.position;
		Vector3 goalPos = cameraTransform.position + new Vector3(0, rise_height, 0);
		while(Time.time < end_time)
		{
			cameraTransform.position = Vector3.Lerp(initialPos, goalPos, (Time.time - start_time) / rise_time);
			cameraTransform.LookAt(piggyPreview.transform);
			yield return null;
		}
		while(piggy_permit_invisible)
		{
			cameraTransform.LookAt(piggyPreview.transform);
			yield return null;
		}
	}
}
