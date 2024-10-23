using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToGridEvent
{
	
}

public class InvisibleChangedEvent
{
	public bool invisible = false;
    public InvisibleChangedEvent(bool invisible)
    {
        this.invisible = invisible;
    }
}
public class PiggyInstantiatedEvent
{
	public PiggyPreview piggyPreview;
	
    public PiggyInstantiatedEvent(PiggyPreview piggyPreview)
    {
        this.piggyPreview = piggyPreview;
    }
}
//public class GameStateChangedEvent
//{
//	public Util.GameStateType state;
//	public int level_num;
//    public GameStateChangedEvent(Util.GameStateType state, int level_num)
//    {
//		this.state = state;
//		this.level_num = level_num;
//    }
//}
public class FirstPersonChangedEvent
{
	public bool first_person;
    public FirstPersonChangedEvent(bool first_person)
    {
		this.first_person = first_person;
    }
}

public class ShowTutorialEvent
{
	public Util.TutorialType tutorialType;
    public ShowTutorialEvent(Util.TutorialType tutorialType)
    {
        this.tutorialType = tutorialType;
    }
}

public class GameState : MonoBehaviour
{
	public static int unlocked_levels = 1;
	public static int start_level = 1;
	public static bool shown_layers = false;
	public static bool shown_trashcan = false;
	public static bool shown_drag_images = false;
	public static bool placed_a_component = false;
	public static bool shown_third_person = false;
	public static bool shown_eraser = false;
	public static bool shown_confirm = false;
	public static bool shown_menu = false;
	public static bool shown_view = false;
	public static bool shown_help = false;
	public static bool shown_retry = false;
	public static bool shown_drag_screen = false;
	public static List<bool> shown_tutorials = new() { false, false, false, false, false };
	PiggyPreview piggyPreview;
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

	static GameState inst;
	public static GameState Inst
	{
		get { Debug.Assert(inst != null, "Game State not set"); return inst; }
	}
	

	public Level CurrentLevel { get
		{
			Level level = GameObject.Find($"Level{current_level_num}").GetComponent<Level>();
			Debug.Assert(level != null);
			
			return level;
		} }

	void OnMoveToGrid(MoveToGridEvent e)
	{
		StartCoroutine(MoveCameraToGrid(false));
	}
	private void Start()
	{
		Debug.Assert(inst == null, "Game State already instantiated");
		inst = this;
		piggyCameraEndTransform = PiggyCameraPivot.Inst.transform.GetChild(0);
		cameraRefTransform = PiggyCameraPivot.Inst.transform;
		EventBus.Subscribe<PiggyInstantiatedEvent>(OnPiggyInstantiated);
		EventBus.Subscribe<FirstPersonChangedEvent>(OnFirstPersonChanged);
		EventBus.Subscribe<RetryEvent>(OnRetry);
		EventBus.Subscribe<NextEvent>(OnNext);
		EventBus.Subscribe<AnimationExitEvent>(OnAnimationExit);
		EventBus.Subscribe<MoveToGridEvent>(OnMoveToGrid);
		// work?
	}


	private void OnEnable()
	{
		StartCoroutine(EnableHelper());
	}

