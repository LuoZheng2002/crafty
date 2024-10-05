using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAnimation : MonoBehaviour
{
    void Start()
    {
        Transform mainCamera = transform.Find("Main Camera");
        Debug.Assert(mainCamera != null);
        Destroy(mainCamera.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Detach()
    {
		Transform mainCamera = transform.GetChild(0);
		mainCamera.parent = null;
		Destroy(gameObject);
		EventBus.Publish(new AnimationExitEvent());
	}
}
