using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class VehicleComponent : MonoBehaviour
{
    public void MoveLocal(Vector3 position)
    {
        Rigidbody rb = GetComponent<Rigidbody>();
		Vector3 worldPosition = transform.parent.TransformPoint(position);
		rb.MovePosition(worldPosition);
        // Debug.Log($"{caller} changed {Content}'s local position");
    }
	public void MoveGlobal(Vector3 position, string caller)
	{
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.MovePosition(position);
		// Debug.Log($"{caller} changed {Content}'s global position");
	}
	public abstract Util.Content Content { get; }
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
	public abstract void SetActive(bool active);
}
public abstract class CrateComponent: VehicleComponent
{

}
public abstract class DirectionalComponent: VehicleComponent
{
    public abstract int Direction { get; set; }
	public abstract void ChangeDirection(bool forward = true);
}
public abstract class LoadComponent: DirectionalComponent
{

}
public abstract class AccessoryComponent: DirectionalComponent
{
	public abstract (int h_delta, int w_delta, int l_delta) AttachDir();
    public abstract (bool wa, bool sd) GetWASD();
}