	IEnumerator EnableHelper()
	{
		yield return null;
		TransitionToIntro();
	}
	void OnNext(NextEvent e)
	{
		if (current_level_num >= Util.LevelItems.Count) // count starting from 1
		{
			// Debug.Log($"Current level num: {current_level_num}, Util.levelItems.count: {Util.LevelItems.Count}");
			// Debug.LogError("No more level to play");
			ToastManager.Toast("More levels coming soon!\nThanks for playing!");
			return;
		}
		current_level_num++;
		TransitionToIntro();
	}
	void OnRetry(RetryEvent e)
	{
		PiggyCameraPivot.Inst.EndFollow();
		camera_follow_pig = false;
		piggy_permit_invisible = false;
		StartCoroutine(MoveCameraToGrid());
	}
	IEnumerator MoveCameraToGrid(bool publishevent=true)
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
			// Debug.Log($"camera main: {Camera.main.transform.position.x}, {Camera.main.transform.position.y},{Camera.main.transform.position.z}");
			yield return null;
		}
		cameraTransform.position = dummyCameraTransform.position;
		cameraTransform.rotation = dummyCameraTransform.rotation;
		Debug.Log($"dummy camera transform position: {dummyCameraTransform.position.x}, {dummyCameraTransform.position.y}, {dummyCameraTransform.position.z}");
		// grid matrix will be enabled at transition to build
		Debug.Log("Move camera to grid called");
		if (publishevent)
		{
			TransitionToBuild();
		}
	}
	IEnumerator CameraToPig()
	{
		Debug.Assert(piggyPreview != null);
		float start_time = Time.time;
		float end_time = start_time + move_to_pig_time;
		Transform cameraTransform = Camera.main.transform;
		Vector3 cameraStartPos = cameraTransform.position;
		// force pivot to move to place immediately
		PiggyCameraPivot.Inst.transform.position = piggyPreview.transform.position;
		PiggyCameraPivot.Inst.transform.rotation = piggyPreview.transform.rotation;
		while (Time.time < end_time)
		{
			if ((Time.time - start_time) / move_to_pig_time > 0.8)
			{
				piggy_permit_invisible = true;
				if (first_person)
				{
					EventBus.Publish(new InvisibleChangedEvent(true));
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
			cameraRefTransform = PiggyCameraPivot.Inst.transform;
		}
		else
		{
			cameraRefTransform = piggyCameraEndTransform;
		}
		if (piggyPreview != null)
		{
			if (first_person)
			{
				EventBus.Publish(new InvisibleChangedEvent(true));
			}
			else if (piggy_permit_invisible)
			{
				EventBus.Publish(new InvisibleChangedEvent(false));
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
		//Dictionary<int, KeyCode> keycodes = new() { { 1, KeyCode.Alpha1 }, { 2, KeyCode.Alpha2 },
		//	{ 3, KeyCode.Alpha3 }, { 4, KeyCode.Alpha4 }, { 5, KeyCode.Alpha5 }, { 6, KeyCode.Alpha6 }, 
		//	{ 7, KeyCode.Alpha7 }, { 8, KeyCode.Alpha8 }, { 9, KeyCode.Alpha9 } };

		//foreach (var pair in keycodes)
		//{
		//	if (Input.GetKeyDown(pair.Value))
		//	{
		//		EventBus.Publish(new GameStateChangedEvent(Util.GameStateType.Intro, pair.Key));
		//	}
		//}
    }

	void TransitionToIntro()
	{
		CanvasSwitch.Inst.TransitionToIntro();
		AudioPlayer.Inst.TransitionToIntro();
		Goal.Select(current_level_num);
		PiggyCameraPivot.Inst.EndFollow();
		ToastManager.Toast($"Level {current_level_num}");

		StartCoroutine(PlayAnimation());
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
	public static IEnumerator ShowTutorial(Util.TutorialType tutorialType, float delay=1.5f)
	{
		yield return new WaitForSeconds(delay);
		EventBus.Publish(new ShowTutorialEvent(tutorialType));
	}

	IEnumerator ShowTutorial(int index)
	{
		if (shown_tutorials[index])
		{
			yield break;
		}
		shown_tutorials[index] = true;
		yield return null;
		EventBus.Publish(new ShowNewTutorialEvent((Util.NewTutorialType)index));
	}
	void TransitionToBuild()
	{
		CanvasSwitch.Inst.TransitionToBuild();
		AudioPlayer.Inst.TransitionToBuild();
		ImageDragHandler.Current = null;
		GridMatrix.SelectGridMatrix(current_level_num);
		StartCoroutine(ShowTutorial(current_level_num - 1));
		piggy_permit_invisible = false;
		PiggyCameraPivot.Inst.EndFollow();
		
	}
	public GameObject dragScreenCanvas;
	IEnumerator ShowDragScreen()
	{
		if (shown_drag_screen)
		{
			yield break;
		}
		shown_drag_screen = true;
		yield return new WaitForSeconds(1.0f);
		dragScreenCanvas.SetActive(true);
		yield return new WaitForSeconds(3.0f);
		dragScreenCanvas.SetActive(false);
	}
	public void TransitionToPlay()
	{
		CanvasSwitch.Inst.TransitionToPlay();
		AudioPlayer.Inst.TransitionToPlay();
		GridMatrix.Current.BuildAndDeactivate();
		GridMatrix.DeselectGridMatrix();
		PiggyCameraPivot.Inst.StartFollow(piggyPreview);
		// coroutine that moves camera to position
		StartCoroutine(CameraToPig());
		StartCoroutine(ShowDragScreen());
		// start updating camera position according to piggyCameraPivot
		//if (!shown_dragscreen_tutorial)
		//{
		//	shown_dragscreen_tutorial = true;
		//	StartCoroutine(ShowTutorial(Util.TutorialType.DragScreen, 4.0f));
		//}
		//if (current_level_num == 2 && !shown_third_person_tutorial)
		//{
		//	shown_third_person_tutorial = true;
		//	StartCoroutine(ShowTutorial(Util.TutorialType.ThirdPerson, 4.0f));
		//}
	}
	public void TransitionToOutro()
	{
		CanvasSwitch.Inst.TransitionToOutro();
		AudioPlayer.Inst.TransitionToOutro();
		Goal.Deselect();
		camera_follow_pig = false;
		EventBus.Publish(new InvisibleChangedEvent(false));
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
