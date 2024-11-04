using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public abstract class BoosterPreview : AccessoryComponent
{
    int current_rotation = 0;
    public override int Direction { get { return current_rotation; } set {
        current_rotation = value;
           transform.localRotation = Util.BoosterRotations[current_rotation].Item1;
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
        c = GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.V) && fuel > 0)
        {
            rb.AddForce(transform.up * thrust);
            fuel -= fuel_usage;
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
            Direction = (Direction + 1) % Util.WheelRotations.Count;
        }
        else
        {
            Direction = (Direction + Util.WheelRotations.Count - 1) % Util.WheelRotations.Count;
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
}
