using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ContentPreview : MonoBehaviour
{
    public void MoveLocal(Vector3 position, string caller)
    {
        transform.localPosition = position;
        // Debug.Log($"{caller} changed {Content}'s local position");
    }
	public void MoveGlobal(Vector3 position, string caller)
	{
		transform.position = position;
		// Debug.Log($"{caller} changed {Content}'s global position");
	}
	public abstract Util.Content Content { get; }
    void Awake()
    {
        EventBus.Subscribe<GameStateChangedEvent>(OnGameStateChanged);
    }
    void OnGameStateChanged(GameStateChangedEvent e)
    {
        if (e.state == Util.GameStateType.Intro || e.state == Util.GameStateType.Build)
        {
            Destroy(gameObject);
        }
    }
    public abstract void Build();
	public abstract void SetActive(bool active);
}
public abstract class DirectionalPreview: ContentPreview
{
    public abstract int Direction { get; set; }
	public abstract void ChangeDirection(bool forward = true);
}
public abstract class LoadPreview: DirectionalPreview
{

}
public abstract class AccessoryPreview: DirectionalPreview
{
	public abstract (int h_delta, int w_delta, int l_delta) AttachDir();
    public abstract (bool wa, bool sd) GetWASD();
}