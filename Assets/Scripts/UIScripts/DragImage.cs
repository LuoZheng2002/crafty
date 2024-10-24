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
	VehicleComponent instantiatedPreview = null;
	private RectTransform rectTransform;
	private CanvasGroup canvasGroup;
	// GridMatrix gridMatrix;
	public Util.ContentType contentType;
	public Util.Content content;
	Text text;
	GameState gameState;

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
	Image image;
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
			dragImage.transform.SetParent(ImageContainer.Inst.transform);
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
		image = GetComponent<Image>();
		StartCoroutine(Scale());
	}
	private void OnDestroy()
	{
		DragImages.Clear();
	}
	IEnumerator Scale()
	{
		while (!GameState.shown_drag_images)
		{
			float scale = (Mathf.Sin(Time.time * scaleSpeed) + 1.0f) / 2.0f * (maxScale - minScale) + minScale;
			// Debug.Log($"Scale: {scale}");
			image.rectTransform.localScale = new Vector3(scale, scale, scale);
			yield return null;
		}
		image.rectTransform.localScale = Vector3.one;
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
		canvasGroup = GetComponent<CanvasGroup>();
	}
	private void Start()
	{
		Debug.Assert(!DragImages.ContainsKey(content));
		DragImages[content] = this;
		EventBus.Subscribe<ResetCountEvent>(ResetCount);
		Count=initial_count;
		gameState = GameObject.Find("GameState").GetComponent<GameState>();
		selectionImage = transform.Find("Selection").GetComponent<Image>();
		selectionImage.enabled = false;
		Debug.Assert(selectionImage != null);
	}
	public void ResetCount(ResetCountEvent e)
	{
		Count = initial_count;
	}
	public void OnBeginDrag(PointerEventData eventData)
	{
		if (count > 0)
		{
			GameState.shown_drag_images = true;
			CurrentContentType = contentType;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			// Get the direction of the ray
			Vector3 rayDirection = ray.direction;

			Vector3 instantiatePos = ray.origin + rayDirection * rayDistance;
			Transform gridMatrixTransform = GridMatrix.Current.transform;

			instantiatedPreview = ContentInstantiator.Inst.InstantiateContent(content, gridMatrixTransform, instantiatePos, false, 0);
			Util.SetLayerRecursively(instantiatedPreview.gameObject, "MaskLayer");
			Count--;
			// DragHelper();
			Current = this;
		}
		else
		{
			Current = null;
		}
	}
	void DragHelper()
	{
		// Debug.Log("Drag helper called");
		if (instantiatedPreview == null)
		{
			return;
		}
		// awkward fix
		instantiatedPreview.transform.localRotation = Quaternion.identity;

		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		// Get the direction of the ray
		Vector3 rayDirection = ray.direction;
		if (GridMatrix.SelectedGrid != null)
		{
			instantiatedPreview.MoveGlobal(GridMatrix.SelectedGrid.transform.position, "Drag helper clamp grid");
			// instantiatedPreview.transform.position = selectedGrid.transform.position;
			Util.SetLayerRecursively(instantiatedPreview.gameObject, "ContentCrate");
		}
		else
		{
			Vector3 newPos = ray.origin + rayDirection * rayDistance;
			instantiatedPreview.MoveGlobal(newPos, "Drag helper free move");
			// instantiatedPreview.transform.position = newPos;
			Util.SetLayerRecursively(instantiatedPreview.gameObject, "MaskLayer");
		}
	}
	public void OnDrag(PointerEventData eventData)
	{
		DragHelper();
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		if (instantiatedPreview == null)
		{
			return;
		}
		// Enable raycast again
		// canvasGroup.blocksRaycasts = true;
		if (GridMatrix.SelectedGrid != null)
		{
			Util.SetLayerRecursively(instantiatedPreview.gameObject, "ContentCrate");
			GridMatrix.Current.AddContent(GridMatrix.SelectedGrid, contentType, instantiatedPreview);
			instantiatedPreview = null;
		}
		else
		{
			Count++;
			Destroy(instantiatedPreview.gameObject);
		}
		GridMatrix.SelectedGrid = null;
		CurrentContentType = Util.ContentType.None;
	}
	public void OnClick()
	{
		if (count > 0)
		{
			GameState.shown_drag_images = true;
			ToastManager.Toast("Drag!");
			Current = this;
		}
		else
		{
			Current = null;
		}
	}
}
