using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Umbrella : AccessoryComponent
{
    Rigidbody rb;
    Collider c;
    public GameObject cube;
    public float dragForce = 0.5f;
    int current_rotation = 0;
    bool built = false;
    bool open = false;
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
                rb.AddForce(rb.velocity * -dragForce);
            }
            
        }
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        c = GetComponent<Collider>();
    }
}
