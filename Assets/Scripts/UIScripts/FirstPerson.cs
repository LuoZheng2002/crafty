using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPerson : MonoBehaviour
{
    public bool first_person = true;
    public void ToggleFirstPerson()
    {
        first_person = !first_person;
        EventBus.Publish(new FirstPersonChangedEvent(first_person));
    }
}
