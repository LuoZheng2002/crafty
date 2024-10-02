using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ContentPreview : MonoBehaviour
{
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

}