using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoiceCanvas : MonoBehaviour
{
    static ChoiceCanvas inst;
    public static ChoiceCanvas Inst
    {
        get { Debug.Assert(inst != null); return inst; }
    }
	Transform choices_transform;
	public Choice choice_prefab;
	private void Start()
	{
		Debug.Assert(inst == null); 
        inst = this;
		gameObject.SetActive(false);
		choices_transform = transform.Find("Choices");
		Debug.Assert(choices_transform != null);
	}
	private void OnDestroy()
	{
		inst= null;	
	}
	public void DisplayChoices(List<(string text, Util.ChoiceName choice_name)> choices)
	{
		gameObject.SetActive(true);
		for (int i = 0; i < choices_transform.childCount; i++)
		{
			Destroy(choices_transform.GetChild(i).gameObject);
		}
		foreach(var choice in choices)
		{
			var choice_inst = Instantiate(choice_prefab.gameObject, choices_transform);
			Choice c = choice_inst.GetComponent<Choice>();
			Debug.Assert(c != null);
			c.choice_name = choice.choice_name;
			c.SetText(choice.text);
		}
	}
	public void Hide()
	{
		gameObject.SetActive(false);
	}
}
