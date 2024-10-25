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
public class DragImage : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
	public int initial_count = 5;
	public float rayDistance = 5.0f;
	public Util.ContentType contentType;
	public Util.Content content;
	public VehicleComponent componentPrefab;
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
	Image image;


	public float minScale = 0.8f;
	public float maxScale = 1.2f;
	public float scaleSpeed = 5.0f;
	static DragImage current;

	public static Dictionary<Util.Content, DragImage> DragImages = new();
	public static void SetComponentCollection(List<KeyValuePair<Util.Content, int>> components)
	{
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
				Debug.Log($"CurrentContentType set to {CurrentContentType}");
			}
			else
			{
				CurrentContentType = Util.ContentType.None;
				Debug.Log($"CurrentContentType set to {CurrentContentType}");
			}
		}
	}
	public static Util.ContentType CurrentContentType { get; private set; }

	public static void OnEraseStart()
	{
		CurrentContentType = Util.ContentType.Erase;
	}
	public static void OnEraseEnd()
	{
		CurrentContentType = Util.ContentType.None;
	}
	private void OnEnable()
	{
		buttonScale = GetComponent<ButtonScale>();
		image = GetComponent<Image>();
		if (!GameState.shown_drag_images)
		{
			buttonScale.ScaleStart();
		}
	}
	private void OnDestroy()
	{
		DragImages.Clear();
	}
	public VehicleComponent InstantiateContent(Vector3 position, bool local, int direction)
	{
		Debug.Assert(componentPrefab != null);
		GameObject inst= Instantiate(componentPrefab.gameObject, GridMatrix.Current.transform);
		Debug.Assert(inst != null);
		VehicleComponent component = inst.GetComponent<VehicleComponent>();
		if (local)
		{
			component.MoveLocal(position);
		}
		else
		{
			component.MoveGlobal(position);
		}
		DirectionalComponent directionalPreview = component as DirectionalComponent;
		if (directionalPreview != null)
		{
			directionalPreview.Direction = direction;
		}
		return component;
	}
	public Text Text
	{
		get
		{
			if (text == null)
			{
				text = transform.GetChild(0).GetComponent<Text>();
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
		Debug.Assert(!DragImages.ContainsKey(content));
		DragImages[content] = this;
		EventBus.Subscribe<ResetCountEvent>(ResetCount);
		Count=initial_count;
		selectionImage = transform.Find("Selection").GetComponent<Image>();
		selectionImage.enabled = false;
		Debug.Assert(selectionImage != null);
	}
	public void ResetCount(ResetCountEvent e)
	{
		Count = initial_count;
	}
	public void ClickPlace()
	{
		if (count > 0)
		{
			VehicleComponent componentInst = InstantiateContent(GridMatrix.SelectedGrid.transform.localPosition, true, 0);
			Count--;
			GridMatrix.Current.AddComponent(GridMatrix.SelectedGrid, contentType, componentInst);
			if (Count <=0)
			{
				Current = null;
			}
		}
		else
		{
			Current = null;
		}
	}
	public void OnBeginDrag(PointerEventData eventData)
	{
		GameState.shown_drag_images = true;
		buttonScale.ScaleStop();
		if (count > 0)
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			// Get the direction of the ray
			Vector3 rayDirection = ray.direction;

			Vector3 instantiatePos = ray.origin + rayDirection * rayDistance;
			Transform gridMatrixTransform = GridMatrix.Current.transform;

			componentInstance = InstantiateContent(instantiatePos, false, 0);
			Util.SetLayerRecursively(componentInstance.gameObject, "MaskLayer");
			Count--;
			Current = this;
		}
		else
		{
			Current = null;
		}
	}
	void DragHelper()
	{
		if (componentInstance == null)
		{
			return;
		}
		// awkward fix
		// componentInstance.transform.localRotation = Quaternion.identity;

		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		// Get the direction of the ray
		Vector3 rayDirection = ray.direction;
		if (GridMatrix.SelectedGrid != null)
		{
			componentInstance.MoveGlobal(GridMatrix.SelectedGrid.transform.position);
			Util.SetLayerRecursively(componentInstance.gameObject, "ContentCrate");
		}
		else
		{
			Vector3 newPos = ray.origin + rayDirection * rayDistance;
			componentInstance.MoveGlobal(newPos);
			Util.SetLayerRecursively(componentInstance.gameObject, "MaskLayer");
		}
	}
	public void OnDrag(PointerEventData eventData)
	{
		DragHelper();
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		Debug.Assert(componentInstance != null);
		if (componentInstance == null)
		{
			return;
		}
		if (GridMatrix.SelectedGrid != null)
		{
			Util.SetLayerRecursively(componentInstance.gameObject, "ContentCrate");
			GridMatrix.Current.AddComponent(GridMatrix.SelectedGrid, contentType, componentInstance);
			componentInstance = null;
			if (Count == 0)
			{
				Current = null;
			}
		}
		else
		{
			Count++;
			Destroy(componentInstance.gameObject);
			componentInstance = null;
		}
		GridMatrix.SelectedGrid = null;
		// CurrentContentType = Util.ContentType.None;
	}
	public void OnClick()
	{
		if (count > 0)
		{
			GameState.shown_drag_images = true;
			buttonScale.ScaleStop();
			ToastManager.Toast("Drag!");
			Current = this;
		}
		else
		{
			Current = null;
		}
	}
}
