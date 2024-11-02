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
public class WASDPressedEvent { }
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
			// TransitionToStory(Util.StoryName.Crash);
			//TransitionToBuild(Util.WaypointName.PreStory1, Util.GoalName.PreStory1);
			TransitionToBuild(Util.WaypointName.PreStory2, Util.GoalName.PreStory2);
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
	public void OnRetry()
	{
		PiggyCameraPivot.Inst.EndFollow();
		camera_follow_pig = false;
		PiggyPermitInvisible = false;
		TransitionToBuild(retry_waypoint, retry_goal);
	}
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A)
			|| Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D))
		{
			EventBus.Publish(new WASDPressedEvent());
		}
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
		if (story_name != Util.StoryName.Intro && story_name!= Util.StoryName.FallOffCliff
			&& story_name != Util.StoryName.InTown)
		{
			DestroyComponentsInScene();
		}
		switch (story_name)
		{
			case Util.StoryName.Crash:
				TransitionToStoryCrash();
				break;
			case Util.StoryName.Intro:
				StartCoroutine(TransitionToStoryIntro());
				break;
			case Util.StoryName.FallOffCliff:
				StartCoroutine(TransitionToStoryCliff());
				break;
			case Util.StoryName.InTown:
				TransitionToStoryInTown();
				break;
		}
	}
	void TransitionToStoryCrash()
	{
		//BlackoutCanvas.Inst.Blackout(1.0f, 1.0f, () =>
		//{
		//	Debug.Log("Story!");
		//	TransitionToBuild(Util.WaypointName.PreStory1, Util.GoalName.PreStory1);
		//});
		StoryAnimation.Inst.PlayAnimation(Util.StoryName.Crash);
		StoryAnimation.Inst.RegisterEndAnimationFunc(() =>
		{
			TransitionToBuild(Util.WaypointName.PreStory1, Util.GoalName.PreStory1);
		});
		// animation end 
	}
	public float rise_time = 5.0f;
	IEnumerator TransitionToStoryIntro()
	{
		Goal.Select(Util.GoalName.FallOffCliff);
		MainCamera.Inst.Stop();
		PiggyPermitInvisible = false;
		EventBus.Publish(new InvisibleStateUpdateEvent());
		float start_time = Time.time;
		Vector3 initial_position = MainCamera.Inst.transform.position;
		Quaternion initial_rotation = MainCamera.Inst.transform.rotation;
		Transform introCameraTransform = IntroCamera.Inst.transform;
		while(Time.time - start_time < rise_time)
		{
			MainCamera.Inst.transform.position = Vector3.Lerp(initial_position, introCameraTransform.position, (Time.time - start_time) / rise_time);
			Quaternion target_rotation = Quaternion.Slerp(initial_rotation, introCameraTransform.rotation, (Time.time - start_time) / rise_time);
			Vector3 look_dir = Piggy.transform.position - MainCamera.Inst.transform.position;
			Quaternion lookat_rotation = Quaternion.LookRotation(look_dir);
			MainCamera.Inst.transform.rotation = Quaternion.Slerp(lookat_rotation, target_rotation, (Time.time - start_time) / rise_time);
			yield return null;
		}
		MainCamera.Inst.transform.position = introCameraTransform.position;
		MainCamera.Inst.transform.rotation = introCameraTransform.rotation;
		IntroCanvas.Inst.Play();
	}
	public float shake_duration = 2.0f;
	public float shake_intensity = 1.0f;
	IEnumerator TransitionToStoryCliff()
	{
		MainCamera.Inst.Stop();
		Debug.Log("Falling off cliff!");
		AudioPlayer.Inst.Wilhelm();
		yield return new WaitForSeconds(1.5f);
		Vector3 original_position = MainCamera.Inst.transform.position;
		float elapsedTime = 0f;
		while (elapsedTime < shake_duration)
		{
			// Calculate vibration offset using Perlin noise
			float x = (Mathf.PerlinNoise(Time.time * 10, 0) - 0.5f) * 2 * shake_intensity;
			float y = (Mathf.PerlinNoise(0, Time.time * 10) - 0.5f) * 2 * shake_intensity;

			// Apply vibration offset to the original position
			MainCamera.Inst.transform.position = original_position + new Vector3(x, y, 0);

			elapsedTime += Time.deltaTime;
			yield return null; // Wait for the next frame
		}
		// Reset to the original position
		MainCamera.Inst.transform.position = original_position;
		yield return new WaitForSeconds(2.0f);
		StoryAnimation.Inst.PlayAnimation(Util.StoryName.FallOffCliff);
		StoryAnimation.Inst.RegisterEndAnimationFunc(() =>
		{
			TransitionToBuild(Util.WaypointName.PreStory2, Util.GoalName.PreStory2);
		});
	}
	void TransitionToStoryInTown()
	{
		MainCamera.Inst.Stop();
		StoryAnimation.Inst.CanSpeedup = false;
		StoryAnimation.Inst.PlayAnimation(Util.StoryName.InTown);
		MainCamera.Inst.FollowStory();
		StoryAnimation.Inst.RegisterEndAnimationFunc(() =>
		{
			TransitionToPlay(false);
			StoryAnimation.Inst.CanSpeedup = true;
		});
		//BlackoutCanvas.Inst.Blackout(3.0f, 3.0f, () =>
		//{
		//	TransitionToBuild(Util.WaypointName.PreStory1, Util.GoalName.PreStory1);
		//});
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
		current_waypoint = waypoint_name;
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

		switch(waypoint_name)
		{
			case Util.WaypointName.PreStory1:
				StartCoroutine(Prestory1Build());
				break;
			case Util.WaypointName.PreStory2:
				StartCoroutine(Prestory2Build());
				break;
		}
	}
	IEnumerator ShowLineAndContinue(LineCanvas line_canvas, string name, string line, float time)
	{
		line_canvas.Name = name;
		line_canvas.Line = line;
		yield return new WaitForSeconds(time);
		line_canvas.ShowContinue();
		while (!Input.GetMouseButtonDown(0))
		{
			yield return null;
		}
		line_canvas.HideContinue();
	}
	IEnumerator ShowLine(LineCanvas line_canvas, string name, string line, float time)
	{
		line_canvas.Name = name;
		line_canvas.Line = line;
		yield return new WaitForSeconds(time);
	}

	IEnumerator ShowLineAndListenForEvent<Event_>(LineCanvas line_canvas, string name, string line, Func<Event_, bool> handler)
	{
		if (name != null || line != null)
		{
			line_canvas.Name = name;
			line_canvas.Line = line;
		}
		bool criteria_met = false;
		var subscription = EventBus.Subscribe<Event_>((Event_ e) =>
		{
			if (handler(e))
			{
				criteria_met = true;
			}
		});
		while (!criteria_met)
		{
			yield return null;
		}
		EventBus.Unsubscribe(subscription);
	}
	IEnumerator Prestory1Build()
	{
		yield return new WaitForSeconds(1.5f);
		Trash.Inst.gameObject.SetActive(false);
		yield return ShowLineAndContinue(LineCanvas.Top, "Shirley", "Welcome to the grid building system!", 0.2f);
		yield return ShowLineAndContinue(LineCanvas.Top, "Shirley", "Oh! There is a mess! Let's clean it up using the eraser!", 0.2f);
		LineCanvas.Top.Line = "Oh! There is a mess! Let's clean it up using the eraser!";
		LineCanvas.Top.Line = "Start by clicking the eraser.";
		EraserImage.Inst.StartScale();
		yield return ShowLineAndListenForEvent(LineCanvas.Top, "Shirley", "Start by clicking the eraser.", (ToolClickedEvent e) =>
		{
			return e.cursor_mode == Util.CursorMode.Erase;
		});
		EraserImage.Inst.EndScale();
		yield return ShowLineAndListenForEvent(LineCanvas.Top, "Shirley", "Good! Hover your mouse on the grid and click to erase a component.",
			(ItemErasedEvent e) => true);
		Trash.Inst.gameObject.SetActive(true);
		Trash.Inst.StartScale();
		yield return ShowLineAndListenForEvent(LineCanvas.Top, "Shirley", "Perfect! Now let's use the trashcan to remove all the components at once!",
			(ResetCountEvent e)=>true);
		Trash.Inst.EndScale();
		yield return ShowLineAndContinue(LineCanvas.Top, "Shirley", "Perfect! Now we have a clear space to build our vehicle!", 0.2f);
		GridMatrix.Current.ShowDesign();
		yield return ShowLineAndContinue(LineCanvas.Top, "Shirley", "For now, let's adhere to a standard vehicle design", 0.2f);
		DragImage.StartScaleAll();
		yield return ShowLineAndListenForEvent(LineCanvas.Top, "Shirley", "Start by clicking on a component icon.",
			(DragImageClickedEvent e) => true);
		DragImage.EndScaleAll();
		yield return ShowLineAndListenForEvent(LineCanvas.Top, "Shirley", "Perfect! Hover your mouse on the grid and click to place a component.",
			(ComponentAddedEvent e)=>true);
		yield return ShowLineAndContinue(LineCanvas.Top, "Shirley", "Perfect! Let's place the rest of the components.", 0.2f);
		yield return ShowLineAndListenForEvent(LineCanvas.Top, null, null, (ReadyToGoEvent e) => true);
		yield return ShowLineAndListenForEvent(LineCanvas.Top, "Shirley", "You are learning fast! Now click the confirm button to start our journey!",
			(ConfirmSuccessEvent e) => true);
		LineCanvas.Top.Hide();
	}

	public IEnumerator Prestory2Build()
	{
		StoryAnimation.Inst.CanSpeedup = false;
		yield return new WaitForSeconds(1.5f);
		yield return ShowLineAndContinue(LineCanvas.Top, "Shirley", "This time, let's build a wider vehicle with a sophisticated control system.", 0.5f);
		LineCanvas.Top.Hide();
		yield return ShowLineAndListenForEvent(LineCanvas.Top, null, null, (ReadyToGoEvent e) => true);
		yield return ShowLineAndListenForEvent(LineCanvas.Top, "Shirley", "Let's roll!",
			(ConfirmSuccessEvent e) => true);
		LineCanvas.Top.Hide();
	}
	void OnGoalReached(GoalReachedEvent e)
	{
		switch (e.goal_name)
		{
			case Util.GoalName.PreStory1:
				TransitionToStory(Util.StoryName.Intro);
				break;
			case Util.GoalName.FallOffCliff:
				TransitionToStory(Util.StoryName.FallOffCliff);
				break;
			case Util.GoalName.PreStory2:
				TransitionToStory(Util.StoryName.InTown);
				break;
		}
	}
	Util.WaypointName current_waypoint;
	public void TransitionToPlay(bool build)
	{
		BuildCanvas.Inst.Hide();
		PlayCanvas.Inst.Show();
		// AudioPlayer.Inst.TransitionToPlay();
		if (build)
		{
			GridMatrix.Current.BuildAndDeactivate();
			GridMatrix.DeselectGridMatrix();
		}
		PiggyCameraPivot.Inst.StartFollow(Piggy);
		// coroutine that moves camera to position
		MainCamera.Inst.MoveAndStickToPig(move_to_pig_time, camera_rotation_time);
		if (build)
		{
			switch (current_waypoint)
			{
				case Util.WaypointName.PreStory2:
					StartCoroutine(PlayPreStory2());
					break;
			}
		}
	}
	public IEnumerator PlayPreStory2()
	{
		yield return new WaitForSeconds(1.0f);
		Retry.Inst.Show();
		// claustrophobia
		yield return ShowLine(LineCanvas.Bottom, "Shirley", "Woohoo! We're rolling!", 1.5f);
		yield return ShowLine(LineCanvas.Bottom, "Shirley", "Hope you don't have claustrophobia in your little crate.", 3.5f);
		yield return ShowLine(LineCanvas.Bottom, "Shirley", "Actually, you may feel more comfortable if you can look at me. I'm on your left.", 3.5f);
		yield return ShowLineAndListenForEvent(LineCanvas.Bottom, "Shirley", "**Drag the screen to look around**", (PlayCanvasDraggedEvent e) => true);
		yield return ShowLine(LineCanvas.Bottom, "Shirley", "Perfect! Now let's start the car.", 2.5f);
		yield return ShowLineAndListenForEvent(LineCanvas.Bottom, "Shirley", "**Press W/S to move and A/D to turn.", (WASDPressedEvent e) => true);
		yield return ShowLine(LineCanvas.Bottom, "Shirley", "You're learning fast! Let's see if you can make to the destination.", 3.5f);
		LineCanvas.Bottom.Hide();
	}
}
