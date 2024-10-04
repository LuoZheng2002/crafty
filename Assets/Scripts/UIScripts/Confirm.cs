using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Confirm : MonoBehaviour
{
    public void OnConfirmClicked()
    {
        EventBus.Publish(new GameStateChangedEvent(Util.GameStateType.Play, 0));
    }
}
