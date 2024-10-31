using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildCanvas : MonoBehaviour
{
    public static BuildCanvas Inst
    {
        get { Debug.Assert(inst != null, "Build Canvas not set");return inst; }
    }
    static BuildCanvas inst;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(inst == null, "Build Canvas Already Set");
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
