using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class TurnWheelPreview : WheelPreview
{
	public override Util.Component Component => Util.Component.TurnWheel;
	public override (bool wa, bool sd) GetWASD()
	{
		return (false, true);
	}
}
