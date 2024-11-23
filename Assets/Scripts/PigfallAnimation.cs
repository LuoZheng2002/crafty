using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PigfallAnimationEndEvent
{

}

public class PigfallAnimation : MonoBehaviour
{
    public void OnAnimationEnd()
    {
        EventBus.Publish(new PigfallAnimationEndEvent());
        Destroy(gameObject);
	}
}
