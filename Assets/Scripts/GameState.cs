using System;
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
	public static int unlocked_levels = 1;
	public static int start_level = 1;
	public static GameState instance;
	PiggyPreview piggyPreview;
	PiggyCameraPivot piggyCameraPivot;
	public GameObject cameraAnimationPrefab;
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

	public Level CurrentLevel { get
		{
			Level level = GameObject.Find($"Level{current_level_num}").GetComponent<Level>();
			Debug.Assert(level != null);
			
			return level;
		} }
	public GridMatrix CurrentGridMatrix
	{
		get
		{
			GridMatrix gridMatrix = CurrentLevel.transform.Find("GridMatrix").GetComponent<GridMatrix>();
			Debug.Assert(gridMatrix != null);
			// ToastManager.Toast($"Current Grid Matrix: {current_level_num}");
			return gridMatrix;
		}
	}
	
	private void Awake()
	{
		//if (instance == null)
		//{
		//	instance = this;
		//	DontDestroyOnLoad(gameObject);
		//}
		//else
		//{
		//	Debug.LogError("Duplicate gamestate detected");
		//	Destroy(gameObject); // Prevent duplicates
		//}
		piggyCameraPivot = GameObject.Find("PiggyCameraPivot").GetComponent<PiggyCameraPivot>();
		Debug.Assert(piggyCameraPivot != null);
		piggyCameraEndTransform = piggyCameraPivot.transform.GetChild(0);
		cameraRefTransform = piggyCameraPivot.transform;
		EventBus.Subscribe<PiggyInstantiatedEvent>(OnPiggyInstantiated);
		EventBus.Subscribe<GameStateChangedEvent>(OnGameStateChanged);
		EventBus.Subscribe<FirstPersonChangedEvent>(OnFirstPersonChanged);
		EventBus.Subscribe<RetryEvent>(OnRetry);
		EventBus.Subscribe<NextEvent>(OnNext);
		EventBus.Subscribe<AnimationExitEvent>(OnAnimationExit);
	}
	private void OnEnable()
	{
		InitializeGridMatrix(start_level);
		StartCoroutine(EnableHelper());
	}
	private void Start()
	{
	}
	IEnumerator EnableHelper()
	{
		yield return null;
		EventBus.Publish(new GameStateChangedEvent(Util.GameStateType.Intro, start_level));
	}
	void InitializeGridMatrix(int next_level)
	{
		for (int level_num = 1; level_num <= Util.LevelItems.Count; level_num++)
		{
			current_level_num = level_num;
			CurrentGridMatrix.enabled = false;
		}
		current_level_num = next_level;
	}
	void OnNext(NextEvent e)
	{
		if (current_level_num >= Util.LevelItems.Count) // count starting from 1
		{
			Debug.Log($"Current level num: {current_level_num}, Util.levelItems.count: {Util.LevelItems.Count}");
			Debug.LogError("No more level to play");
			return;
		}
		current_level_num++;
		EventBus.Publish(new GameStateChangedEvent(Util.GameStateType.Intro, current_level_num));
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
		yield return null;
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
			Camera.main.transform.position = Vector3.Lerp(startPosition, dummyCameraTransform.position, progress);
			Camera.main.transform.rotation = Quaternion.Slerp(startRotation, dummyCameraTransform.rotation, progress);
			Debug.Log($"camera main: {Camera.main.transform.position.x}, {Camera.main.transform.position.y},{Camera.main.transform.position.z}");
			yield return null;
		}
		cameraTransform.position = dummyCameraTransform.position;
		cameraTransform.rotation = dummyCameraTransform.rotation;
		Debug.Log($"dummy camera transform position: {dummyCameraTransform.position.x}, {dummyCameraTransform.position.y}, {dummyCameraTransform.position.z}");
		// grid matrix will be enabled at transition to build
		Debug.Log("Move camera to grid called");
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
		Dictionary<int, KeyCode> keycodes = new() { { 1, KeyCode.Alpha1 }, { 2, KeyCode.Alpha2 },
			{ 3, KeyCode.Alpha3 }, { 4, KeyCode.Alpha4 }, { 5, KeyCode.Alpha5 }, { 6, KeyCode.Alpha6 }, 
			{ 7, KeyCode.Alpha7 }, { 8, KeyCode.Alpha8 }, { 9, KeyCode.Alpha9 } };

		foreach (var pair in keycodes)
		{
			if (Input.GetKeyDown(pair.Value))
			{
				EventBus.Publish(new GameStateChangedEvent(Util.GameStateType.Intro, pair.Key));
			}
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
	//IEnumerator TransitionToIntroHelper()
	//{
	//	yield return null;
	//	Transform dummyCameraTransform = CurrentLevel.transform.Find("GridMatrix").Find("DummyCamera");
	//	Camera.main.transform.position = dummyCameraTransform.position;
	//	Camera.main.transform.rotation = dummyCameraTransform.rotation;
	//}
	void TransitionToIntro(int level_num)
	{
		InitializeGridMatrix(level_num);
		current_level_num = level_num;
		piggyCameraPivot.EndFollow();
		piggyCameraPivot.dragEulerAngle = Vector3.zero;
		ToastManager.Toast($"Intro level {level_num}");

		StartCoroutine(PlayAnimation());
		// StartCoroutine(MoveCameraToGrid());
		

		// StartCoroutine(TransitionToIntroHelper());
		// Invoke(nameof(GoToBuild), 1.0f);
	}
	IEnumerator PlayAnimation()
	{
		GameObject cameraAnim = Instantiate(cameraAnimationPrefab, Vector3.zero, Quaternion.identity);
		Animator animator = cameraAnim.GetComponent<Animator>();
		yield return null;
		Camera.main.transform.parent = animator.transform;
		yield return null;
		animator.Rebind();
		yield return null;
		animator.SetTrigger($"level{current_level_num}");
	}

	void OnAnimationExit(AnimationExitEvent e)
	{
		Debug.Log("Animation exit called");
		// Camera.main.transform.parent = null;
		// cameraAnimator.Rebind();
		StartCoroutine(MoveCameraToGrid());
	}
	//void GoToBuild()
	//{
	//	ToastManager.Toast("Gone to build");
	//	EventBus.Publish(new GameStateChangedEvent(Util.GameStateType.Build, current_level_num));
	//}
	void TransitionToBuild()
	{
		ToastManager.Toast("Tansition to build called");
		piggy_permit_invisible = false;
		piggyCameraPivot.EndFollow();
		piggyCameraPivot.dragEulerAngle = Vector3.zero;
		GridMatrix gridMatrix = CurrentGridMatrix;
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
		if (unlocked_levels < current_level_num + 1 && current_level_num < Util.LevelItems.Count)
		{
			unlocked_levels = current_level_num + 1;
		}
		StartCoroutine(RiseAndWatch());
	}
	IEnumerator RiseAndWatch()
	{
		float start_time = Time.time;
		float end_time = start_time + rise_time;
		Transform cameraTransform = Camera.main.transform;
		Vector3 initialPos = cameraTransform.position;
		Vector3 goalPos = cameraTransform.position + new Vector3(0, rise_height, 0);
		while(Time.time < end_time && piggyPreview != null)
		{
			cameraTransform.position = Vector3.Lerp(initialPos, goalPos, (Time.time - start_time) / rise_time);
			cameraTransform.LookAt(piggyPreview.transform);
			yield return null;
		}
		while(piggy_permit_invisible && piggyPreview != null)
		{
			cameraTransform.LookAt(piggyPreview.transform);
			yield return null;
		}
	}
}
