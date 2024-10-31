using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class InvisibleStateUpdateEvent
{
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

public class GameState : MonoBehaviour
{
	public static int unlocked_levels = 1;
	public static int start_level = 1;

	public static Dictionary<string, bool> button_clicked = new()
	{
		{"layers", false},
		{"trashcan", false },
		{"drag_images", false },
		{"placed_a_component", false },
		{"third_person", false },
		{"eraser", false },
		{"confirm", false },
		{"menu", false },
		{"view", false },
		{"help", false },
		{"retry", false }
	};
	public static bool shown_layers = false;
	public static bool shown_trashcan = false;
	public static bool shown_drag_images = false;
	public static bool shown_third_person = false;
	public static bool shown_eraser = false;
	public static bool shown_confirm = false;
	public static bool shown_menu = false;
	public static bool shown_view = false;
	public static bool shown_help = false;
	public static bool shown_retry = false;
	public static bool drag_screen_shown = false;
	public static List<bool> shown_tutorials = new() { false, false, false, false, false };

	public static Dictionary<Util.Component, int> Inventory { get; set; } = new()
	{
		{Util.Component.Pig, 1 },
		{Util.Component.WoodenCrate, 6 },
		{Util.Component.Wheel, 4 },
		{Util.Component.TurnWheel, 0 },
		{Util.Component.MotorWheel, 0 }
	};

	public GameObject cameraAnimationPrefab;
	bool camera_follow_pig = false;
	public int current_level_num = 1;
	public float move_to_pig_time = 0.5f;
	public float camera_rotation_time = 0.5f;
	public float retry_move_time = 0.5f;
	public float rise_height = 10.0f;
	public float rise_time = 2.0f;

	Util.WaypointName retry_waypoint = Util.WaypointName.None;
	Util.GoalName retry_goal = Util.GoalName.None;
	public bool FirstPerson
	{
		get { return first_person; }
		set 
		{ 
			first_person = value;
			EventBus.Publish(new InvisibleStateUpdateEvent());
			PiggyCameraPivot.Inst.OnFirstPersonChanged(value);
		}
	}
	private bool first_person = true;
	public bool PiggyPermitInvisible { get; set; } = false;
	public List<VehicleComponent> Components { get; set; } = new();

	public PiggyPreview Piggy { get; set; }

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

	//public void GoBackToBuild()
	//{
	//	StartCoroutine(MoveCameraToGrid(false));
	//}
	private void Start()
	{
		// temporary shut down
		Debug.Assert(inst == null, "Game State already instantiated");
		inst = this;
		Util.Delay(this, () =>
		{
			TransitionToStory(Util.StoryName.Crash);
		});
		EventBus.Subscribe<GoalReachedEvent>(OnGoalReached);
	}
	private void OnDestroy()
	{
		inst = null;
	}
	//void OnNext(NextEvent e)
	//{
	//	if (current_level_num >= Util.WaypointItems.Count) // count starting from 1
	//	{
	//		ToastManager.Toast("More levels coming soon!\nThanks for playing!");
	//		return;
	//	}
	//	current_level_num++;
	//	TransitionToIntro();
	//}
	public void Retry()
	{
		PiggyCameraPivot.Inst.EndFollow();
		camera_follow_pig = false;
		PiggyPermitInvisible = false;
		TransitionToBuild(retry_waypoint, retry_goal);
	}
	IEnumerator MoveCameraToGrid()
	{
		yield return null;
		float startTime = Time.time;
		float endTime = startTime + retry_move_time;
		Transform cameraTransform = Camera.main.transform;
		Vector3 startPosition = cameraTransform.position;
		Quaternion startRotation = cameraTransform.rotation;
		Transform dummyCameraTransform = GridMatrix.Current.DummyCamera;
		Debug.Assert(dummyCameraTransform != null, "dummy camera transform is null");
		while (Time.time < endTime)
		{
			float progress = (Time.time - startTime) / retry_move_time;
			Camera.main.transform.position = Vector3.Lerp(startPosition, dummyCameraTransform.position, progress);
			Camera.main.transform.rotation = Quaternion.Slerp(startRotation, dummyCameraTransform.rotation, progress);
			yield return null;
		}
		cameraTransform.position = dummyCameraTransform.position;
		cameraTransform.rotation = dummyCameraTransform.rotation;
	}
	private void Update()
	{
		// CheatCode();
	}
	//void CheatCode()
	//{
	//	if (camera_follow_pig)
	//	{
	//		Camera.main.transform.position = cameraRefTransform.position;
	//		Camera.main.transform.rotation = cameraRefTransform.rotation;
	//	}
	//	Dictionary<int, KeyCode> keycodes = new() { { 1, KeyCode.Alpha1 }, { 2, KeyCode.Alpha2 },
	//		{ 3, KeyCode.Alpha3 }, { 4, KeyCode.Alpha4 }, { 5, KeyCode.Alpha5 }, { 6, KeyCode.Alpha6 },
	//		{ 7, KeyCode.Alpha7 }, { 8, KeyCode.Alpha8 }, { 9, KeyCode.Alpha9 } };

	//	foreach (var pair in keycodes)
	//	{
	//		if ( Input.GetKey(KeyCode.LeftShift)&& Input.GetKeyDown(pair.Value))
	//		{
	//			current_level_num = pair.Key;
	//			TransitionToIntro();
	//		}
	//	}
	//}
	
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

	//void OnAnimationExit(AnimationExitEvent e)
	//{
	//	StartCoroutine(MoveCameraToGrid());
	//}
	//void GoToBuild()
	//{
	//	ToastManager.Toast("Gone to build");
	//	EventBus.Publish(new GameStateChangedEvent(Util.GameStateType.Build, current_level_num));
	//}
	IEnumerator ShowTutorial(int index)
	{
		yield return null;
		EventBus.Publish(new ShowNewTutorialEvent((Util.NewTutorialType)index));
		yield break;
	}

	void DestroyComponentsInScene()
	{
		foreach(var component in Components)
		{
			Destroy(component.gameObject);
		}
		Components.Clear();
		Piggy = null;
	}
	public void TransitionToStory(Util.StoryName story_name)
	{
		Goal.Deselect();
		PlayCanvas.Inst.Hide();
		PiggyCameraPivot.Inst.EndFollow();
		PiggyPermitInvisible = false;		
		EventBus.Publish(new InvisibleStateUpdateEvent());
		switch (story_name)
		{
			case Util.StoryName.Crash:
				TransitionToStoryCrash();
				break;
			case Util.StoryName.Intro:
				TransitionToStoryIntro();
				break;
			case Util.StoryName.InTown:
				TransitionToStoryInTown();
				break;
		}
	}
	void TransitionToStoryCrash()
	{
		BlackoutCanvas.Inst.Blackout(1.0f, 1.0f, () =>
		{
			Debug.Log("Story!");
			TransitionToBuild(Util.WaypointName.PreStory1, Util.GoalName.PreStory1);
		});

		// animation end 
	}
	void TransitionToStoryIntro()
	{
		MainCamera.Inst.Stop();
		BlackoutCanvas.Inst.Blackout(2.0f, 1.0f, () =>
		{
			TransitionToBuild(Util.WaypointName.PreStory2, Util.GoalName.PreStory2);
		});
	}
	void TransitionToStoryInTown()
	{
		MainCamera.Inst.Stop();
		BlackoutCanvas.Inst.Blackout(3.0f, 3.0f, () =>
		{
			TransitionToBuild(Util.WaypointName.PreStory1, Util.GoalName.PreStory1);
		});
	}
	//public void TransitionToIntro()
	//{
	//	DestroyComponentsInScene();
	//	PiggyCameraPivot.Inst.EndFollow();
	//	ToastManager.Toast($"Level {current_level_num}");

	//	StartCoroutine(PlayAnimation());
	//}
	static HashSet<Util.WaypointName> can_retry_waypoints = new()
	{
		Util.WaypointName.PreStory1,
		Util.WaypointName.PreStory2
	};
	void TransitionToBuild(Util.WaypointName waypoint_name, Util.GoalName goal_name)
	{
		if (can_retry_waypoints.Contains(waypoint_name))
		{
			retry_waypoint = waypoint_name;
			retry_goal = goal_name;
		}
		else
		{
			waypoint_name = Util.WaypointName.None;
		}
		BuildCanvas.Inst.Show();
		PlayCanvas.Inst.Hide();
		// AudioPlayer.Inst.TransitionToBuild();
		DestroyComponentsInScene();		
		DragImage.Current = null;
		GridMatrix.SelectGridMatrix(waypoint_name);
		if (goal_name != Util.GoalName.None)
		{
			Goal.Select(goal_name);
		}
		MainCamera.Inst.MoveAndStickToGridMatrix(0.5f, 0.5f, 0.5f);
		PiggyPermitInvisible = false;
		PiggyCameraPivot.Inst.EndFollow();
	}
	void OnGoalReached(GoalReachedEvent e)
	{
		switch (e.goal_name)
		{
			case Util.GoalName.PreStory1:
				TransitionToStory(Util.StoryName.Intro);
				break;
			case Util.GoalName.PreStory2:
				TransitionToStory(Util.StoryName.InTown);
				break;
		}
	}
	public void TransitionToPlay()
	{
		BuildCanvas.Inst.Hide();
		PlayCanvas.Inst.Show();
		AudioPlayer.Inst.TransitionToPlay();
		GridMatrix.Current.BuildAndDeactivate();
		GridMatrix.DeselectGridMatrix();
		PiggyCameraPivot.Inst.StartFollow(Piggy);
		// coroutine that moves camera to position
		MainCamera.Inst.MoveAndStickToPig(move_to_pig_time, camera_rotation_time);
	}
}
