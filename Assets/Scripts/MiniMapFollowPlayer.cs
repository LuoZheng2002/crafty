using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapFollowPlayer : MonoBehaviour
{ 

    void LateUpdate()
    {
        if (GameState.Inst.Piggy == null)
        {
            Debug.Log("Pig doesn't exist yet!");
        }
        else
        {
            Transform player = GameState.Inst.Piggy.transform;
            Vector3 newPosition = player.position;
            //newPosition.y = transform.position.y;
            transform.position = newPosition;
            //transform.rotation = Quaternion.Euler(90f, player.eulerAngles.y, 0f);
        }
    }

}
