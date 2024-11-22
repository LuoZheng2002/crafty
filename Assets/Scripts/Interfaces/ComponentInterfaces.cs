using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vec3
{
    public int h;
    public int w;
    public int l;
    public Vec3(int h, int w, int l)
    {
        this.h = h;
        this.w = w;
        this.l = l;
    }
	public static implicit operator Vec3((int h, int w, int l) pos)
	{
        return new Vec3(pos.h, pos.w, pos.l);
	}
	public void Deconstruct(out int h, out int w, out int l)
	{
        h = this.h;
        w = this.w;
		l = this.l;
	}
	public static Vec3 operator+(Vec3 v1, Vec3 v2)
    {
        return new Vec3(v1.h+v2.h, v1.w + v2.w, v1.l +v2.l);
    }
    //public (int h, int w, int l) Unwrap()
    //{
    //    return (h, w, l);
    //}
}

public abstract class VehicleComponent : MonoBehaviour
{
    public Rigidbody RB
    {
        get
        {
            if (rb == null)
            {
                rb = GetComponent<Rigidbody>();
            }
            return rb;
        }
    }
    Rigidbody rb;
    GridMatrix grid_matrix;
    public void DampStart()
    {
        RB.drag = 1;
    }
    public void DampStop()
    {
        RB.drag = 0;
    }
    public GridMatrix GridMatrix { 
        get { Debug.Assert(grid_matrix != null, $"{Component}'s grid matrix not set"); return grid_matrix; }
        set { grid_matrix = value; }
    }
    Vec3 pos;
    public Vec3 Pos
    {
        get { Debug.Assert(pos != null, $"{Component}'s position not set"); return pos; }
        set { pos = value; }    
    }
    public void MoveLocal(Vector3 position)
    {
		Vector3 worldPosition = transform.parent.TransformPoint(position);
		RB.MovePosition(worldPosition);
        // Debug.Log($"{caller} changed {Content}'s local position");
    }
	public void MoveGlobal(Vector3 position)
	{
        RB.MovePosition(position);
		// Debug.Log($"{caller} changed {Content}'s global position");
	}
    public void InitRotation()
    {
        RB.MoveRotation(transform.parent.rotation);
    }
	public abstract Util.Component Component { get; }
    //void Awake()
    //{
    //    EventBus.Subscribe<GameStateChangedEvent>(OnGameStateChanged);
    //}
    //void OnGameStateChanged(GameStateChangedEvent e)
    //{
    //    if (e.state == Util.GameStateType.Intro || e.state == Util.GameStateType.Build)
    //    {
    //        Destroy(gameObject);
    //    }
    //}
    public abstract void Build();
	private void OnJointBreak(float breakForce)
	{
		if (!GameSave.BreakTutorialWatched)
        {
            GameSave.BreakTutorialWatched = true;
            GameState.Inst.StartCoroutine(JointBreakTutorial());
		}
        if (particle_system != null)
        {
            particle_system.gameObject.SetActive(true);
            particle_system.Play();
        }
	}
	public ParticleSystem particle_system;
	IEnumerator JointBreakTutorial()
	{
		yield return LineCanvas.Top.DisplayLineAndWaitForClick("Shirley", "Oh no! Our car breaks! We cannot rebuild our vehicle in place anymore.", null);
		yield return LineCanvas.Top.DisplayLineAndWaitForClick("Shirley", "However, we can still go back to the latest checkpoint.", null);
        LineCanvas.Top.Hide();
        Retry.Inst.StartScale();
	}
}

public abstract class CrateComponent: VehicleComponent
{

}
public abstract class LoadComponent: VehicleComponent
{

}
public abstract class AccessoryComponent : VehicleComponent
{
    public abstract List<(Quaternion, RotationInfo)> Rotations { get; }
    public abstract (bool wa, bool sd) GetWASD();
    public abstract void Stick();
    protected bool[] direction_mask;
    public bool listen_event = true;
    public int Direction
    {
        get { return direction; }
        set { direction = value; SetDirection(value); }
    }
    public void OnNeighborChanged(NeighborChangedEvent e)
    {
        direction_mask = GetDirectionMask();
        Debug.Assert(direction_mask.Length == Rotations.Count, "Direction mask's length is not consistent with num directions");
        UpdateDirection();
    }
    void UpdateDirection()
    {
        if (!direction_mask[Direction])
        {
            Debug.Log($"{Component} direction changed");
            ChangeDirection();
        }
    }
    public abstract bool[] GetDirectionMask();
    // ()
    // left, 
    // rotation, 
    protected CrateComponent GetCrateOnDirection(Vec3 dir)
    {
        Debug.Assert(Pos != null);
        Debug.Assert(dir != null);
        Vec3 new_pos = Pos + dir;
        if (!GridMatrix.InGrid(Pos + dir))
        {
            return null;
        }
        return GridMatrix.GetCrate(new_pos);
    }
    public void Init()
    {
        direction_mask = new bool[Rotations.Count];
        Array.Fill(direction_mask, true);
        if (listen_event)
        {
            EventBus.Subscribe<NeighborChangedEvent>(OnNeighborChanged);
        }
    }
    int direction = 0;
    public void ChangeDirection()
    {
        Debug.Assert(direction_mask != null, "Direction mask is null");
        int start_direction = direction;
        do
        {
			direction = (direction + 1) % Rotations.Count;
            if (direction == start_direction)
            {
                Debug.Assert(false, "No avaliable direction!");
                break;
            }
        } while (!direction_mask[direction]);
        SetDirection(direction);
    }
    void SetDirection(int direction)
    {
        Debug.Assert(RB != null);
        Debug.Assert(GridMatrix != null);
		RB.MoveRotation(GridMatrix.transform.rotation * Rotations[direction].Item1);
	}
    protected void StickUmbrellaOrWheel(bool is_wheel)
    {
		Vec3 attach_dir = Rotations[Direction].Item2.attach_dir;
		Vec3 new_pos = Pos + attach_dir;
		if (GridMatrix.InGrid(new_pos) && GridMatrix.GetCrate(new_pos) != null)
		{
            if (is_wheel)
			{
				Util.CreateJoint(this, GridMatrix.GetCrate(new_pos), Util.break_force, Util.break_torque);
			}
			else
			{
				Util.CreateJoint(this, GridMatrix.GetCrate(new_pos), Util.break_force, Util.break_torque);
			}
		}
	}
    protected void StickRocket()
    {
		List<Vec3> directions = new() { (1, 0, 0), (-1, 0, 0), (0, 1, 0), (0, -1, 0), (0, 0, 1), (0, 0, -1) };
		Debug.Log("Booster Stick called");
		foreach (var direction in directions)
		{
			Vec3 new_pos = Pos + direction;
			if (GridMatrix.InGrid(new_pos) && GridMatrix.GetCrate(new_pos) != null)
			{
				Util.CreateJoint(this, GridMatrix.GetCrate(new_pos), Util.break_force*3, Util.break_torque*3);
			}
		}
	}
    protected bool[] GetDirectionMaskWheelOrUmbrella()
    {
		bool has_neighbor_crate = false;
		bool[] result = new bool[Rotations.Count];
		for (int i = 0; i < Rotations.Count; i++)
		{
			var rotation = Rotations[i];
			Vec3 attach_dir = rotation.Item2.attach_dir;
			if (GetCrateOnDirection(attach_dir) != null)
			{
				has_neighbor_crate = true;
				result[i] = true;
			}
		}
		if (!has_neighbor_crate)
		{
			Array.Fill(result, true);
		}
		return result;
	}
}

// onneighborchanged
// allowed_direction_mask