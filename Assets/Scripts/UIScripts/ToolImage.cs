using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolClickedEvent
{
	public Util.CursorMode cursor_mode;
    public ToolClickedEvent(Util.CursorMode cursor_mode)
    {
        this.cursor_mode = cursor_mode;
    }
}
public class ToolImage : MonoBehaviour
{
	public Util.CursorMode cursor_mode;
	Image selectionImage;
	private void Start()
	{
		selectionImage = transform.Find("Selection").GetComponent<Image>();
		Debug.Assert(selectionImage != null, "Selection image is null");
		selectionImage.enabled = false;
		EventBus.Subscribe<OtherItemSelectedEvent>(OnOtherItemSelected);
	}
	void OnOtherItemSelected(OtherItemSelectedEvent e)
	{
		selectionImage.enabled = false;
	}
	private void Update()
	{
        if (cursor_mode == Util.CursorMode.Idle)
        {
			if (Input.GetKeyDown(KeyCode.BackQuote) || Input.GetKeyDown(KeyCode.Escape))
			{
				OnClick();
			}
		}
	}
	public void OnClick()
	{
		EventBus.Publish(new OtherItemSelectedEvent());
		EventBus.Publish(new ToolClickedEvent(cursor_mode));
		GridMatrix.Inst.CurrentCursorMode = cursor_mode;
		selectionImage.enabled = true;
		switch (cursor_mode)
		{
			case Util.CursorMode.Idle:
				CustomCursor.Inst.SetIdleCursor();
				break;
			case Util.CursorMode.Erase:
				CustomCursor.Inst.SetEraserCursor();
				break;
			case Util.CursorMode.ChangeDirection:
				CustomCursor.Inst.SetWrenchCursor();
				break;
		}
	}
}
