using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PresetButtonClickedEvent
{

}
public class PresetButton : MonoBehaviour
{
    static PresetButton inst;
    public Image image;
    ButtonScale button_scale;
    public static PresetButton Inst
    {
        get { Debug.Assert(inst != null); return inst; }
    }

    private void Start()
    {
        Debug.Assert(inst == null);
        inst = this;
        button_scale = GetComponent<ButtonScale>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        inst = null;
    }

    public void StartScale()
    {
        button_scale.ScaleStart();
    }

    public void OnClick()
    {
        button_scale.ScaleStop();

        EventBus.Publish(new PresetButtonClickedEvent());
    }
}
