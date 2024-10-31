using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Retry : MonoBehaviour
{
	public void OnRetry()
    {
		GameState.Inst.Retry();
		GameState.shown_retry = true;
    }
}
