using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public abstract class BoosterPreview : AccessoryComponent
{
    int current_rotation = 0;

    public abstract override Util.Component Component {  get; }

    Collider c;
    [SerializeField] float thrust = 2.0f;
    [SerializeField] float fuel = 100f;
    [SerializeField] float fuel_usage = 5f;

    bool built;

    // Start is called before the first frame update
    void Start()
    {
        c = GetComponent<Collider>();
        
        Init();
    }

    // Update is called once per frame
    public float max_time = 1.0f;
    float time = 0.0f;
    void Update()
    {
        if (built)
        { 
            if (Input.GetKey(KeyCode.Q))
            {
                time = Mathf.Clamp(time + Time.deltaTime, 0, max_time);
                RB.AddForce(transform.up * thrust * time / max_time);
                fuel -= fuel_usage;
            }
            else
            {
                time = Mathf.Clamp(time - 2 * Time.deltaTime, 0, max_time);
            }
        }
    }
    public override (bool wa, bool sd) GetWASD()
    {
        return (false, false);
    }


    public override void Build()
    {
        c.enabled = true;
		RB.useGravity = true;
		built = true;
    }
	public override void Stick()
	{
        StickRocket();
	}
    public override List<(Quaternion, RotationInfo)> Rotations => Util.BoosterRotations;
	public override bool[] GetDirectionMask()
	{
        return new bool[] { true, true, true, true, true, true };
	}
}
