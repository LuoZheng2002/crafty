using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotorWheelPreview : WheelPreview
{
	public override Util.Component Component => Util.Component.MotorWheel;
	public override (bool wa, bool sd) GetWASD()
	{
		return (true, false);
	}
}
