using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EraserImage : MonoBehaviour
{
    ButtonScale buttonScale;
    static EraserImage inst;
    public static EraserImage Inst
    {
        get { Debug.Assert(inst != null, "Eraser Image not set"); return inst; }
    }
	private void Start()
	{
        Debug.Assert(inst == null, "Eraser Image already set");
        inst = this;
        buttonScale = GetComponent<ButtonScale>();
        Debug.Assert(buttonScale != null);
	}
    public void StartScale()
    {
        buttonScale.ScaleStart();
    }
    public void EndScale()
    {
        buttonScale.ScaleStop();
    }
}
