using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public abstract class BoosterPreview : AccessoryComponent
{
    int current_rotation = 0;
    public override int Direction { get { return current_rotation; } set {
        current_rotation = value;
            Quaternion global_rotation = transform.parent.rotation * Util.BoosterRotations[current_rotation].Item1;
			rb = GetComponent<Rigidbody>();
			rb.MoveRotation(global_rotation);
        } }

    public abstract override Util.Component Component {  get; }

    Rigidbody rb;
    Collider c;
    [SerializeField] float thrust = 2.0f;
    [SerializeField] float fuel = 100f;
    [SerializeField] float fuel_usage = 5f;

    bool built;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Debug.Assert(rb != null);
        c = GetComponent<Collider>();
    }

    // Update is called once per frame
    public float max_time = 1.0f;
    float time = 0.0f;
    void Update()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            time = Mathf.Clamp(time + Time.deltaTime, 0, max_time);
            rb.AddForce(transform.up * thrust * time/max_time);
            fuel -= fuel_usage;
        }
        else
        {
			time = Mathf.Clamp(time - 2*Time.deltaTime, 0, max_time);
		}
    }
    public override (bool wa, bool sd) GetWASD()
    {
        return (false, false);
    }

    public override (int h_delta, int w_delta, int l_delta) AttachDir()
    {
        return (-1, 0, 0);
    }

    public override void ChangeDirection(bool forward = true)
    {
        Debug.Log("Direction Changed");
        if (forward)
        {
            Direction = (Direction + 1) % Util.BoosterRotations.Count;
        }
        else
        {
            Direction = (Direction + Util.BoosterRotations.Count - 1) % Util.BoosterRotations.Count;
        }
    }

    public override void Build()
    {
        rb.useGravity = true;
        c.enabled = true;
        built = true;
    }

    public override void SetActive(bool active)
    {
        
    }
	public override void Stick(GridMatrix gridMatrix, int h, int w, int l)
	{
        List<(int dh, int dw, int dl)> directions = new() { (1, 0, 0), (-1, 0, 0), (0, 1, 0), (0, -1, 0), (0, 0, 1), (0, 0, -1) };
        Debug.Log("Booster Stick called");
        foreach (var direction in directions)
        {
            (int dh, int dw, int dl) = direction;
            (int new_h, int new_w, int new_l) = (h+dh, w+dw, l+dl);
            if(gridMatrix.InGrid(new_h, new_w, new_l) && gridMatrix.crates[new_h, new_w, new_l] != null)
            {
				Debug.Log("Hello");
				Util.CreateJoint(this, gridMatrix.crates[new_h, new_w, new_l], gridMatrix.position_spring, gridMatrix.position_damper);
            }
        }
	}
}
