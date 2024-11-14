using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapPigPlayer : MonoBehaviour
{
    void LateUpdate()
    {
        if (GameState.Inst.Piggy == null)
        {
            Debug.Log("Pig UI doesn't exist yet!");
        }
        else
        {
            Transform player = GameState.Inst.Piggy.transform;
            transform.localRotation = Quaternion.Euler(0, 0, -player.eulerAngles.y);
        }
    }
}
