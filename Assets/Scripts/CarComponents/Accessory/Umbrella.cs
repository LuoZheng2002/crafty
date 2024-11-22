using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Umbrella : AccessoryComponent
{
    Collider c;
    int current_rotation = 0;
    bool built = false;
    bool open = true;
    public float damp = 1.0f;
    public GameObject open_visual;
    public GameObject close_visual;
    public override Util.Component Component => Util.Component.Umbrella;

    public override void Build()
    {
        RB.useGravity = true;
        c.enabled = true;
        built = true;
        Open = false;
    }

    public override (bool wa, bool sd) GetWASD()
    {
        return (false, false);
    }
    public bool Open
    {
        get { return open; }
        set
        {
            open = value;
            if (open)
            {
                RB.mass = 20;
                RB.drag = damp;
                open_visual.SetActive(true);
                close_visual.SetActive(false);
            }
            else
            {
                RB.mass = 2;
                RB.drag = 0;
                open_visual.SetActive(false);
                close_visual.SetActive(true);
            }
        }
    }

	// Update is called once per frame
	void Update()
    {
        if (built)
        {
			if (Input.GetKeyDown(KeyCode.Space))
			{
                Open = !Open;
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
        StickUmbrellaOrWheel(false);
	}
    public override List<(Quaternion, RotationInfo)> Rotations => Util.UmbrellaRotations;
	public override bool[] GetDirectionMask()
	{
        return GetDirectionMaskWheelOrUmbrella();
	}
}
