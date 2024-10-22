using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ShowNewTutorialEvent
{
    public Util.NewTutorialType newTutorialType;
    public ShowNewTutorialEvent(Util.NewTutorialType newTutorialType)
    {
        this.newTutorialType = newTutorialType;
    }
}
public class OpenNewTutorialEvent
{

}
public class NewTutorialPlayer : MonoBehaviour
{
	public Sprite image0;
	public Sprite image1;
	public Sprite image2;
	public Sprite image3;
	public Sprite image4;
	int current_index = 0;
	GameObject wrapper;
	Image ok;
	Image image;
	bool visible = false;
	List<Sprite> images;
	private void Start()
	{
		wrapper = transform.GetChild(0).gameObject;
		wrapper.SetActive(false);
		EventBus.Subscribe<ShowNewTutorialEvent>(OnShowNewTutorial);
		EventBus.Subscribe<OpenNewTutorialEvent>(OnOpenNewTutorial);
		ok = transform.Find("Wrapper").Find("OK").GetComponent<Image>();
		image = transform.Find("Wrapper").Find("Panel").Find("Image").GetComponent<Image>();
		Debug.Assert(ok != null);
		Debug.Assert(image != null);
		images = new() { image0, image1, image2, image3, image4 };
		image.sprite = image0;
	}

	void OnShowNewTutorial(ShowNewTutorialEvent e)
	{
		if (!visible)
		{
			wrapper.SetActive(true);
			visible = true;
		}
		ok.enabled = true;
		switch (e.newTutorialType)
		{
			case Util.NewTutorialType.Build:
				current_index = 0;
				break;
			case Util.NewTutorialType.Wide:
				current_index = 1;
				break;
			case Util.NewTutorialType.Turn:
				current_index = 2;
				break;
			case Util.NewTutorialType.Power:
				current_index = 3;
				break;
			case Util.NewTutorialType.Momentum:
				current_index = 4;
				break;
		}
		image.sprite = images[current_index];
	}
	public void LeftClicked()
	{
		current_index = (current_index + 3) % 5;
		image.sprite = images[current_index];
	}
	public void RightClicked()
	{
		current_index = (current_index + 1) % 5;
		image.sprite = images[current_index];
	}
	public void OnOKClicked()
	{
		wrapper.SetActive(false);
		visible = false;
	}
	public void OnCrossClicked()
	{
		wrapper.SetActive(false);
		visible = false;
	}

	void OnOpenNewTutorial(OpenNewTutorialEvent e)
	{
		if (!visible)
		{
			wrapper.SetActive(true);
			visible = true;
		}
		ok.enabled = false;
	}
}
