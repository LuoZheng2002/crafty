using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayCanvas : MonoBehaviour
{
    static PlayCanvas inst;
    public static PlayCanvas Inst
    {
        get { Debug.Assert(inst != null, "Play Canvas not set"); return inst; }
    }
	private void Start()
	{
        Debug.Assert(inst == null, "Play canvas already set");
        inst = this;
        Util.Delay(this, () =>
        {
            gameObject.SetActive(false);
        });        
	}
    public void Show()
    {
        gameObject.SetActive(true);
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
