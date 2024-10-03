using UnityEngine;

public class DragLook : MonoBehaviour
{
	public float rotationSpeed = 0.2f;  // Speed of rotation
	private Vector3 lastMousePosition;  // Store the last mouse position

	void Update()
	{
		if (Input.GetMouseButtonDown(0))  // Check if the left mouse button is pressed
		{
			// Capture the initial mouse position
			lastMousePosition = Input.mousePosition;
		}

		if (Input.GetMouseButton(0))  // While the mouse button is held down
		{
			// Calculate the difference between the current and last mouse position
			Vector3 deltaMouse = Input.mousePosition - lastMousePosition;

			// Apply rotation based on mouse movement
			float rotationX = deltaMouse.y * rotationSpeed;  // Vertical rotation
			float rotationY = -deltaMouse.x * rotationSpeed;  // Horizontal rotation

			// Rotate the camera accordingly
			transform.eulerAngles += new Vector3(rotationX, rotationY, 0);

			// Update the last mouse position
			lastMousePosition = Input.mousePosition;
		}
	}
}