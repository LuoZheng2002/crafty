using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JointBreak : MonoBehaviour
{
	private void OnJointBreak(float breakForce)
	{
		Debug.Log("A joint has just been broken!, force: " + breakForce);
		RebuildButton.Inst.CarBroken = true;
	}
}
