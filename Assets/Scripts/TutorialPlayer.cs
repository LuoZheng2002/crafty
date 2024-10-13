using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialPlayer : MonoBehaviour
{
    // Start is called before the first frame update
    GameObject wrapper;
    Image ok;
    bool visible = false;
    Animator animator;
    int current_index = 0;
    static List<string> tutorials = new() { "space", "drag", "dragscreen", "thirdperson" };
    static List<string> texts = new() { "Press Space", "Drag!", "Drag!", "Third Person" };
    Text text;
    void Start()
    {
        wrapper = transform.GetChild(0).gameObject;
        EventBus.Subscribe<ShowTutorialEvent>(OnShowTutorial);
        EventBus.Subscribe<OpenTutorialEvent>(OnOpenTutorial);
        ok = transform.Find("Wrapper").Find("OK").GetComponent<Image>();
        Debug.Assert(ok != null);
        animator = GetComponent<Animator>();
        text = transform.Find("Wrapper").Find("Panel").Find("Image").Find("Text").GetComponent<Text>();
        Debug.Assert(text != null);
		animator.SetTrigger(tutorials[current_index]);
		text.text = texts[current_index];
	}

    void OnOpenTutorial(OpenTutorialEvent e)
    {
		if (!visible)
		{
			wrapper.SetActive(true);
			visible = true;
		}
        ok.enabled = false;
		animator.SetTrigger(tutorials[current_index]);
		text.text = texts[current_index];
	}
    void OnShowTutorial(ShowTutorialEvent e)
    {
        if (!visible)
        {
            wrapper.SetActive(true);
            visible = true;
        }
        ok.enabled = true;
        switch (e.tutorialType)
        {
            case Util.TutorialType.Space:
                animator.SetTrigger("space");
                current_index = 0;
                break;
            case Util.TutorialType.Drag:
                animator.SetTrigger("drag");
                current_index = 1;
                break;
            case Util.TutorialType.DragScreen:
                animator.SetTrigger("dragscreen");
                current_index = 2;
                break;
            case Util.TutorialType.ThirdPerson:
                animator.SetTrigger("thirdperson");
                current_index = 3;
                break;
        }
		text.text = texts[current_index];
	}
    public void LeftClicked()
    {
        current_index = (current_index + 3) % 4;
        animator.SetTrigger(tutorials[current_index]);
		text.text = texts[current_index];
	}
    public void RightClicked()
    {
        current_index = (current_index + 1) % 4;
        animator.SetTrigger(tutorials[current_index]);
		text.text = texts[current_index];
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
    // Update is called once per frame
    void Update()
    {
        
    }
}
