using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroCamera : MonoBehaviour
{
    static IntroCamera inst;
    public static IntroCamera Inst
    {
        get { Debug.Assert(inst != null); return inst; }
    }
	private void Start()
	{
		Debug.Assert(inst == null);
        inst = this;
	}
}
