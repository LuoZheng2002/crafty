using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ViewCanvasDrag : MonoBehaviour
{
	public GameObject buildCanvas;
	public float rotationSpeed = 0.05f;  // Speed of rotation
	public float vertical_speed = 1.0f;
	public float horizontal_speed = 1.0f;
	public float mouseSensitivity = 100f;
	private void Start()
	{
		Cursor.lockState = CursorLockMode.None;
	}
	IEnumerator MoveBack()
	{
		float time = Time.time;
		while (Time.time - time < 0.5f)
		{
			Vector3 deltaPos = -Camera.main.transform.forward * 10.0f * Time.deltaTime;
			deltaPos += new Vector3(0, 5.0f, 0) * Time.deltaTime;
			Camera.main.transform.position += deltaPos;
			yield return null;
		}
	}
	private void OnEnable()
	{
		buildCanvas.SetActive(false);
		StartCoroutine(MoveBack());
		Cursor.lockState = CursorLockMode.Locked;
	}
	private void Update()
	{
		float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
		float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
		Camera.main.transform.eulerAngles += new Vector3(-mouseY, mouseX, 0);

		if (Input.GetKey(KeyCode.RightShift) || Input.GetKey(KeyCode.LeftShift))
        {
			Camera.main.transform.position += new Vector3(0, -vertical_speed * Time.deltaTime, 0);
        }
		if (Input.GetKey(KeyCode.Space))
		{
			Camera.main.transform.position += new Vector3(0, vertical_speed * Time.deltaTime, 0);
		}
		float h_axis = Input.GetAxis("Horizontal");
		float v_axis = Input.GetAxis("Vertical");
		Vector3 deltaPosition = Camera.main.transform.right * h_axis * horizontal_speed * Time.deltaTime;
		deltaPosition += Camera.main.transform.forward * v_axis * horizontal_speed * Time.deltaTime;
		Camera.main.transform.position += deltaPosition;
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			Debug.LogError("Deprecated!");
			buildCanvas.SetActive(true);
			gameObject.SetActive(false);
			Cursor.lockState = CursorLockMode.None;
			CustomCursor.Inst.SetIdleCursor();
		}
    }
}
