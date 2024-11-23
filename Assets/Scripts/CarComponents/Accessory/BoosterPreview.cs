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
	private void OnEnable()
	{
		particle_system1.Stop();
		particle_system2.Stop();
		particle_system3.Stop();
	}

	// Update is called once per frame
	public float max_time = 1.0f;
    float time = 0.0f;
    public ParticleSystem particle_system1;
	public ParticleSystem particle_system2;
	public ParticleSystem particle_system3;
    bool exhausted = false;
    public void PlayParticle()
    {
		particle_system1.Play();
		particle_system2.Play();
		particle_system3.Play();
	}
	void Update()
    {
        if (built)
        { 
            if (Input.GetMouseButton(1))
            {
                if (PlayCanvas.Inst.RocketFuel >0.0f)
                {
					RB.AddForce(transform.up * thrust);
					if (Input.GetMouseButtonDown(1))
					{
                        PlayParticle();
					}
				}				
			}
            if (!exhausted && PlayCanvas.Inst.RocketFuel <=0.0f)
            {
                exhausted = true;
				particle_system1.Stop();
				particle_system2.Stop();
				particle_system3.Stop();
			}
            if (Input.GetMouseButtonUp(1))
			{
				particle_system1.Stop();
				particle_system2.Stop();
				particle_system3.Stop();
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
