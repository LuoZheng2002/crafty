using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Umbrella : AccessoryComponent
{
    Rigidbody rb;
    Collider c;
    public GameObject cube;
    int current_rotation = 0;
    bool built = false;
    bool open = false;
    public float damp = 1.0f;
    public override int Direction {
        get => 0;
        set { }
        
    }

    public override Util.Component Component => Util.Component.Umbrella;

    public override (int h_delta, int w_delta, int l_delta) AttachDir()
    {
        return Util.UmbrellaRotations[current_rotation].Item2;
    }

    public override void Build()
    {
        rb.useGravity = true;
        c.enabled = true;
        built = true;
    }

    public override void ChangeDirection(bool forward = true)
    {
        
    }

    public override (bool wa, bool sd) GetWASD()
    {
        return (false, false);
    }

    public override void SetActive(bool active)
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            open = !open;
            
            cube.SetActive(open);
        }
        if (built)
        {
            if (open)
            {
                rb.mass = 20;
                rb.drag = damp;
            }
            else
            {
                rb.mass = 2;
				rb.drag = 0;
			}
            
        }
    }
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        c = GetComponent<Collider>();
    }
	public override void Stick(GridMatrix gridMatrix, int h, int w, int l)
	{
        (int dh, int dw, int dl) = AttachDir();
		(int new_h, int new_w, int new_l) = (h + dh, w + dw, l + dl);
		if (gridMatrix.InGrid(new_h, new_w, new_l) && gridMatrix.crates[new_h, new_w, new_l] != null)
		{
			Util.CreateJoint(this, gridMatrix.crates[new_h, new_w, new_l], gridMatrix.position_spring, gridMatrix.position_damper);
		}
	}
}
