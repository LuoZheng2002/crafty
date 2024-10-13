using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAnimation : MonoBehaviour
{
    public float speedup_speed = 5.0f;
    void Start()
    {
        Transform mainCamera = transform.Find("Main Camera");
        Debug.Assert(mainCamera != null);
        Destroy(mainCamera.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            Animator animator = GetComponent<Animator>();
            animator.speed = speedup_speed;
        }
    }
    public void Detach()
    {
		Transform mainCamera = transform.GetChild(0);
		mainCamera.parent = null;
		Destroy(gameObject);
		EventBus.Publish(new AnimationExitEvent());
	}
}
