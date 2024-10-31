using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomCursor : MonoBehaviour
{
	// Drag your cursor texture here in the Inspector
	public Texture2D idleCursorTexture;
	public Texture2D pressedCursorTexture;
	public Texture2D eraserCursorTexture;
	public Texture2D wrenchCursorTexture;
	public Vector2 hotSpot = Vector2.zero;  // Position of the cursor point
	public CursorMode cursorMode = CursorMode.Auto;
	Util.CursorMode cursorState = Util.CursorMode.Idle;
	public static CustomCursor Inst
	{
		get { Debug.Assert(inst != null, "Custom Cursor not set");return inst; }
	}
	static CustomCursor inst;

	Texture2D ResizeTexture(Texture2D originalTexture, int width, int height)
	{
		Texture2D resizedTexture = new Texture2D(width, height);
		Color[] pixels = originalTexture.GetPixels();
		Color[] resizedPixels = resizedTexture.GetPixels();

		for (int y = 0; y < height; y++)
		{
			for (int x = 0; x < width; x++)
			{
				// Map the coordinates to the original texture
				int originalX = Mathf.RoundToInt((x / (float)width) * originalTexture.width);
				int originalY = Mathf.RoundToInt((y / (float)height) * originalTexture.height);
				resizedPixels[y * width + x] = originalTexture.GetPixel(originalX, originalY);
			}
		}

		resizedTexture.SetPixels(resizedPixels);
		resizedTexture.Apply();
		return resizedTexture;
	}
	void Start()
	{
		Debug.Assert(inst == null, "Custom Cursor Already Set");
		inst = this;
		// Set the custom cursor at the start of the game
		Cursor.SetCursor(idleCursorTexture, hotSpot, cursorMode);
	}
	private void OnDestroy()
	{
		ResetCursor();
		inst = null;
	}
	private void Update()
	{
		if (cursorState == Util.CursorMode.Idle)
		{
			if (Input.GetMouseButtonDown(0))
			{
				SetCursor(pressedCursorTexture);
			}
			else if (Input.GetMouseButtonUp(0))
			{
				SetCursor(idleCursorTexture);
			}
		}
	}
	// If you want to change the cursor dynamically, you can add functions like this:
	public void SetCursor(Texture2D newCursorTexture)
	{
		Cursor.SetCursor(newCursorTexture, hotSpot, cursorMode);
	}
	public void SetIdleCursor()
	{
		cursorState = Util.CursorMode.Idle;
		SetCursor(idleCursorTexture);
	}
	public void SetEraserCursor()
	{
		cursorState = Util.CursorMode.Erase;
		SetCursor(eraserCursorTexture);
	}
	public void SetWrenchCursor()
	{
		cursorState = Util.CursorMode.ChangeDirection;
		SetCursor(wrenchCursorTexture);
	}
	// To reset to the default system cursor:
	public void ResetCursor()
	{
		Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
	}
}