using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageContainer : MonoBehaviour
{
    public static ImageContainer Inst
    {
        get { Debug.Assert(inst != null, "Image Container is not set"); return inst; }
    }
    static ImageContainer inst;
    // Start is called before the first frame update
    public RectTransform RectTransform { get; private set; }
    void Start()
    {
        RectTransform = GetComponent<RectTransform>();
        Debug.Assert(inst == null, "Image Container already set");
        inst = this;
    }
	private void OnDestroy()
	{
        inst = null;
	}
}
