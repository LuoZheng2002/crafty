using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ContentPreview : MonoBehaviour
{
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
	public abstract void ChangeDirection();
}
public abstract class LoadPreview: DirectionalPreview
{

}
public abstract class AccessoryPreview: DirectionalPreview
{
	public abstract (int h_delta, int w_delta, int l_delta) AttachDir();
}