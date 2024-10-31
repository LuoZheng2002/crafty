using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Replay : MonoBehaviour
{
    public void OnReplay()
    {
        Debug.LogError("Deprecated!");
        // GameState.Inst.TransitionToIntro();
    }
}
