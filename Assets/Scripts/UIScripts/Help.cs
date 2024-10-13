using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenTutorialEvent
{

}
public class Help : MonoBehaviour
{
    public void OpenTutorial()
    {
        EventBus.Publish(new OpenTutorialEvent());
    }
}
