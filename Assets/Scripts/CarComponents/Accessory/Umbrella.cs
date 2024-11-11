using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Umbrella : AccessoryComponent
{
    Collider c;
    public GameObject cube;
    int current_rotation = 0;
    bool built = false;
    bool open = false;
    public float damp = 1.0f;

    public override Util.Component Component => Util.Component.Umbrella;

    public override void Build()
    {
        RB.useGravity = true;
        c.enabled = true;
        built = true;
    }

    public override (bool wa, bool sd) GetWASD()
    {
        return (false, false);
    }


    // Update is called once per frame
    void Update()
    {
        if (built)
        {
			if (Input.GetKeyDown(KeyCode.Space))
			{
				open = !open;
				cube.SetActive(open);
			}
				if (open)
            {
                RB.mass = 20;
                RB.drag = damp;
            }
            else
            {
                RB.mass = 2;
				RB.drag = 0;
			}
            
        }
    }
    private void Start()
    {
        c = GetComponent<Collider>();
        Init();
    }
	public override void Stick()
	{
        StickUmbrellaOrWheel();
	}
    public override List<(Quaternion, RotationInfo)> Rotations => Util.UmbrellaRotations;
	public override bool[] GetDirectionMask()
	{
        return GetDirectionMaskWheelOrUmbrella();
	}
}
