using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObtainCanvas : MonoBehaviour
{
    static ObtainCanvas inst;
	public Image img;
	public Sprite motor_wheel_sprite;
	public Sprite turn_wheel_sprite;
	public Sprite umbrella_sprite;
	public Sprite rocket_sprite;
	public Text text;
	Dictionary<Util.Component, (Sprite, string)> dict;
	public static ObtainCanvas Inst
	{
		get
		{
			Debug.Assert(inst != null);
			return inst;
		}
	}
	private void Start()
	{
		Debug.Assert(inst == null);
		inst = this;
		dict = new()
		{
			{Util.Component.MotorWheel, (motor_wheel_sprite, "Motor Wheel")},
			{Util.Component.TurnWheel, (turn_wheel_sprite, "Turn Wheel")},
			{Util.Component.Umbrella, (umbrella_sprite, "Umbrella")},
			{Util.Component.Rocket, (rocket_sprite, "Rocket")}
		};
		gameObject.SetActive(false);
	}
	private void OnDestroy()
	{
		inst = null;
	}
	public void Show(Util.Component component)
	{
		gameObject.SetActive(true);
		img.sprite = dict[component].Item1;
		text.text = dict[component].Item2;
		// CarCore.Inst.Fix();
		CarCore.Inst.DampStart();
	}
	private void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			// CarCore.Inst.Unfix();
			gameObject.SetActive(false);
			CarCore.Inst.DampStop();
		}
	}
}
