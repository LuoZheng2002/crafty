using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationExitEvent
{

}
public class CameraAnimExit : StateMachineBehaviour
{
	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		Transform mainCamera = animator.transform.GetChild(0);
		mainCamera.parent = null;
		Destroy(animator.gameObject);
		EventBus.Publish(new AnimationExitEvent());
	}
}
