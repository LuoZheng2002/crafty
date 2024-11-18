using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapFollowPlayer : MonoBehaviour
{ 

    void LateUpdate()
    {
        //if (GameState.Inst.Piggy == null)
        //{
        //    Debug.Log("Pig doesn't exist yet!");
        //}
        //else
        //{
        //    Transform player = GameState.Inst.Piggy.transform;
        //    Vector3 newPosition = player.position;
        //    //newPosition.y = transform.position.y;
        //    transform.position = newPosition;
        //    //transform.rotation = Quaternion.Euler(90f, player.eulerAngles.y, 0f);
        //}
        transform.position = CarCore.Inst.transform.position;
        Vector3 front_dir = CarCore.Inst.transform.forward;
		front_dir.y = 0;
        transform.rotation = Quaternion.LookRotation(front_dir, new Vector3(0, 1, 0));
	}

}
