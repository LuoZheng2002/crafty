using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// kept for broadcasting
/// </summary>
public class ResetCountEvent
{

}
public class OtherItemSelectedEvent
{

}
public class DragImageClickedEvent { }
public class DragImage : MonoBehaviour
{
	public bool empty = false;
	public int empty_index = 0;
	public int initial_count = 5;
	public float rayDistance = 5.0f;
	public Util.ComponentType contentType;
	public Util.Component content;
	public VehicleComponent componentPrefab;
	public VehicleComponent componentDesignPrefab;
	VehicleComponent componentInstance = null;
	private RectTransform rectTransform;
	// GridMatrix gridMatrix;
	Text text;
	ButtonScale buttonScale;

	int count = 0;
	public int Count
	{
		get { return count; }
		set
		{
			count = value;
			Text.text = count.ToString();
		}
	}

	Image selectionImage;


	public float minScale = 0.8f;
	public float maxScale = 1.2f;
	public float scaleSpeed = 5.0f;
	static DragImage current;

	public static SortedDictionary<Util.Component, DragImage> DragImages = new();

	public static Dictionary<int, DragImage> EmptyImages = new();
	public static void ClearCountAll()
	{
		foreach(var dragImage in DragImages)
		{
			dragImage.Value.SetInitialCount(0);
		}
	}
	public static void DetachAll()
	{
		foreach (var dragImage in DragImages)
		{
			dragImage.Value.transform.SetParent(null);
		}
		foreach(var emptyImage in EmptyImages)
		{
			emptyImage.Value.transform.SetParent(null);
		}
	}
	public static void StartScaleAll()
	{
		foreach (var dragImage in DragImages)
		{
			dragImage.Value.buttonScale.ScaleStart();
		}
	}
	public static void EndScaleAll()
	{
		foreach (var dragImage in DragImages)
		{
			dragImage.Value.buttonScale.ScaleStop();
		}
	}
	public void SetInitialCount(int count)
	{
		initial_count = count;
		Count = count;
	}
	public static void SetComponentCollection(List<KeyValuePair<Util.Component, int>> components)
	{
		Debug.LogError("Deprecated!");
		foreach (var dragImage in DragImages)
		{
			dragImage.Value.transform.SetParent(null);
		}
		foreach (var component in components)
		{
			Debug.Assert(DragImages.ContainsKey(component.Key));
			var dragImage = DragImages[component.Key];
			dragImage.transform.SetParent(ImageContainer.Inst.transform, false);
			dragImage.transform.localPosition = Vector3.zero;
			dragImage.transform.localScale = Vector3.one;
			dragImage.transform.localRotation = Quaternion.identity;
			dragImage.initial_count = component.Value;
			dragImage.count = component.Value;
			dragImage.ResetCount(null);
		}
	}
	public static DragImage Current
	{
		get
		{
			return current;
		}
		set
		{
			if (current != null)
			{
				current.selectionImage.enabled = false;
			}
			current = value;
			if (current != null)
			{
				current.selectionImage.enabled = true;
				CurrentContentType = current.contentType;
				// Debug.Log($"CurrentContentType set to {CurrentContentType}");
			}
			else
			{
				CurrentContentType = Util.ComponentType.None;
				// Debug.Log($"CurrentContentType set to {CurrentContentType}");
			}
		}
	}
	public static Util.ComponentType CurrentContentType { get; private set; }

	//public static void OnEraseStart()
	//{
	//	CurrentContentType = Util.ContentType.Erase;
	//}
	//public static void OnEraseEnd()
	//{
	//	CurrentContentType = Util.ContentType.None;
	//}
	private void OnEnable()
	{

	}
	private void OnDisable()
	{
		if (componentInstance != null)
		{
			Destroy(componentInstance.gameObject);
			componentInstance = null;
		}
	}
	private void OnDestroy()
	{
		DragImages.Clear();
	}
	public VehicleComponent InstantiateDesignComponent(GridCell grid)
	{
		Debug.Assert(componentDesignPrefab != null);
		// Debug.Log($"Instantiated a design component {content}");
		GameObject inst = Instantiate(componentDesignPrefab.gameObject, GridMatrix.Inst.transform);
		Debug.Assert(inst != null);
		VehicleComponent component = inst.GetComponent<VehicleComponent>();
		Debug.Assert(component != null);
		AccessoryComponent accessory = component as AccessoryComponent;
		if (accessory != null)
		{
			accessory.listen_event = false;
			accessory.GridMatrix = GridMatrix.Inst;
		}
		component.MoveGlobal(grid.transform.position);
		component.InitRotation();
		return component;
	}
	public VehicleComponent InstantiateComponent(Vector3 position, bool local, int direction)
	{
		Debug.Assert(componentPrefab != null);
		GameObject inst= Instantiate(componentPrefab.gameObject, GridMatrix.Inst.transform);
		Debug.Assert(inst != null);
		VehicleComponent component = inst.GetComponent<VehicleComponent>();
		Debug.Assert(component != null);
		if (local)
		{
			component.MoveLocal(position);
		}
		else
		{
			component.MoveGlobal(position);
		}
		AccessoryComponent accessory = component as AccessoryComponent;
		if (accessory != null)
		{
			Debug.Assert(GridMatrix.Inst != null);
			accessory.GridMatrix = GridMatrix.Inst;
			accessory.Direction = direction;
		}
		return component;
	}
	public Text Text
	{
		get
		{
			if (text == null)
			{
				text = transform.Find("Count").GetComponent<Text>();
				Debug.Assert(text != null, "Text not found");
			}
			return text;
		}
	}
	
