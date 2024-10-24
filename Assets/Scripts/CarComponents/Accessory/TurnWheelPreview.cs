using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnWheelPreview : WheelPreview
{
	public override Util.Content Content => Util.Content.TurnWheel;
	public override (bool wa, bool sd) GetWASD()
	{
		return (false, true);
	}
}
