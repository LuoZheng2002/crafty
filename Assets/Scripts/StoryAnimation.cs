using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryAnimation : MonoBehaviour
{
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
		AnimationCamera = transform.Find("AnimationCamera");
		Debug.Assert(AnimationCamera != null);
	}
	private void OnDestroy()
	{
		inst = null;
	}
	public void PlayAnimation(Util.StoryName storyName)
	{
		switch (storyName)
		{
			case Util.StoryName.Crash:
				animator.SetTrigger("prestory1");
				break;
		}
	}
}
