using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryAnimation : MonoBehaviour
{
	public bool CanSpeedup { get; set; } = true;
	static StoryAnimation inst;
	public static StoryAnimation Inst
	{
		get { Debug.Assert(inst != null, "Story Animation not set"); return inst; }
	}
	Animator animator;
	public Transform AnimationCamera { get; private set; }
	private void Start()
	{
		Debug.Assert(inst == null, "Story Animation already set");
		inst = this;
		animator = GetComponent<Animator>();
		Debug.Assert(animator != null);
		animator.enabled = false;
		AnimationCamera = transform.Find("AnimationCamera");
		Debug.Assert(AnimationCamera != null);
		Camera camera = AnimationCamera.GetComponent<Camera>();
		camera.enabled = false;
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
		MainCamera.Inst.FollowStory();
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
				animator.speed = 10;
			}
		}
	}
	public void SetLine(string line)
	{
		string[] strings = line.Split('@');
		if (strings.Length >=2)
		{
			LineCanvas.Bottom.Name = strings[0];
			LineCanvas.Bottom.Line = strings[1];
		}
		else
		{
			LineCanvas.Bottom.Line = line;
		}
	}
	public void BlackOutBlack(float time)
	{
		BlackoutCanvas.Inst.Blackout(time, true);
	}
	public void BlackOutWhite(float time)
	{
		BlackoutCanvas.Inst.Blackout(time, false);
	}
}
