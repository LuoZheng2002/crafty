using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using static Util;

public class InvisibleStateUpdateEvent
{
}
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
			// PiggyCameraPivot.Inst.OnFirstPersonChanged(value);
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
	void Yikai()
	{
		Util.unbreakable = true;
		// TransitionToStory(Util.StoryName.Crash);
		// TransitionToFirstBuild();
		Util.unbreakable = false;
		GameSave.IncrementGridSize(1, 1, 0);
		GameSave.CurrentMemory.MemCrates[1, 0, 0] = Util.Component.WoodenCrate;
		GameSave.CurrentMemory.MemAccessories[1, 0, 1] = Util.Component.Rocket;
		GameSave.CurrentMemory.MemCrates[1, 0, 2] = Util.Component.WoodenCrate;
		GameSave.CurrentMemory.MemAccessories[1, 1, 0] = Util.Component.Rocket;
		GameSave.CurrentMemory.MemCrates[1, 1, 1] = Util.Component.WoodenCrate;
		GameSave.CurrentMemory.MemAccessories[1, 1, 2] = Util.Component.Rocket;
		GameSave.CurrentMemory.MemCrates[1, 2, 0] = Util.Component.WoodenCrate;
		GameSave.CurrentMemory.MemAccessories[1, 2, 1] = Util.Component.Rocket;
		GameSave.CurrentMemory.MemCrates[1, 2, 2] = Util.Component.WoodenCrate;
		GameSave.CurrentMemory.MemLoads[1, 2, 2] = Util.Component.Pig;
		GameSave.CurrentMemory.MemLoads[1, 0, 2] = Util.Component.Partner;
		GameSave.CurrentMemory.MemAccessories[0, 0, 0] = Util.Component.MotorWheel;
		GameSave.CurrentMemory.MemAccessories[0, 0, 2] = Util.Component.TurnWheel;
		GameSave.CurrentMemory.MemAccessories[0, 2, 0] = Util.Component.MotorWheel;
		GameSave.CurrentMemory.MemAccessories[0, 2, 2] = Util.Component.TurnWheel;
		GameSave.Inventory[Util.Component.MotorWheel] += 2;
		GameSave.Inventory[Util.Component.TurnWheel] += 2;
		GameSave.Inventory[Util.Component.WoodenCrate] = 9;
		GameSave.Inventory[Util.Component.Rocket] = 9;
		GameSave.Inventory[Util.Component.Umbrella] = 9;

		GoToCheckpointAsync(Util.WaypointName.VolcanoGate, true);
		//GoToCheckpointAsync(Util.WaypointName.VolcRoom4, true);

		// TransitionToBuild(Util.WaypointName.PreStory2, Util.GoalName.PreStory2);
		//  TransitionToBuild(Util.WaypointName.None, Util.GoalName.PreStory2);
		// TransitionToStory(Util.StoryName.TownWaypoint);
		// town_waypoint_met = true;
		// TransitionToBuild(Util.WaypointName.C1S1, Util.GoalName.None);
		// TransitionToStory(Util.StoryName.C1S1);
		// TransitionToBuild(Util.WaypointName.Volcano, Util.GoalName.VolcBottom);
		// TransitionToBuild(Util.WaypointName.VolcBottom, Util.GoalName.VolcTop);
		// FirstPerson.Inst.Show();
		// Retry.Inst.Show();
		// GoalCanvas.Inst.GoalToFollow = Util.GoalName.PreStory1;
		// Goal.Activate(Util.GoalName.PreStory1);
		PlayCanvas.Inst.HideUmbrella();
		PlayCanvas.Inst.HideRocket();
	}
	IEnumerator AtTownWaypoint()
	{
		yield return LineCanvas.Top.DisplayLineAndWaitForClick("Shirley", "You've reached a permanent waypoint!", null);
		MapImage.Inst.Show();
		MapImage.Inst.StartScale();
		yield return LineCanvas.Top.DisplayLineAndWaitForEvent("Shirley", "Click on the map to see the whole region!", (MapImageClickedEvent e) => true);
		LineCanvas.Top.Hide();
	}
	IEnumerator DisplayChapter(string type_str, string text_str, string state_str)
	{
		yield return BlackoutCanvas.Inst.Blackout(1.0f, 0.0f, 1.0f);
		ChapterCanvas.Inst.DisplayTextAsync(type_str, text_str, state_str, 0.5f);
		yield return BlackoutCanvas.Inst.Blackout(0.5f, 1.0f, 0.0f);
	}
	public void OnCheckpointReached(CheckpointReachedEvent e)
	{
		Util.Delay(this, 1, () =>
		{
			switch (e.waypoint_name)
			{
				case WaypointName.MotorWheel:
					Checkpoint.Get(WaypointName.TurnWheel).Activate();
					GoalCanvas.Inst.CheckpointToFollow = WaypointName.TurnWheel;
					GameSave.Inventory[Util.Component.MotorWheel] = 2;
					ObtainCanvas.Inst.Show(Util.Component.MotorWheel);
					Retry.Inst.Show();
					RebuildButton.Inst.Show();
					RebuildButton.Inst.StartScale();
					// BackButton.Inst.Show();
					break;
				case WaypointName.TurnWheel:
					Util.unbreakable = false;
					Checkpoint.Get(WaypointName.Turn1).Activate();
					GoalCanvas.Inst.CheckpointToFollow = WaypointName.Turn1;
					GameSave.Inventory[Util.Component.TurnWheel] = 2;
					GameSave.Inventory[Util.Component.WoodenCrate] += 3;
					ObtainCanvas.Inst.Show(Util.Component.TurnWheel);
					RebuildButton.Inst.StartScale();
					GameSave.IncrementGridSize(0, 1, 0);
					BuildCanvas.Inst.ShowDesignNumbers();
					break;
				case WaypointName.Turn1:
					Checkpoint.Get(WaypointName.Lake).Activate();
					GoalCanvas.Inst.CheckpointToFollow = WaypointName.Lake;
					break;
				case WaypointName.Lake:
					Checkpoint.Get(WaypointName.TownEntrance).Activate();
					GoalCanvas.Inst.CheckpointToFollow = WaypointName.TownEntrance;
					break;
				case WaypointName.TownEntrance:
					Checkpoint.Get(WaypointName.TownWaypoint).Activate();
					GoalCanvas.Inst.CheckpointToFollow = WaypointName.TownWaypoint;
					break;
				case WaypointName.TownWaypoint:
					Checkpoint.Get(WaypointName.Gate).Activate();
					GoalCanvas.Inst.CheckpointToFollow = WaypointName.Gate;
					StartCoroutine(AtTownWaypoint());
					break;
				case WaypointName.Gate:
					Checkpoint.Get(WaypointName.Rocket).Activate();
					GoalCanvas.Inst.CheckpointToFollow = WaypointName.Rocket;
					break;
				case WaypointName.Rocket:
					Checkpoint.Get(WaypointName.Umbrella).Activate();
					GoalCanvas.Inst.CheckpointToFollow = WaypointName.Umbrella;
					GameSave.Inventory[Util.Component.Rocket] = 6;
					GameSave.IncrementGridSize(1, 0, 0);
					ObtainCanvas.Inst.Show(Util.Component.Rocket);
					Retry.Inst.Show();
					RebuildButton.Inst.StartScale();
					break;
				case WaypointName.Umbrella:
					Checkpoint.Get(WaypointName.VolcanoGate).Activate();
					GoalCanvas.Inst.CheckpointToFollow = WaypointName.VolcanoGate;
					GameSave.Inventory[Util.Component.Umbrella] = 6;
					ObtainCanvas.Inst.Show(Util.Component.Umbrella);
					Retry.Inst.Show();
					RebuildButton.Inst.StartScale();
					break;
				case WaypointName.VolcanoGate:
					Checkpoint.Get(WaypointName.VolcRoom1_1).Activate();
					GoalCanvas.Inst.CheckpointToFollow = WaypointName.VolcRoom1_1;
					break;
				case WaypointName.VolcRoom1_1:
					Checkpoint.Get(WaypointName.VolcRoom1_2).Activate();
					GoalCanvas.Inst.CheckpointToFollow = WaypointName.VolcRoom1_2;
					break;
				case WaypointName.VolcRoom1_2:
					Checkpoint.Get(WaypointName.VolcRoom1_3).Activate();
					GoalCanvas.Inst.CheckpointToFollow = WaypointName.VolcRoom1_3;
					break;
				case WaypointName.VolcRoom1_3:
					Checkpoint.Get(WaypointName.VolcRoom2_1).Activate();
					GoalCanvas.Inst.CheckpointToFollow = WaypointName.VolcRoom2_1;
					break;
				case WaypointName.VolcRoom2_1:
					Checkpoint.Get(WaypointName.VolcRoom3).Activate();
					GoalCanvas.Inst.CheckpointToFollow = WaypointName.VolcRoom3;
					break;
				case WaypointName.VolcRoom3:
					Checkpoint.Get(WaypointName.VolcRoom4).Activate();
					GoalCanvas.Inst.CheckpointToFollow = WaypointName.VolcRoom4;
					break;
				case WaypointName.VolcRoom4:
					Checkpoint.Get(WaypointName.VolcTop).Activate();
					GoalCanvas.Inst.CheckpointToFollow = WaypointName.VolcTop;
					break;
				case WaypointName.VolcTop:
					Checkpoint.Get(WaypointName.VolcPlatform).Activate();
					GoalCanvas.Inst.CheckpointToFollow = WaypointName.VolcPlatform;
					break;
				case WaypointName.VolcPlatform:
					Checkpoint.Get(WaypointName.VolcPlatform2).Activate();
					GoalCanvas.Inst.CheckpointToFollow = WaypointName.VolcPlatform2;
					break;
				case WaypointName.VolcPlatform2:
					Util.unbreakable = true;
					break;
			}
		});
	}
	private void Start()
	{
		// temporary shut down
		Debug.Assert(inst == null, "Game State already instantiated");
		inst = this;
		Util.Delay(this, () =>
		{
			Yikai();
			BuildCanvas.Inst.HideDesignNumbers();
			PlayCanvas.Inst.HideStory();
			BlackoutCanvas.Inst.BlackoutAsync(3.0f, 1.0f, 0.0f);
		});
		EventBus.Subscribe<GoalReachedEvent>(OnGoalReached);
		EventBus.Subscribe<TouchLavaEvent>(OnTouchLava);
		EventBus.Subscribe<ScanSuccessEvent>(OnScanSuccess);
		EventBus.Subscribe<ScanFailEvent>(OnScanFail);
		EventBus.Subscribe<CheckpointReachedEvent>(OnCheckpointReached);

		Util.Delay(this, 1, ()=> { MapCanvas.Inst.Deactivate(); });
	}
	IEnumerator ScanSuccessHelper()
	{
		PlayCanvas.Inst.Hide();
		Debug.LogWarning($"joints: {CarCore.Inst.joint != null}, {CarCore.Inst.fix_joint != null}");
		CarCore.Inst.ActivateContainer();
		CarCore.Inst.DestroyComponents();		
		CarCore.Inst.Unfix();
		yield return null;
		yield return CarCore.Inst.AlignToGridMatrix();
		CarCore.Inst.Fix();
		BuildCanvas.Inst.Show();
		BuildCanvas.Inst.InitializeItems();
		yield return GridMatrix.Inst.Activate();
	}
	void OnScanSuccess(ScanSuccessEvent e)
	{
		StartCoroutine(ScanSuccessHelper());
	}
	public void GoBack()
	{
		PlayCanvas.Inst.Show();
		BuildCanvas.Inst.Hide();
		GridMatrix.Inst.Deactivate();
		CarCore.Inst.Unfix();
		CarCore.Inst.ActivateContainer();
	}
	void OnScanFail(ScanFailEvent e)
	{
		CarCore.Inst.ActivateContainer();
		CarCore.Inst.Unfix();
	}
	public IEnumerator GoToCheckpoint(Util.WaypointName waypoint_name, bool camera_follow)
	{
		if (camera_follow)
		{
			MainCamera.Inst.Stop();
		}
		AudioPlayer.Inst.StopSoundEffect();
		PlayCanvas.Inst.Hide();
		GridMatrix.Inst.MoveToCheckpoint(waypoint_name);
		yield return null;
		Debug.Assert(!GridMatrix.Inst.Active);
		CarCore.Inst.DestroyComponents();
		yield return null;
		yield return CarCore.Inst.AlignToGridMatrix();
		Debug.Log("Fix!");
		CarCore.Inst.Fix();
		BuildCanvas.Inst.Show();
		BuildCanvas.Inst.InitializeItems();
		yield return GridMatrix.Inst.Activate();
		GridMatrix.Inst.MoveProbeToOrigin();
		CarCore.Inst.ResetPivot();
		if (camera_follow)
		{
			MainCamera.Inst.MoveAndStickTo(CarCore.Inst.CameraEnd);
		}
	}
	public void GoToCheckpointAsync(Util.WaypointName waypoint_name, bool camera_follow = true)
	{
		StartCoroutine(GoToCheckpoint(waypoint_name, camera_follow));
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
		//// PiggyCameraPivot.Inst.EndFollow();
		//camera_follow_pig = false;
		//PiggyPermitInvisible = false;
		//Util.BuildInfo build_info = last_choice_name == Util.ChoiceName.NeedHelp ? Util.BuildInfo.NeedHelp : Util.BuildInfo.DontNeedHelpButRetry;
		//TransitionToBuild(retry_waypoint, retry_goal, build_info);
		GoToCheckpointAsync(GameSave.CurrentCheckpoint);
	
	}
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A)
			|| Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D))
		{
			EventBus.Publish(new WASDPressedEvent());
		}
		CheatCode();
	}
	
	void CheatCode()
	{
		//if (Input.GetKeyDown(KeyCode.Y))
		//{
		//	TryScan();
		//}
		//if (Input.GetKeyDown(KeyCode.E))
		//{
		//	CarCore.Inst.Move();
		//}
		//if (Input.GetKeyDown(KeyCode.R))
		//{
		//	GoToCheckpointAsync(Util.WaypointName.PreStory1);
		//}
		//if (Input.GetKeyDown(KeyCode.T))
		//{
		//	GoToMap();
		//}
		if (Input.GetKeyDown(KeyCode.P))
		{
			StartCoroutine(DisplayChapter("Main Story Quest: Act 1", "Treasures in the Flaming Mountain", "Started"));
		}
		if (Input.GetKeyDown(KeyCode.O))
		{
			StoryCanvas.Inst.Show();
		}
	}
	public void GoToMap()
	{
		MainCamera.Inst.Stop();
		MainCamera.Inst.MoveAndStickTo(BigMapCamera.Inst.transform);
		MapCanvas.Inst.Activate();
		BigMapCamera.Inst.Activate();
		PlayCanvas.Inst.Hide();
	}
	public void DampStart()
	{
		foreach (var component in Components)
		{
			component.DampStart();
		}
	}
	public void DampStop()
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
		if (story_name != Util.StoryName.Intro)
		{
			AudioPlayer.Inst.TransitionToStory();
		}
		Goal.Deselect();
		PlayCanvas.Inst.Hide();
		// PiggyCameraPivot.Inst.EndFollow();
		PiggyPermitInvisible = false;
		MainCamera.Inst.Stop();
		EventBus.Publish(new InvisibleStateUpdateEvent());
		if (story_name != Util.StoryName.Intro && story_name!= Util.StoryName.FallOffCliff
			&& story_name != Util.StoryName.InTown && story_name != Util.StoryName.C1S2)
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
			case Util.StoryName.C1S1:
				StartCoroutine(TransitionToStoryC1S1());
				break;
			case Util.StoryName.C1S2:
				// StartCoroutine(TransitionToStoryC1S2());
				break;
		}
	}
	IEnumerator TransitionToStoryC1S1()
	{
		yield return MainCamera.Inst.WarpTo(TRef.Get(Util.TRefName.CameraC1S1), 1.0f);
		yield return LineCanvas.Bottom.DisplayLineAndWaitForClick("Shirley", "It seems you've recovered. Let's travel around and see if anyone knows about your girlfriend.", Character.Partner);
		ChoiceCanvas.Inst.DisplayChoices(new() { ("Sure. Let's go!", Util.ChoiceName.DontCare), ("Definitely!", Util.ChoiceName.DontCare), ("Absolutely!", Util.ChoiceName.DontCare) });
		Util.ChoiceObj choice_obj = new();
		yield return WaitForChoice(choice_obj);
		LineCanvas.Bottom.Hide();
		Character.Partner.WarpTo(TRef.Get(Util.TRefName.Origin));
		Character.GetCharacter(Util.CharacterName.NPC1).WarpTo(TRef.Get(Util.TRefName.NPC1C1S2));
		Character.GetCharacter(Util.CharacterName.NPC2).WarpTo(TRef.Get(Util.TRefName.NPC2C1S2));
		Character.GetCharacter(Util.CharacterName.NPC3).WarpTo(TRef.Get(Util.TRefName.NPC3C1S2));
		// TransitionToBuild(Util.WaypointName.C1S1, Util.GoalName.C1S2);
	}
	//void ShowVehicle()
	//{
	//	foreach(var component in Components)
	//	{
	//		component.gameObject.SetActive(true);
	//	}
	//}
	//void HideVehicle()
	//{
	//	foreach (var component in Components)
	//	{
	//		component.gameObject.SetActive(false);
	//	}
	//}
	//IEnumerator TransitionToStoryC1S2()
	//{
	//	DampStart();
	//	GridMatrix.Get(Util.WaypointName.C1S1).gameObject.SetActive(false);
	//	Character.Piggy.WarpTo(TRef.Get(Util.TRefName.PiggyC1S2));
	//	Character.Partner.WarpTo(TRef.Get(Util.TRefName.PartnerC1S2));
	//	yield return MainCamera.Inst.WarpTo(TRef.Get(Util.TRefName.CameraC1S2_1), 1.0f);

	//	yield return LineCanvas.Bottom.DisplayLineAndWaitForClick("Jerry", "The Groundhog. It's attacking the town again.", Character.GetCharacter(Util.CharacterName.NPC1));
	//	yield return LineCanvas.Bottom.DisplayLineAndWaitForClick("Tom", "What should we do?", Character.GetCharacter(Util.CharacterName.NPC2));
	//	yield return LineCanvas.Bottom.DisplayLineAndWaitForClick("Spike", "According to my experience, there will soon be a guy falling from the sky to save us.", Character.GetCharacter(Util.CharacterName.NPC3));
	//	yield return MainCamera.Inst.WarpTo(TRef.Get(Util.TRefName.CameraC1S2_2), 1.0f);
	//	yield return LineCanvas.Bottom.DisplayLineAndWaitForClick("Shirley", "Hi, excuse me, have you seen a monster kidnapping a girl walking by?", Character.Partner);
	//	yield return MainCamera.Inst.WarpTo(TRef.Get(Util.TRefName.CameraC1S2_1), 1.0f);
	//	yield return LineCanvas.Bottom.DisplayLineAndWaitForClick("Jerry", "Nope. What about you, Tom?", Character.GetCharacter(Util.CharacterName.NPC1));
	//	yield return LineCanvas.Bottom.DisplayLineAndWaitForClick("Tom", "Nope. What about you, Spike?", Character.GetCharacter(Util.CharacterName.NPC2));
	//	yield return LineCanvas.Bottom.DisplayLineAndWaitForClick("Spike", "Nope.", Character.GetCharacter(Util.CharacterName.NPC3));
	//	yield return MainCamera.Inst.WarpTo(TRef.Get(Util.TRefName.CameraC1S2_2), 1.0f);
	//	yield return LineCanvas.Bottom.DisplayLineAndWaitForClick("Shirley", "Ah, that's disappointing.", Character.Partner);
	//	LineCanvas.Bottom.Hide();
	//	MainCamera.Inst.FollowStory();
	//	StoryAnimation.Inst.PlayAnimation(Util.StoryName.C1S2);
	//	yield return LineCanvas.Bottom.WaitForEvent((AnimationExitEvent e) => true);
	//	MainCamera.Inst.Stop();
	//	yield return MainCamera.Inst.WarpTo(TRef.Get(Util.TRefName.CameraC1S2_2), 1.0f);
	//	yield return LineCanvas.Bottom.DisplayLineAndWaitForClick("Shirley", "Oh no! Something terrible just happened!", Character.Partner);
	//	yield return MainCamera.Inst.WarpTo(TRef.Get(Util.TRefName.CameraC1S2_1), 1.0f);
	//	yield return LineCanvas.Bottom.DisplayLineAndWaitForClick("Jerry", "Please, outlander, please kill that groundhog and save our town!", Character.GetCharacter(Util.CharacterName.NPC1));
	//	yield return LineCanvas.Bottom.DisplayLineAndWaitForClick("Tom", "I know you can do it!", Character.GetCharacter(Util.CharacterName.NPC2));
	//	yield return LineCanvas.Bottom.DisplayLineAndWaitForClick("Spike", "You are the chosen one!", Character.GetCharacter(Util.CharacterName.NPC3));
	//	yield return MainCamera.Inst.WarpTo(TRef.Get(Util.TRefName.CameraC1S2_2), 1.0f);
	//	yield return LineCanvas.Bottom.DisplayLineAndWaitForClick("You", "All right, all right. I know this is happening.", Character.Piggy);
	//	yield return LineCanvas.Bottom.DisplayLineAndWaitForClick("You", "But how is it possible to beat such a giant guy?", Character.Piggy);
	//	yield return MainCamera.Inst.WarpTo(TRef.Get(Util.TRefName.CameraC1S2_1), 1.0f);
	//	yield return LineCanvas.Bottom.DisplayLineAndWaitForClick("Jerry", "There is a myth saying that the only way to beat the ground hog is...", Character.GetCharacter(Util.CharacterName.NPC1));
	//	yield return LineCanvas.Bottom.DisplayLineAndWaitForClick("Tom", "Is...", Character.GetCharacter(Util.CharacterName.NPC2));
	//	yield return AtTheSameTime(
	//		LineCanvas.Bottom.DisplayLineAndWaitForClick("Spike", "Is to obtain the enchanted obsidian lying in the heart of the volcano.", Character.GetCharacter(Util.CharacterName.NPC3)),
	//		MainCamera.Inst.WarpTo(TRef.Get(Util.TRefName.CameraC1S2_3), 4.0f));
	//	yield return LineCanvas.Bottom.DisplayLineAndWaitForClick("Jerry", "And the prophecy says that there will be a hero driving through the lava, and eventually...", Character.GetCharacter(Util.CharacterName.NPC1));
	//	yield return LineCanvas.Bottom.DisplayLineAndWaitForClick("Tom", "Eventually...", Character.GetCharacter(Util.CharacterName.NPC2));
	//	yield return AtTheSameTime(
	//		LineCanvas.Bottom.DisplayLineAndWaitForClick("Spike", "Eventually he flew out from the top of the volcano!", Character.GetCharacter(Util.CharacterName.NPC3)),
	//		MainCamera.Inst.WarpTo(TRef.Get(Util.TRefName.CameraC1S2_4), 2.0f));
	//	yield return MainCamera.Inst.WarpTo(TRef.Get(Util.TRefName.CameraC1S2_2), 2.0f);
	//	yield return LineCanvas.Bottom.DisplayLineAndWaitForClick("Shirley", "Yeah, look how poor they are! Let's help them!", Character.Partner);
	//	yield return LineCanvas.Bottom.DisplayLineAndWaitForClick("You", "All right, all right. I'll try my best.", Character.Piggy);
	//	yield return MainCamera.Inst.WarpTo(TRef.Get(Util.TRefName.CameraC1S2_1), 1.0f);
	//	yield return LineCanvas.Bottom.DisplayLineAndWaitForClick("Jerry", "The journey will be dangerous. So, take my umbrellas.", Character.GetCharacter(Util.CharacterName.NPC1));
	//	yield return LineCanvas.Bottom.DisplayLineAndWaitForClick("Tom", "Take my rockets.", Character.GetCharacter(Util.CharacterName.NPC2));
	//	yield return LineCanvas.Bottom.DisplayLineAndWaitForClick("Spike", "Take care.", Character.GetCharacter(Util.CharacterName.NPC3));
	//	yield return MainCamera.Inst.WarpTo(TRef.Get(Util.TRefName.CameraC1S2_2), 1.0f);
	//	yield return LineCanvas.Bottom.DisplayLineAndWaitForClick("You", "Thank you guys.", Character.Piggy);
	//	LineCanvas.Bottom.Hide();
	//	Character.Piggy.WarpTo(TRef.Get(Util.TRefName.Origin));
	//	Character.Partner.WarpTo(TRef.Get(Util.TRefName.Origin));
	//	DampStop();
	//	GridMatrix.Get(Util.WaypointName.C1S1).gameObject.SetActive(true);
	//	TransitionToPlay(false);
	//	yield return LineCanvas.Top.DisplayLineAndWaitForClick("Shirley", "Now let's march towards the volcano.", Character.Partner);
	//	Goal.Activate(Util.GoalName.Volcano);
	//}
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
		// Waypoint.Waypoints[Util.WaypointName.Town].ChangeToGreen();
		yield return WaitForClick();
		yield return MainCamera.Inst.WarpTo(TRef.Get(Util.TRefName.CameraTownW1), 1.0f);
		yield return LineCanvas.Bottom.DisplayLineAndWaitForClick("Shirley", "Let's try it out!", Character.Partner);
		LineCanvas.Bottom.Hide();
		Character.Partner.WarpTo(TRef.Get(Util.TRefName.Origin));
		// TransitionToBuild(Util.WaypointName.Town, Util.GoalName.None);
	}

	public void Test(Action func)
	{
		try
		{
			func();
		}
		catch (Exception e)
		{
			ToastManager.Toast($"{e.Message}\n{e.StackTrace}");
		}
	}
	IEnumerator TransitionToStoryCrash()
	{
		
		//yield return BlackoutCanvas.Inst.Blackout(1.0f, 1.0f, 1.0f);
		//	yield return BlackoutCanvas.Inst.DisplaySub("You and your girlfriend's spaceship crashed to this planet because of an attack.", 1.5f, 0.0f, 1.0f);
		//	yield return WaitForClick();
		//	yield return BlackoutCanvas.Inst.DisplaySub("You and your girlfriend's spaceship crashed to this planet because of an attack.", 0.5f, 1.0f, 0.0f);
		Test(() =>
		{
			MainCamera.Inst.WarpTo(TRef.Get(Util.TRefName.CameraPrestory1_1));
		});

		// StoryAnimation.Inst.PlayAnimation(Util.StoryName.Crash);
		//yield return BlackoutCanvas.Inst.Blackout(1.5f, 1.0f, 0.2f);
		//yield return BlackoutCanvas.Inst.Blackout(1.5f, 0.2f, 1.0f);
		//yield return BlackoutCanvas.Inst.Blackout(1.5f, 1.0f, 0.4f);
		//yield return BlackoutCanvas.Inst.Blackout(1.5f, 0.4f, 1.0f);
		//yield return new WaitForSeconds(1.0f);
		// yield return BlackoutCanvas.Inst.DisplaySub("You were unconcious for some time", 1.0f, 0.0f, 1.0f);
		// yield return new WaitForSeconds(1.0f);
		// yield return BlackoutCanvas.Inst.DisplaySub(null, 1.0f, 1.0f, 0.0f);
		//Test(() =>
		//{
		//	Character.Partner.WarpTo(TRef.Get(Util.TRefName.PartnerPrestory1));
		//});

		// yield return new WaitForSeconds(1.0f);
		// yield return BlackoutCanvas.Inst.Blackout(1.5f, 1.0f, 0.0f);


		//	yield return LineCanvas.Bottom.DisplayLineAndWaitForClick("???", "Are you all right?", Character.Partner);
		//	yield return LineCanvas.Bottom.DisplayLineAndWaitForClick("You", "Who... who are you?", null);
		//	yield return LineCanvas.Bottom.DisplayLineAndWaitForClick("Shirley", "I'm Shirley, Outrider for the New Sorpigal. Anything I can help?", Character.Partner);

		//Test(() =>
		//{
		//	LineCanvas.Bottom.Hide();
		//});



		//	yield return BlackoutCanvas.Inst.Blackout(0.5f, 0.0f, 1.0f);
		//	yield return BlackoutCanvas.Inst.DisplaySub("You told the stranger everything just happened", 0.5f, 0.0f, 1.0f);
		//	yield return new WaitForSeconds(1.0f);
		//	yield return BlackoutCanvas.Inst.DisplaySub("You told the stranger everything just happened", 0.5f, 1.0f, 0.0f);
		//	yield return BlackoutCanvas.Inst.Blackout(0.5f, 1.0f, 0.0f);
		//	yield return LineCanvas.Bottom.DisplayLineAndWaitForClick("Shirley", "That sounds terrible! " +
		//		"Looks like you are injured. Let's get down to the town to have a rest first.", Character.Partner);
		//	yield return LineCanvas.Bottom.DisplayLineAndWaitForClick("Shirley", "Maybe someone in the town knows where to look for your girlfriend.", Character.Partner);
		yield return LineCanvas.Bottom.WaitForEvent((PigfallAnimationEndEvent e) => true);
		Character.Piggy.WarpTo(TRef.Get(TRefName.PigPrestory1));
		Character.Partner.WarpTo(TRef.Get(TRefName.PartnerPrestory1));
		yield return new WaitForSeconds(1.0f);
		yield return AtTheSameTime(
			Character.Piggy.WarpTo(TRef.Get(TRefName.PigPrestory2), 0.5f),
			Character.Partner.WarpTo(TRef.Get(TRefName.PartnerPrestory2), 0.5f)
			);
		yield return LineCanvas.Bottom.DisplayLineAndWaitForClick("Shirley", "I didn't see our rocket engines nearby. They must have been stolen.", Character.Partner);
		yield return LineCanvas.Bottom.DisplayLineAndWaitForClick("You", "Yeah. Let's travel around and find some clue.", Character.Piggy);
		yield return LineCanvas.Bottom.DisplayLineAndWaitForClick("Shirley", "There are scattered parts nearby. Let's make ourselves a car.", Character.Partner);
		LineCanvas.Bottom.Hide();
		Character.Piggy.WarpTo(TRef.Get(TRefName.Origin));
		Character.Partner.WarpTo(TRef.Get(TRefName.Origin));
		//	yield return AtTheSameTime(
		//		MainCamera.Inst.Transition(TRef.Get(Util.TRefName.CameraPrestory1_1), TRef.Get(Util.TRefName.CameraPrestory1_2), 0.5f),
		//		LineCanvas.Bottom.DisplayLineAndWaitForClick("Shirley", "There are some scattered parts nearby. Let's take advantage of them for a ride.", Character.Partner)
		//		);
		//Test(() =>
		//{
		//	LineCanvas.Bottom.Hide();
		//	Character.Piggy.WarpTo(TRef.Get(Util.TRefName.Origin));
		//	Character.Partner.WarpTo(TRef.Get(Util.TRefName.Origin));
		//});
		//	yield return null;
		//	// TransitionToBuild(Util.WaypointName.PreStory1, Util.GoalName.PreStory1, Util.BuildInfo.NeedHelp);
		yield return TransitionToFirstBuild();

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
		IntroCanvas.Inst.Play(2.5f);
		while (Time.time - start_time < rise_time)
		{
			MainCamera.Inst.transform.position = Vector3.Lerp(initial_position, introCameraTransform.position, (Time.time - start_time) / rise_time);
			Quaternion target_rotation = Quaternion.Slerp(initial_rotation, introCameraTransform.rotation, (Time.time - start_time) / rise_time);
			// Vector3 look_dir = Piggy.transform.position - MainCamera.Inst.transform.position;
			// Quaternion lookat_rotation = Quaternion.LookRotation(look_dir);
			MainCamera.Inst.transform.rotation = Quaternion.Slerp(initial_rotation, target_rotation, (Time.time - start_time) / rise_time);
			yield return null;
		}
		MainCamera.Inst.transform.position = introCameraTransform.position;
		MainCamera.Inst.transform.rotation = introCameraTransform.rotation;
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

		Retry.Inst.Hide();
		// Reset to the original position
		MainCamera.Inst.transform.position = original_position;

		yield return GoToCheckpoint(Util.WaypointName.Cliff, false);
		// yield return new WaitForSeconds(5.0f);
		yield return null;
		yield return null;
		yield return null;
		yield return null;
		yield return null;
		ConfirmButton.Inst.ForceConfirmClicked();
		// DestroyComponentsInScene();
		yield return null;
		MainCamera.Inst.MoveAndStickTo(CarCore.Inst.CameraEnd);

		// GameSave.Inventory[Util.Component.MotorWheel] = 2;
		// ObtainCanvas.Inst.Show(Util.Component.MotorWheel);
		GridMatrix.Inst.ForceDesign = false;

		// Goal.Activate(Util.GoalName.TurnWheel);
		Checkpoint.Get(Util.WaypointName.MotorWheel).Activate();
		GoalCanvas.Inst.CheckpointToFollow = Util.WaypointName.MotorWheel;
		// GameSave.CurrentCheckpoint = Util.WaypointName.PreStory2;
		// GoToCheckpointAsync(Util.WaypointName.PreStory2);
		
		// BackButton.Inst.Hide();
		yield return new WaitForSeconds(0.5f);
		yield return LineCanvas.Top.DisplayLineAndWaitForClick("Shirley", "Oh, no! We turned over!", null);
		RebuildButton.Inst.Show();
		RebuildButton.Inst.StartScale();
		yield return LineCanvas.Top.DisplayLineAndWaitForEvent("Shirley", "Press the wrench icon to rebuild.", (RebuildButtonClickedEvent e) => true);
		LineCanvas.Top.Hide();
		//Character.Piggy.WarpTo(TRef.Get(Util.TRefName.PigPrestory2));
		//Character.Partner.WarpTo(TRef.Get(Util.TRefName.PartnerPrestory2));
		//MainCamera.Inst.WarpTo(TRef.Get(Util.TRefName.CameraPrestory2_1));
		//yield return LineCanvas.Bottom.DisplayLineAndWaitForClick("Shirley", "Awww. That hurts!", Character.Partner);
		//yield return MainCamera.Inst.Transition(TRef.Get(Util.TRefName.CameraPrestory2_1), TRef.Get(Util.TRefName.CameraPrestory2_2), 1.0f);
		//yield return LineCanvas.Bottom.DisplayLineAndWaitForClick("You", "Yes.", Character.Piggy);
		//yield return MainCamera.Inst.Transition(TRef.Get(Util.TRefName.CameraPrestory2_2), TRef.Get(Util.TRefName.CameraPrestory2_1), 1.0f);
		//yield return LineCanvas.Bottom.DisplayLineAndWaitForClick("Shirley", "A car without control is like the West without Jerusalem.", Character.Partner);
		//yield return AtTheSameTime(
		//	LineCanvas.Bottom.DisplayLineAndWaitForClick("Shirley", "Fortunately, there is a garage nearby that stores what we want.", Character.Partner),
		//	MainCamera.Inst.Transition(TRef.Get(Util.TRefName.CameraPrestory2_1), TRef.Get(Util.TRefName.CameraPrestory2_3), 1.5f)
		//	);
		//yield return AtTheSameTime(
		//	LineCanvas.Bottom.DisplayLineAndWaitForClick("Shirley", "The turning wheels and the motor wheels.", Character.Partner),
		//	MainCamera.Inst.Transition(TRef.Get(Util.TRefName.CameraPrestory2_3), TRef.Get(Util.TRefName.CameraPrestory2_4), 1.5f)
		//	);
		//yield return MainCamera.Inst.Transition(TRef.Get(Util.TRefName.CameraPrestory2_4), TRef.Get(Util.TRefName.CameraPrestory2_1), 1.5f);
		//yield return LineCanvas.Bottom.DisplayLineAndWaitForClick("Shirley", "With them, we can steer the car easily.", Character.Partner);

		//yield return LineCanvas.Bottom.DisplayLineAndWaitForClick("Shirley", "This time, would you like to try it yourself?", Character.Partner);
		//ChoiceCanvas.Inst.DisplayChoices(new() { ("Let me try it!", Util.ChoiceName.DontNeedHelp), ("I need help!", Util.ChoiceName.NeedHelp) });
		//Util.ChoiceObj choice_obj = new();
		//last_choice_name = choice_obj.choice_name;
		//yield return WaitForChoice(choice_obj);
		//// choice_name = choice_obj.choice_name;
		//if (choice_obj.choice_name == Util.ChoiceName.DontNeedHelp)
		//{
		//	yield return LineCanvas.Bottom.DisplayLineAndWaitForClick("Shirley", "I admire your courage. Good luck!", Character.Partner);
		//}
		//else
		//{
		//	yield return LineCanvas.Bottom.DisplayLineAndWaitForClick("Shirley", "I admire your modesty. Let's figure it out together.", Character.Partner);
		//}
		//Character.Piggy.WarpTo(TRef.Get(Util.TRefName.Origin));
		//Character.Partner.WarpTo(TRef.Get(Util.TRefName.Origin));
		//LineCanvas.Bottom.Hide();
		//Util.BuildInfo build_info = choice_obj.choice_name == Util.ChoiceName.DontNeedHelp? Util.BuildInfo.DontNeedHelp: Util.BuildInfo.NeedHelp;
		//TransitionToBuild(Util.WaypointName.PreStory2, Util.GoalName.PreStory2, build_info);
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
		// Util.WaypointName.PreStory2
	};

	IEnumerator TransitionToFirstBuild()
	{
		Test(() =>
		{
			Debug.Assert(GameSave.GridMemories[0].MemCrates.GetLength(0) == 2);
			Debug.Assert(GameSave.GridMemories[0].MemCrates.GetLength(1) == 2);
			Debug.Assert(GameSave.GridMemories[0].MemCrates.GetLength(2) == 3);
			GameSave.GridMemories[0].Clear();
			GameSave.GridMemories[0].MemCrates[0, 0, 0] = Util.Component.WoodenCrate;
			GameSave.GridMemories[0].MemCrates[1, 0, 0] = Util.Component.WoodenCrate;
			GameSave.GridMemories[0].MemCrates[1, 1, 0] = Util.Component.WoodenCrate;
			GameSave.GridMemories[0].MemCrates[1, 1, 1] = Util.Component.WoodenCrate;
			GameSave.GridMemories[0].MemAccessories[1, 0, 1] = Util.Component.Wheel;
			GameSave.GridMemories[0].MemAccessories[1, 1, 2] = Util.Component.Wheel;
			GameSave.GridMemories[0].AccessoryDirections[1, 0, 1] = 1;
			GameSave.GridMemories[0].AccessoryDirections[1, 1, 2] = 2;
		});
		
		yield return null;

		yield return GoToCheckpoint(Util.WaypointName.PreStory1, true);
		Test(() =>
		{
			DragImage.Current = null;
			// GridMatrix.DeselectGridMatrix();
			// GridMatrix.SelectGridMatrix(waypoint_name, build_info != Util.BuildInfo.NeedHelp);
			Goal.Activate(Util.GoalName.PreStory1);
			// MainCamera.Inst.MoveAndStickToGridMatrix(0.5f, 0.5f, 0.5f);
			PiggyPermitInvisible = false;
			StartCoroutine(Prestory1Build());
		});
	}
	//void TransitionToBuild(Util.WaypointName waypoint_name, Util.GoalName goal_name, Util.BuildInfo build_info = Util.BuildInfo.NeedHelp)
	//{
	//	AudioPlayer.Inst.TransitionToStory();
	//	current_waypoint = waypoint_name;
	//	retry_waypoint = waypoint_name;
	//	retry_goal = goal_name;
	//	//if (can_retry_waypoints.Contains(waypoint_name))
	//	//{
			
	//	//}
	//	//else
	//	//{
	//	//	// waypoint_name = Util.WaypointName.None;
	//	//	retry_waypoint = Util.WaypointName.None;
	//	//	retry_goal = Util.GoalName.None;
	//	//}
	//	BuildCanvas.Inst.Show();
	//	BuildCanvas.Inst.InitializeItems();
	//	GridMatrix.Inst.ActivateAsync();
	//	GridMatrix.Inst.MoveToCheckpoint(Util.WaypointName.PreStory1);
	//	PlayCanvas.Inst.Hide();
	//	// AudioPlayer.Inst.TransitionToBuild();
	//	DestroyComponentsInScene();		
	//	DragImage.Current = null;
	//	GridMatrix.DeselectGridMatrix();
	//	GridMatrix.SelectGridMatrix(waypoint_name, build_info != Util.BuildInfo.NeedHelp);
	//	if (goal_name != Util.GoalName.None)
	//	{
	//		Goal.Select(goal_name);
	//	}
	//	// MainCamera.Inst.MoveAndStickToGridMatrix(0.5f, 0.5f, 0.5f);
	//	PiggyPermitInvisible = false;
	//	// PiggyCameraPivot.Inst.EndFollow();

	//	switch(waypoint_name)
	//	{
	//		case Util.WaypointName.PreStory1:
	//			StartCoroutine(Prestory1Build());
	//			break;
	//		//case Util.WaypointName.PreStory2:
	//		//	StartCoroutine(Prestory2Build(build_info));
	//		//	break;
	//		//case Util.WaypointName.Town:
	//		//	if (!town_waypoint_met)
	//		//	{
	//		//		town_waypoint_met = true;
	//		//		StartCoroutine(TownWaypointBuild());
	//		//	}
	//		//	break;
	//		//case Util.WaypointName.Volcano:
	//		//	StartCoroutine(VolcanoBuild());
	//		//	break;
	//		//case Util.WaypointName.VolcBottom:
	//		//	StartCoroutine(VolcBottomBuild());
	//		//	break;
	//		//case Util.WaypointName.VolcTop:
	//		//	StartCoroutine(VolcTopBuild());
	//		//	break;
	//	}
	//}
	IEnumerator VolcanoBuild()
	{
		yield return new WaitForSeconds(1.5f);
		yield return LineCanvas.Top.DisplayLineAndWaitForClick("Shirley", "The volcano is too tall, so we have to take a detour.", null);
		yield return LineCanvas.Top.DisplayLineAndWaitForClick("Shirley", "The destination is deep, so be prepared.", null);
		LineCanvas.Top.Hide();
	}
	IEnumerator VolcBottomBuild()
	{
		yield return new WaitForSeconds(1.5f);
		yield return LineCanvas.Top.DisplayLineAndWaitForClick("Shirley", "The incline is too steep. We can't make it with only wheels.", null);
		LineCanvas.Top.Hide();
	}
	IEnumerator VolcTopBuild()
	{
		yield return new WaitForSeconds(1.5f);
		yield return LineCanvas.Top.DisplayLineAndWaitForClick("Shirley", "Let's take a leap of faith.", null);
		LineCanvas.Top.Hide();
	}
	bool town_waypoint_met = false;
	IEnumerator Prestory1Build()
	{
		yield return new WaitForSeconds(1.5f);

		MapImage.Inst.Hide();
		RebuildButton.Inst.Hide();
		Retry.Inst.Hide();

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
		// GridMatrix.Current.ShowDesign();

		// force design
		GridMatrix.Inst.ShowDesign(Util.DesignPrestory1());
		GridMatrix.Inst.ForceDesign = true;
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
						// TransitionToBuild(Util.WaypointName.PreStory2, Util.GoalName.PreStory2, Util.BuildInfo.NeedHelp);
						yield break;
					}
				}
				break;
		}
	}
	public IEnumerator TownWaypointBuild()
	{
		ConfirmButton.Inst.EnableConfirm = false;
		yield return new WaitForSeconds(1.5f);

		yield return LineCanvas.Top.DisplayLineAndWaitForEvent("Shirley", "**Drag the screen to view the grid**", (CanvasDragEvent e) => true);
		yield return LineCanvas.Top.DisplayLineAndWaitForClick("Shirley", "Perfect! Now there's an **important** feature that you want to learn", null);
		yield return LineCanvas.Top.DisplayLineAndWaitForEvent("Shirley", "**Press \"Space\" to toggle build layers.**", (SwitchLayerEvent e) => true);
		yield return LineCanvas.Top.DisplayLineAndWaitForClick("Shirley", "Perfect! Without previous design constraints, it would be hard to locate a cell without specifying layers.", null);
		yield return LineCanvas.Top.DisplayLineAndWaitForEvent("Shirley", "Now, press \"Space\" a few more times to go back to the full layer mode.", (FullLayerEvent e) => true);
		yield return LineCanvas.Top.DisplayLineAndWaitForClick("Shirley", "Awesome! Feel free to explore the world!", null);
		LineCanvas.Top.Hide();
		Goal.Activate(Util.GoalName.C1S1);
		Character.Partner.WarpTo(TRef.Get(Util.TRefName.PartnerC1S1));
		ConfirmButton.Inst.EnableConfirm = true;
		yield break;
	}
	void OnGoalReached(GoalReachedEvent e)
	{
		// Debug.LogError("Deprecated");
		switch (e.goal_name)
		{
			case Util.GoalName.PreStory1:
				TransitionToStory(Util.StoryName.Intro);
				break;
			case Util.GoalName.FallOffCliff:
				TransitionToStory(Util.StoryName.FallOffCliff);
				break;
			//case Util.GoalName.TurnWheel:
			//	GameSave.CurrentCheckpoint = Util.WaypointName.TurnWheel;
			//	GameSave.Inventory[Util.Component.TurnWheel] = 2;
			//	GameSave.IncrementGridSize(0, 1, 0);
			//	ObtainCanvas.Inst.Show(Util.Component.TurnWheel);
			//	Goal.Activate(GoalName.Umbrella);
			//	GoalCanvas.Inst.GoalToFollow = Util.GoalName.Umbrella;
			//	break;
			//case Util.GoalName.Umbrella:
			//	GameSave.CurrentCheckpoint = WaypointName.Umbrella;
			//	GameSave.Inventory[Util.Component.Umbrella] = 4;
			//	GameSave.IncrementGridSize(1, 0, 0);
			//	ObtainCanvas.Inst.Show(Util.Component.Umbrella);
			//	Goal.Activate(GoalName.Rocket);
			//	GoalCanvas.Inst.GoalToFollow = Util.GoalName.Rocket;
			//	break;
			//case Util.GoalName.PreStory2:
			//	TransitionToStory(Util.StoryName.InTown);
			//	break;
			//case Util.GoalName.Town:
			//	TransitionToStory(Util.StoryName.TownWaypoint);
			//	break;
			//case Util.GoalName.C1S1:
			//	TransitionToStory(Util.StoryName.C1S1);
			//	break;
			//case Util.GoalName.C1S2:
			//	TransitionToStory(Util.StoryName.C1S2);
			//	break;
			//case Util.GoalName.Volcano:
			//	TransitionToBuild(Util.WaypointName.Volcano, Util.GoalName.VolcBottom);
			//	break;
			//case Util.GoalName.VolcBottom:
			//	TransitionToBuild(Util.WaypointName.VolcBottom, Util.GoalName.VolcTop);
			//	break;
			//case Util.GoalName.VolcTop:
			//	TransitionToBuild(Util.WaypointName.VolcTop, Util.GoalName.VolcAfter);
			//	break;
			//case Util.GoalName.VolcAfter:
			//	StartCoroutine(HandleVolcAfter());
			//	break;
			default:
				Debug.LogError("Goal reached not handled");
				break;
		}
	}
	void OnTouchLava(TouchLavaEvent e)
	{
		StartCoroutine(TouchLavaHelper());
	}
	IEnumerator TouchLavaHelper()
	{
		yield return BlackoutCanvas.Inst.Blackout(0.5f, 0.0f, 1.0f);
		yield return BlackoutCanvas.Inst.DisplaySub("You tried to swim in lava.", 0.5f, 0.0f, 1.0f);
		OnRetry();
		yield return BlackoutCanvas.Inst.DisplaySub("You tried to swim in lava.", 0.5f, 1.0f, 0.0f);
		yield return BlackoutCanvas.Inst.Blackout(0.5f, 1.0f, 0.0f);
	}
	IEnumerator HandleVolcAfter()
	{
		yield return BlackoutCanvas.Inst.Blackout(0.5f, 0.0f, 1.0f);
		yield return BlackoutCanvas.Inst.DisplaySub("Thanks for playing", 1.5f, 0.0f, 1.0f);
		yield return BlackoutCanvas.Inst.DisplaySub("More Contents Coming Soon", 1.5f, 1.0f, 0.0f);
		yield return BlackoutCanvas.Inst.Blackout(0.5f, 1.0f, 0.0f);
	}
	Util.WaypointName current_waypoint;


	public void TryScan()
	{
		CarCore.Inst.DeactivateContainer();
		GridMatrix.Inst.Scan();
	}

	public void TransitionToPlay(bool build)
	{
		AudioPlayer.Inst.TransitionToPlay();
		BuildCanvas.Inst.Hide();
		PlayCanvas.Inst.Show();
		
		// AudioPlayer.Inst.TransitionToPlay();
		if (build)
		{
			StartCoroutine(GridMatrix.Inst.BuildAndDeactivate());
		}
		// PiggyCameraPivot.Inst.StartFollow(Piggy);
		// coroutine that moves camera to position
		// MainCamera.Inst.MoveAndStickToPig(move_to_pig_time, camera_rotation_time);
		//if (build)
		//{
		//	switch (current_waypoint)
		//	{
		//		case Util.WaypointName.PreStory1:
		//			FirstPerson.Inst.Hide();
		//			Retry.Inst.Hide();
		//			break;
		//		case Util.WaypointName.PreStory2:
		//			StartCoroutine(PlayPreStory2());
		//			break;
		//	}
		//}
	}
	public IEnumerator PlayPreStory2()
	{
		yield return new WaitForSeconds(1.0f);
		Retry.Inst.Show();
		FirstPerson.Inst.Show();
		yield return LineCanvas.Bottom.DisplayLineAndWaitForClick("Shirley", "Drive straight into the winding valley. That's the shortest path.", null);
		LineCanvas.Bottom.Hide();
	}
}
