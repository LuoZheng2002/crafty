using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class TrashEvent
{

}
public class Trash : MonoBehaviour
{
    public void OnImageClick()
    {
        EventBus.Publish(new TrashEvent());
    }
}
