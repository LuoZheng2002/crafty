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
	public int current_level_num = 0;
	public float move_to_pig_time = 0.5f;
	public float camera_rotation_time = 0.5f;
	bool first_person = true;
	Transform cameraRefTransform;
	bool piggy_permit_invisible = false;
	private void Start()
	{
		piggyCameraPivot = GameObject.Find("PiggyCameraPivot").GetComponent<PiggyCameraPivot>();
		Debug.Assert(piggyCameraPivot != null);
		piggyCameraEndTransform = piggyCameraPivot.transform.GetChild(0);
		cameraRefTransform = piggyCameraPivot.transform;
		EventBus.Subscribe<PiggyInstantiatedEvent>(OnPiggyInstantiated);
		EventBus.Subscribe<GameStateChangedEvent>(OnGameStateChanged);
		EventBus.Subscribe<FirstPersonChangedEvent>(OnFirstPersonChanged);
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
			Debug.Log("Rotating");
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
		Debug.Log("Game state changed event fired");
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

	}
	void TransitionToBuild()
	{

	}
	void TransitionToPlay()
	{
		Debug.Log("Transitioning to play");
		piggyCameraPivot.StartFollow(piggyPreview);
		// coroutine that moves camera to position
		StartCoroutine(CameraToPig());
		// start updating camera position according to piggyCameraPivot
	}
	void TransitionToOutro()
	{

	}
}
