using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class RetryEvent
{

}

public class Retry : MonoBehaviour
{
    public void OnRetry()
    {
        EventBus.Publish(new RetryEvent());
    }
}