	void Awake()
	{
		rectTransform = GetComponent<RectTransform>();
	}
	private void Start()
	{
		buttonScale = GetComponent<ButtonScale>();
		Debug.Assert(buttonScale != null);
		Debug.Assert(!DragImages.ContainsKey(content));
		if (!empty)
		{
			DragImages[content] = this;
		}
		else
		{
			EmptyImages[empty_index] = this;
		}
		EventBus.Subscribe<ResetCountEvent>(ResetCount);
		Count=initial_count;
		selectionImage = transform.Find("Selection").GetComponent<Image>();
		selectionImage.enabled = false;
		Debug.Assert(selectionImage != null);
		EventBus.Subscribe<OtherItemSelectedEvent>(OnAddComponentInterrupt);
	}
	public void ResetCount(ResetCountEvent e)
	{
		Count = initial_count;
	}
	bool mouse_click_flag = false;
	IEnumerator StartDrag()
	{
		while (count > 0)
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			// Get the direction of the ray
			Vector3 rayDirection = ray.direction;

			Vector3 instantiatePos = ray.origin + rayDirection * rayDistance;
			componentInstance = InstantiateComponent(instantiatePos, false, 0);
			while(!mouse_click_flag)
			{
				// don't know if it will work
				if (Input.GetMouseButtonDown(0))
				{
					mouse_click_flag = true;
				}
				Debug.Assert(componentInstance != null);
				ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				// Get the direction of the ray
				rayDirection = ray.direction;
				if (GridMatrix.Inst.SelectedGrid != null)
				{
					componentInstance.MoveGlobal(GridMatrix.Inst.SelectedGrid.transform.position);
				}
				else
				{
					Vector3 newPos = ray.origin + rayDirection * rayDistance;
					componentInstance.MoveGlobal(newPos);
				}
				yield return null;
			}
			// mouse has clicked
			mouse_click_flag = false;
			// handle post click event
			Debug.Assert(componentInstance != null);
			if (GridMatrix.Inst.SelectedGrid != null)
			{
				Count--;
				GridMatrix.Inst.AddComponent(GridMatrix.Inst.SelectedGrid, contentType, componentInstance);
				componentInstance = null;
			}
			else
			{
				Destroy(componentInstance.gameObject);
				componentInstance = null;
				if (reset_flag)
				{
					GridMatrix.Inst.CurrentCursorMode = Util.CursorMode.Idle;
					reset_flag = false;
					yield break;
				}
				reset_flag = true;
			}
			GridMatrix.Inst.SelectedGrid = null;
		}
	}
	bool reset_flag = false;
	void OnAddComponentInterrupt(OtherItemSelectedEvent e)
	{
		// Debug.Log($"{content} gets interrupted!");
		Current = null;
		if (coroutine != null)
		{
			StopCoroutine(coroutine);
			coroutine = null;
			if (componentInstance != null)
			{
				Destroy(componentInstance.gameObject) ;
				componentInstance = null;
			}
		}
	}
	IEnumerator coroutine;
	public void OnClick()
	{
		EventBus.Publish(new DragImageClickedEvent());
		EventBus.Publish(new OtherItemSelectedEvent());
		CustomCursor.Inst.SetIdleCursor();
		GridMatrix.Inst.CurrentCursorMode = Util.CursorMode.AddComponent;
		Current = this;
		if (count > 0)
		{
			// GameState.shown_drag_images = true;
			// buttonScale.ScaleStop();
			// ToastManager.Toast("Drag!");
			if (coroutine == null)
			{
				coroutine = StartDrag();
				StartCoroutine(coroutine);
			}
			else
			{
				Debug.LogWarning("A coroutine already in progress");
			}
		}
	}
}
