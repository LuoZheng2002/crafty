using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ChoiceSelectedEvent
{
    public Util.ChoiceName choice_name;
    public ChoiceSelectedEvent(Util.ChoiceName choice_name)
    {
        this.choice_name = choice_name;
    }
}
public class Choice : MonoBehaviour
{
    public Util.ChoiceName choice_name;
    public Text text;
    public void OnClick()
    {
        EventBus.Publish(new ChoiceSelectedEvent(choice_name));
        ChoiceCanvas.Inst.Hide();
    }
    public void SetText(string t)
    {
        text.text = t;
    }
}
