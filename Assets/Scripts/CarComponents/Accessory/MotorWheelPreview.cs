using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotorWheelPreview : WheelPreview
{
	public override Util.Content Content => Util.Content.MotorWheel;
	public override (bool wa, bool sd) GetWASD()
	{
		return (true, false);
	}
}
