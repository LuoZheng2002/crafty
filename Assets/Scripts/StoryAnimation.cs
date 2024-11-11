using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryAnimation : MonoBehaviour
{
	public bool CanSpeedup { get; set; } = true;
	static StoryAnimation inst;
	public Camera StoryCamera { get; private set; }
	public static StoryAnimation Inst
	{
		get { Debug.Assert(inst != null, "Story Animation not set"); return inst; }
	}
	Animator animator;
	private void Start()
	{
		Debug.Assert(inst == null, "Story Animation already set");
		inst = this;
		animator = GetComponent<Animator>();
		Debug.Assert(animator != null);
		animator.enabled = false;
		StoryCamera = transform.Find("StoryCamera").GetComponent<Camera>();
		Debug.Assert(StoryCamera != null);
		gameObject.SetActive(false);
	}
	private void OnDestroy()
	{
		inst = null;
	}
	public void PlayAnimation(Util.StoryName storyName)
	{
		gameObject.SetActive(true);
		animator.enabled = true;
		animator.speed = 1.0f;
		// MainCamera.Inst.FollowStory();
		switch (storyName)
		{
			case Util.StoryName.Crash:
				animator.Play("crash");
				break;
			case Util.StoryName.FallOffCliff:
				animator.Play("cliff");
				break;
			case Util.StoryName.InTown:
				animator.Play("town");
				break;
			case Util.StoryName.TownWaypoint:
				animator.Play("townwaypoint");
				break;
			case Util.StoryName.C1S2:
				animator.Play("groundhog");
				break;
			default:
				Debug.LogError("Animation not set!");
				break;
		}
	}
	public void WaypointChangeToGreen(Util.WaypointName waypoint_name)
	{
		Waypoint.Waypoints[waypoint_name].ChangeToGreen();
	}
	Action func;
	public void RegisterEndAnimationFunc(Action func)
	{
		this.func = func;
	}
	public void EndAnimation()
	{
		gameObject.SetActive(false);
		animator.enabled = false;
		LineCanvas.Bottom.Hide();
		if (func != null)
		{
			func();
			func = null;
		}
		EventBus.Publish(new AnimationExitEvent());
	}
	public void Pause()
	{
		animator.speed = 0;
		paused = true;
		LineCanvas.Bottom.ShowContinue();
	}
	bool paused = false;
	private void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			if (paused)
			{
				paused = false;
				animator.speed = 1;
				LineCanvas.Bottom.HideContinue();
			}
			else if (CanSpeedup)
			{
				animator.speed = 4;
			}
		}
	}
}
