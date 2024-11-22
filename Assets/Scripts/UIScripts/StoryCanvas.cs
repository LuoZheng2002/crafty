using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OtherStoryClickedEvent
{

}

public class StoryCanvas : MonoBehaviour
{
    static StoryCanvas inst;
	public Util.QuestName current_quest_name = Util.QuestName.None;
	public static StoryCanvas Inst
	{
		get
		{
			Debug.Assert(inst != null);
			return inst;
		}
	}
	private void Start()
	{
		Debug.Assert(inst == null);
		inst = this;
		EventBus.Subscribe<OtherStoryClickedEvent>(OnOtherStoryClicked);
		gameObject.SetActive(false);
	}
	void OnOtherStoryClicked(OtherStoryClickedEvent e)
	{
		main_story_background.color = light_gray_color;
	}
	public void Show()
	{
		gameObject.SetActive(true);
	}
	private void OnDestroy()
	{
		inst = null;
	}
	public void OnConfirmClicked()
	{
		gameObject.SetActive(false);
		GameSave.CurrentQuestName = current_quest_name;
	}
	public void OnBackClicked()
	{
		gameObject.SetActive(false);
	}
	public Image main_story_background;
	public static Color green_color = new Color(0.2f, 1.0f, 0.2f, 1);
	public static Color light_gray_color = new Color(0.8f, 0.8f, 0.8f, 0.8f);
	public void OnMainStoryClicked()
	{
		EventBus.Publish(new OtherStoryClickedEvent());
		main_story_background.color = green_color;
	}
}
