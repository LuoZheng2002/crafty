using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public abstract class BoosterPreview : AccessoryComponent
{
    public override int Direction { get => 0; set { } }

    public abstract override Util.Content Content {  get; }

    bool built;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public override (bool wa, bool sd) GetWASD()
    {
        return (false, false);
    }

    public override (int h_delta, int w_delta, int l_delta) AttachDir()
    {
        return (0, 0, 0);
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
        Rigidbody rb = GetComponent<Rigidbody>();
        Collider c = GetComponent<Collider>();
        rb.useGravity = true;
        c.enabled = true;
        built = true;
    }

    public override void SetActive(bool active)
    {
        
    }
}
