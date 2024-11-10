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
		{Util.Component.WoodenCrate, 9 },
		{Util.Component.Wheel, 4 },
		{Util.Component.TurnWheel, 4 },
		{Util.Component.MotorWheel, 4 },
		{Util.Component.Rocket, 8 },
		{Util.Component.Umbrella, 8 }
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
	public bool IsFirstPerson
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
	void Init()
	{
		// TransitionToStory(Util.StoryName.Crash);
		// TransitionToBuild(Util.WaypointName.PreStory1, Util.GoalName.PreStory1);
		// TransitionToStory(Util.StoryName.FallOffCliff);
		// TransitionToBuild(Util.WaypointName.PreStory2, Util.GoalName.PreStory2);
		//  TransitionToBuild(Util.WaypointName.None, Util.GoalName.PreStory2);
		TransitionToStory(Util.StoryName.TownWaypoint);
		// town_waypoint_met = true;
		// TransitionToBuild(Util.WaypointName.Town, Util.GoalName.None);

		FirstPerson.Inst.Show();
		Retry.Inst.Show();
	}
	private void Start()
	{
		// temporary shut down
		Debug.Assert(inst == null, "Game State already instantiated");
		inst = this;
		Util.Delay(this, () =>
		{
			Init();
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
		Util.BuildInfo build_info = last_choice_name == Util.ChoiceName.NeedHelp ? Util.BuildInfo.NeedHelp : Util.BuildInfo.DontNeedHelpButRetry;
		TransitionToBuild(retry_waypoint, retry_goal, build_info);
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
	
	void DampStart()
	{
		foreach (var component in Components)
		{
			component.DampStart();
		}
	}
	void DampStop()
	{
		foreach (var component in Components)
		{
			component.DampStop();
		}
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
		MainCamera.Inst.Stop();
		EventBus.Publish(new InvisibleStateUpdateEvent());
		if (story_name != Util.StoryName.Intro && story_name!= Util.StoryName.FallOffCliff
			&& story_name != Util.StoryName.InTown)
		{
			DestroyComponentsInScene();
		}
		switch (story_name)
		{
			case Util.StoryName.Crash:
				StartCoroutine(TransitionToStoryCrash());
				break;
			case Util.StoryName.Intro:
				StartCoroutine(TransitionToStoryIntro());
				break;
			case Util.StoryName.FallOffCliff:
				StartCoroutine(TransitionToStoryCliff());
				break;
			case Util.StoryName.InTown:
				StartCoroutine(TransitionToStoryInTown());
				break;
			case Util.StoryName.TownWaypoint:
				StartCoroutine(TransitionToStoryTownWaypoint());
				break;
		}
	}
	IEnumerator WaitForClick()
	{
		while (!Input.GetMouseButtonDown(0)) {
			yield return null;
		}
	}
	IEnumerator TransitionToStoryTownWaypoint()
	{
		// yield return BlackoutCanvas.Inst.Blackout(1.0f, true);
		// yield return null;
		// yield return LineCanvas.Bottom.DisplayLine("Shirley","Arrived!");
		Character.Partner.WarpTo(TRef.Get(Util.TRefName.PartnerTownW));
		yield return MainCamera.Inst.WarpTo(TRef.Get(Util.TRefName.CameraTownW1), 1.0f);
		yield return LineCanvas.Bottom.DisplayLineAndWaitForClick("Shirley", "Congratulations! You found the Waypoint of the town.", Character.Partner);
		yield return LineCanvas.Bottom.DisplayLineAndWaitForClick("Shirley", "Waypoints are scattered across the world that enables you to rebuild your vehicle.", Character.Partner);
		yield return LineCanvas.Bottom.DisplayLineAndWaitForClick("Shirley", "The waypoint in the town opens for free to you, but you will have to complete challenging challenges to unlock some of them in the wild.", Character.Partner);
		yield return MainCamera.Inst.WarpTo(TRef.Get(Util.TRefName.CameraTownW2), 1.0f);
		yield return LineCanvas.Bottom.DisplayLineAndWaitForClick("Waypoint de New Sorpigal", "As long as you do not lose faith, the world will open to you.", null);
		Waypoint.Waypoints[Util.WaypointName.Town].ChangeToGreen();
		yield return WaitForClick();
		yield return MainCamera.Inst.WarpTo(TRef.Get(Util.TRefName.CameraTownW1), 1.0f);
		yield return LineCanvas.Bottom.DisplayLineAndWaitForClick("Shirley", "Let's try it out!", Character.Partner);
		LineCanvas.Bottom.Hide();
		Character.Partner.WarpTo(TRef.Get(Util.TRefName.Origin));
		TransitionToBuild(Util.WaypointName.Town, Util.GoalName.None);
	}
	IEnumerator TransitionToStoryCrash()
	{
		MainCamera.Inst.WarpTo(TRef.Get(Util.TRefName.CameraPrestory1_1));
		StoryAnimation.Inst.PlayAnimation(Util.StoryName.Crash);
		yield return BlackoutCanvas.Inst.Blackout(1.5f, 1.0f, 0.2f);
		yield return BlackoutCanvas.Inst.Blackout(1.5f, 0.2f, 1.0f);
		yield return BlackoutCanvas.Inst.Blackout(1.5f, 1.0f, 0.4f);
		yield return BlackoutCanvas.Inst.Blackout(1.5f, 0.4f, 1.0f);
		yield return new WaitForSeconds(1.0f);
		yield return BlackoutCanvas.Inst.DisplaySub("You were unconcious for some time", 1.0f, 0.0f, 1.0f);
		yield return new WaitForSeconds(1.0f);
		yield return BlackoutCanvas.Inst.DisplaySub(null, 1.0f, 1.0f, 0.0f);
		Character.Partner.WarpTo(TRef.Get(Util.TRefName.PartnerPrestory1));
		yield return new WaitForSeconds(1.0f);
		yield return BlackoutCanvas.Inst.Blackout(1.5f, 1.0f, 0.0f);
		yield return LineCanvas.Bottom.DisplayLineAndWaitForClick("???", "Are you all right?", Character.Partner);
		yield return LineCanvas.Bottom.DisplayLineAndWaitForClick("You", "Who... who are you?", null);
		yield return LineCanvas.Bottom.DisplayLineAndWaitForClick("Shirley", "I’m Shirley, Outrider for the New Sorpigal. Anything I can help?", Character.Partner);
		LineCanvas.Bottom.Hide();
		yield return BlackoutCanvas.Inst.Blackout(0.5f, 0.0f, 1.0f);
		yield return BlackoutCanvas.Inst.DisplaySub("You told the stranger everything just happened", 0.5f, 0.0f, 1.0f);
		yield return new WaitForSeconds(1.0f);
		yield return BlackoutCanvas.Inst.DisplaySub("You told the stranger everything just happened", 0.5f, 1.0f, 0.0f);
		yield return BlackoutCanvas.Inst.Blackout(0.5f, 1.0f, 0.0f);
		yield return LineCanvas.Bottom.DisplayLineAndWaitForClick("Shirley", "That sounds terrible! " +
			"Looks like you are injured. Let’s get down to the town to have a rest first.", Character.Partner);
		yield return LineCanvas.Bottom.DisplayLineAndWaitForClick("Shirley", "Maybe someone in the town knows where to look for your girlfriend.", Character.Partner);
		yield return AtTheSameTime(
			MainCamera.Inst.Transition(TRef.Get(Util.TRefName.CameraPrestory1_1), TRef.Get(Util.TRefName.CameraPrestory1_2), 2.0f),
			LineCanvas.Bottom.DisplayLineAndWaitForClick("Shirley", "There are some scattered parts nearby. Let's take advantage of them for a ride.", Character.Partner)
			);
		LineCanvas.Bottom.Hide();
		Character.Piggy.WarpTo(TRef.Get(Util.TRefName.Origin));
		Character.Partner.WarpTo(TRef.Get(Util.TRefName.Origin));
		TransitionToBuild(Util.WaypointName.PreStory1, Util.GoalName.PreStory1, Util.BuildInfo.NeedHelp);
		// Character.Piggy.WarpTo(TransformRef.Get(Util.TransformRefName.PigPrestory1));


		//BlackoutCanvas.Inst.Blackout(1.0f, 1.0f, () =>
		//{
		//	Debug.Log("Story!");
		//	TransitionToBuild(Util.WaypointName.PreStory1, Util.GoalName.PreStory1);
		//});

		//StoryAnimation.Inst.PlayAnimation(Util.StoryName.Crash);
		//StoryAnimation.Inst.RegisterEndAnimationFunc(() =>
		//{
		//	TransitionToBuild(Util.WaypointName.PreStory1, Util.GoalName.PreStory1);
		//});
		// animation end 
	}
	public float rise_time = 5.0f;

	public IEnumerator AtTheSameTime(IEnumerator task1, IEnumerator task2)
	{
		var coroutine = StartCoroutine(task1);
		yield return task2;
		yield return coroutine;
	}
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
	Util.ChoiceName last_choice_name = Util.ChoiceName.None;
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
		DestroyComponentsInScene();


		Character.Piggy.WarpTo(TRef.Get(Util.TRefName.PigPrestory2));
		Character.Partner.WarpTo(TRef.Get(Util.TRefName.PartnerPrestory2));
		MainCamera.Inst.WarpTo(TRef.Get(Util.TRefName.CameraPrestory2_1));
		yield return LineCanvas.Bottom.DisplayLineAndWaitForClick("Shirley", "Awww. That hurts!", Character.Partner);
		yield return MainCamera.Inst.Transition(TRef.Get(Util.TRefName.CameraPrestory2_1), TRef.Get(Util.TRefName.CameraPrestory2_2), 1.0f);
		yield return LineCanvas.Bottom.DisplayLineAndWaitForClick("You", "Yes.", Character.Piggy);
		yield return MainCamera.Inst.Transition(TRef.Get(Util.TRefName.CameraPrestory2_2), TRef.Get(Util.TRefName.CameraPrestory2_1), 1.0f);
		yield return LineCanvas.Bottom.DisplayLineAndWaitForClick("Shirley", "A car without control is like the West without Jerusalem.", Character.Partner);
		yield return AtTheSameTime(
			LineCanvas.Bottom.DisplayLineAndWaitForClick("Shirley", "Fortunately, there is a garage nearby that stores what we want.", Character.Partner),
			MainCamera.Inst.Transition(TRef.Get(Util.TRefName.CameraPrestory2_1), TRef.Get(Util.TRefName.CameraPrestory2_3), 1.5f)
			);
		yield return AtTheSameTime(
			LineCanvas.Bottom.DisplayLineAndWaitForClick("Shirley", "The turning wheels and the motor wheels.", Character.Partner),
			MainCamera.Inst.Transition(TRef.Get(Util.TRefName.CameraPrestory2_3), TRef.Get(Util.TRefName.CameraPrestory2_4), 1.5f)
			);
		yield return MainCamera.Inst.Transition(TRef.Get(Util.TRefName.CameraPrestory2_4), TRef.Get(Util.TRefName.CameraPrestory2_1), 1.5f);
		yield return LineCanvas.Bottom.DisplayLineAndWaitForClick("Shirley", "With them, we can steer the car easily.", Character.Partner);
		
		yield return LineCanvas.Bottom.DisplayLineAndWaitForClick("Shirley", "This time, would you like to try it yourself?", Character.Partner);
		ChoiceCanvas.Inst.DisplayChoices(new() { ("Let me try it!", Util.ChoiceName.DontNeedHelp), ("I need help!", Util.ChoiceName.NeedHelp) });
		Util.ChoiceObj choice_obj = new();
		last_choice_name = choice_obj.choice_name;
		yield return WaitForChoice(choice_obj);
		// choice_name = choice_obj.choice_name;
		if (choice_obj.choice_name == Util.ChoiceName.DontNeedHelp)
		{
			yield return LineCanvas.Bottom.DisplayLineAndWaitForClick("Shirley", "I admire your courage. Good luck!", Character.Partner);
		}
		else
		{
			yield return LineCanvas.Bottom.DisplayLineAndWaitForClick("Shirley", "I admire your modesty. Let's figure it out together.", Character.Partner);
		}
		Character.Piggy.WarpTo(TRef.Get(Util.TRefName.Origin));
		Character.Partner.WarpTo(TRef.Get(Util.TRefName.Origin));
		LineCanvas.Bottom.Hide();
		Util.BuildInfo build_info = choice_obj.choice_name == Util.ChoiceName.DontNeedHelp? Util.BuildInfo.DontNeedHelp: Util.BuildInfo.NeedHelp;
		TransitionToBuild(Util.WaypointName.PreStory2, Util.GoalName.PreStory2, build_info);
	}
	IEnumerator WaitForChoice(Util.ChoiceObj choice_obj)
	{
		Action<ChoiceSelectedEvent> handler = (ChoiceSelectedEvent e) => { choice_obj.choice_name = e.choice_name; };
		var subscription = EventBus.Subscribe<ChoiceSelectedEvent>(handler);
		while(choice_obj.choice_name == Util.ChoiceName.None)
		{
			yield return null;
		}
		EventBus.Unsubscribe(subscription);
	}
	IEnumerator TransitionToStoryInTown()
	{
		DampStart();
		yield return MainCamera.Inst.WarpTo(TRef.Get(Util.TRefName.CameraInTown1), 1.5f);
		yield return AtTheSameTime(
			MainCamera.Inst.WarpTo(TRef.Get(Util.TRefName.CameraInTown2), 1.5f),
			LineCanvas.Bottom.DisplayLine("Shirley", "We've arrived! Let's explore the town."));
		yield return new WaitForSeconds(1.0f);
		LineCanvas.Bottom.Hide();
		DampStop();
		// MainCamera.Inst.FollowStory();
		yield return new WaitForSeconds(1.0f);
		TransitionToPlay(false);
		Goal.Activate(Util.GoalName.Town);

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
	void TransitionToBuild(Util.WaypointName waypoint_name, Util.GoalName goal_name, Util.BuildInfo build_info = Util.BuildInfo.NeedHelp)
	{
		current_waypoint = waypoint_name;
		retry_waypoint = waypoint_name;
		retry_goal = goal_name;
		//if (can_retry_waypoints.Contains(waypoint_name))
		//{
			
		//}
		//else
		//{
		//	// waypoint_name = Util.WaypointName.None;
		//	retry_waypoint = Util.WaypointName.None;
		//	retry_goal = Util.GoalName.None;
		//}
		BuildCanvas.Inst.Show();
		PlayCanvas.Inst.Hide();
		// AudioPlayer.Inst.TransitionToBuild();
		DestroyComponentsInScene();		
		DragImage.Current = null;
		GridMatrix.DeselectGridMatrix();
		GridMatrix.SelectGridMatrix(waypoint_name, build_info != Util.BuildInfo.NeedHelp);
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
				StartCoroutine(Prestory2Build(build_info));
				break;
			case Util.WaypointName.Town:
				if (!town_waypoint_met)
				{
					town_waypoint_met = true;
					StartCoroutine(TownWaypointBuild());
				}
				break;
		}
	}
	bool town_waypoint_met = false;
	IEnumerator Prestory1Build()
	{
		yield return new WaitForSeconds(1.5f);
		Trash.Inst.gameObject.SetActive(false);
		yield return LineCanvas.Top.DisplayLineAndWaitForClick("Shirley", "Welcome to the grid building system!", null);
		yield return LineCanvas.Top.DisplayLineAndWaitForClick("Shirley", "Oh! There is a mess! Let's clean it up using the eraser!", null);
		EraserImage.Inst.StartScale();
		//// to do: display line and wait for event

		yield return LineCanvas.Top.DisplayLineAndWaitForEvent("Shirley", "Start by clicking the eraser.", (ToolClickedEvent e) =>
		{
			return e.cursor_mode == Util.CursorMode.Erase;
		});
		EraserImage.Inst.EndScale();
		yield return LineCanvas.Top.DisplayLineAndWaitForEvent("Shirley", "Good! Hover your mouse on the grid and click to erase a component.",
			(ItemErasedEvent e) => true);
		Trash.Inst.gameObject.SetActive(true);
		Trash.Inst.StartScale();
		yield return LineCanvas.Top.DisplayLineAndWaitForEvent("Shirley", "Perfect! Now let's use the trashcan to remove all the components at once!",
			(ResetCountEvent e) => true);
		Trash.Inst.EndScale();
		yield return LineCanvas.Top.DisplayLineAndWaitForClick("Shirley", "Perfect! Now we have a clear space to build our vehicle!", null);
		GridMatrix.Current.ShowDesign();
		yield return LineCanvas.Top.DisplayLineAndWaitForClick("Shirley", "For now, let's adhere to a standard vehicle design", null);
		DragImage.StartScaleAll();
		yield return LineCanvas.Top.DisplayLineAndWaitForEvent("Shirley", "Start by clicking on a component icon.",
			(DragImageClickedEvent e) => true);
		DragImage.EndScaleAll();
		yield return LineCanvas.Top.DisplayLineAndWaitForEvent("Shirley", "Perfect! Hover your mouse on the grid and click to place a component.",
			(ComponentAddedEvent e) => true);
		yield return LineCanvas.Top.DisplayLineAndWaitForClick("Shirley", "Perfect! Let's place the rest of the components.", null);
		yield return LineCanvas.Top.WaitForEvent((ReadyToGoEvent e) => true);
		yield return LineCanvas.Top.DisplayLineAndWaitForEvent("Shirley", "You are learning fast! Now click the confirm button to start our journey!",
			(ConfirmSuccessEvent e) => true);
		LineCanvas.Top.Hide();
	}

	public IEnumerator Prestory2Build(Util.BuildInfo build_info)
	{
		StoryAnimation.Inst.CanSpeedup = false;
		yield return new WaitForSeconds(1.5f);
		switch (build_info)
		{
			case Util.BuildInfo.NeedHelp:
				{
					ConfirmButton.Inst.EnableConfirm = false;
					yield return LineCanvas.Top.DisplayLineAndWaitForClick("Shirley", "Follow the design to build the vehicle.", null);
					LineCanvas.Top.Hide();
					ConfirmButton.Inst.EnableConfirm = true;
					yield return LineCanvas.Top.WaitForEvent((ReadyToGoEvent e) => true);
					yield return LineCanvas.Top.DisplayLineAndWaitForEvent("Shirley", "Let's roll!",
						(ConfirmSuccessEvent e) => true);
					LineCanvas.Top.Hide();
				}
				break;
			case Util.BuildInfo.DontNeedHelp:
				{
					ConfirmButton.Inst.EnableConfirm = false;
					yield return LineCanvas.Top.DisplayLineAndWaitForClick("Shirley", "Try to build the vehicle yourself!", null);
					LineCanvas.Top.Hide();
					ConfirmButton.Inst.EnableConfirm = true;
					// yield return LineCanvas.Top.WaitForEvent((ConfirmSuccessEvent e) => true);
				}
				break;
			case Util.BuildInfo.DontNeedHelpButRetry:
				{
					ConfirmButton.Inst.EnableConfirm = false;
					yield return LineCanvas.Top.DisplayLineAndWaitForClick("Shirley", "It seems you had a rough time. Would you like to get some hint?", null);
					ChoiceCanvas.Inst.DisplayChoices(new() { ("Ok, I need some help.", Util.ChoiceName.NeedHelp), ("No way. Let me try it myself!", Util.ChoiceName.DontNeedHelp) });
					Util.ChoiceObj choice_obj = new();
					last_choice_name = choice_obj.choice_name;
					yield return WaitForChoice(choice_obj);
					if (choice_obj.choice_name == Util.ChoiceName.DontNeedHelp)
					{
						yield return LineCanvas.Top.DisplayLineAndWaitForClick("Shirley", "All right. Good luck!", null);
						LineCanvas.Top.Hide();
						ConfirmButton.Inst.EnableConfirm = true;
					}
					else
					{
						yield return LineCanvas.Top.DisplayLineAndWaitForClick("Shirley", "Good Choice! Let's figure it out together!", null);
						LineCanvas.Top.Hide();
						ConfirmButton.Inst.EnableConfirm = true;
						TransitionToBuild(Util.WaypointName.PreStory2, Util.GoalName.PreStory2, Util.BuildInfo.NeedHelp);
						yield break;
					}
				}
				break;
		}
	}
	public IEnumerator TownWaypointBuild()
	{
		yield return new WaitForSeconds(1.5f);

		yield return LineCanvas.Top.DisplayLineAndWaitForEvent("Shirley", "**Drag the screen to view the grid**", (GridMatrixDragEvent e) => true);
		yield return LineCanvas.Top.DisplayLineAndWaitForClick("Shirley", "Perfect! Now there's an **important** feature that you want to learn", null);
		yield return LineCanvas.Top.DisplayLineAndWaitForEvent("Shirley", "**Press \"Space\" to toggle build layers.**", (SwitchLayerEvent e) => true);
		yield return LineCanvas.Top.DisplayLineAndWaitForClick("Shirley", "Perfect! Without previous design constraints, it would be hard to locate a cell without specifying layers.", null);
		yield return LineCanvas.Top.DisplayLineAndWaitForEvent("Shirley", "Now, press \"Space\" a few more times to go back to the full layer mode.", (FullLayerEvent e) => true);
		yield return LineCanvas.Top.DisplayLineAndWaitForClick("Shirley", "Awesome! Feel free to explore the world!", null);
		LineCanvas.Top.Hide();
		Goal.Activate(Util.GoalName.C1S1);
		Character.Partner.WarpTo(TRef.Get(Util.TRefName.PartnerC1S1));
		yield break;
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
			case Util.GoalName.Town:
				TransitionToStory(Util.StoryName.TownWaypoint);
				break;
			case Util.GoalName.C1S1:
				TransitionToStory(Util.StoryName.C1S1);
				break;
			case Util.GoalName.C1S2:
				TransitionToStory(Util.StoryName.C1S2);
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
				case Util.WaypointName.PreStory1:
					FirstPerson.Inst.Hide();
					Retry.Inst.Hide();
					break;
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
		yield return LineCanvas.Bottom.DisplayLineAndWaitForClick("Shirley", "Drive straight into the winding valley. That's the shortest path.", null);
		LineCanvas.Bottom.Hide();
	}
}
