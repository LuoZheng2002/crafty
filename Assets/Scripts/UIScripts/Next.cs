using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextEvent
{

}
public class Next : MonoBehaviour
{
    public void OnNext()
    {
        EventBus.Publish(new NextEvent());
    }
}
