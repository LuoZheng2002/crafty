using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ContentTypeChangedEvent
{
	public Util.ContentType contentType;
    public ContentTypeChangedEvent(Util.ContentType contentType)
    {
		this.contentType = contentType;   
    }
}
public class ContentRecycleEvent
{
	public Util.Content content;
    public ContentRecycleEvent(Util.Content content)
    {
		this.content = content;
    }
}

// to do: add content event
public class AddContentEvent
{
	public Util.ContentType contentType;
	public ContentPreview content;
	public GridCell selectedGrid;
	public AddContentEvent(GridCell selectedGrid, Util.ContentType contentType, ContentPreview content)
    {
		this.selectedGrid = selectedGrid;
		this.contentType = contentType;
		this.content = content;
    }
}
public class ItemCountChangeEvent
{
	public Util.Content content;
	public int delta;
	public ItemCountChangeEvent(Util.Content content, int delta)
	{
		
		this.content = content;
		this.delta = delta;
	}
}
public class ImageDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
	public int initial_count = 5;
	int count = 0;
	public float rayDistance = 5.0f;
	GameObject instantiatedObject = null;
	GridCell selectedGrid;
	public ContentPreview contentPreview;
	private RectTransform rectTransform;
	private CanvasGroup canvasGroup;
	// GridMatrix gridMatrix;
	public Util.ContentType contentType;
	public Util.Content content;
	Text text;
	GameState gameState;

	Image selectionImage;


	public float minScale = 0.8f;
	public float maxScale = 1.2f;
	public float scaleSpeed = 5.0f;
	Image image;
	private void OnEnable()
	{
		image = GetComponent<Image>();
		StartCoroutine(Scale());
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
		// gridMatrix = GameObject.Find("GridMatrix").GetComponent<GridMatrix>();
		// gridMatrix.GridSelectionChanged += OnSelectionChanged;
		EventBus.Subscribe<GridSelectionChangedEvent>(OnSelectionChanged);
		EventBus.Subscribe<ItemCountChangeEvent>(OnItemCountChange);
		EventBus.Subscribe<TrashEvent>(OnTrash);
		EventBus.Subscribe<ContentSelectionChangedEvent>(OnContentSelectionChanged);
		EventBus.Subscribe<ContentRecycleEvent>(OnContentRecycle);
		count = initial_count;
		Text.text = count.ToString();

		gameState = GameObject.Find("GameState").GetComponent<GameState>();
		selectionImage = transform.Find("Selection").GetComponent<Image>();
		selectionImage.enabled = false;
		Debug.Assert(selectionImage != null);
	}
	void OnContentSelectionChanged(ContentSelectionChangedEvent e)
	{
		selectionImage.enabled = false;
	}
	void OnTrash(TrashEvent e)
	{
		count = initial_count;
		Text.text = count.ToString();
	}
	void OnItemCountChange(ItemCountChangeEvent e)
	{
		if (e.content == content)
		{
			count += e.delta;
			Text.text = count.ToString();
		}
	}
	public void OnSelectionChanged(GridSelectionChangedEvent e)
	{
		selectedGrid = e.gridCell;
		if (instantiatedObject != null)
		{
			DragHelper();
		}
	}
	public void OnBeginDrag(PointerEventData eventData)
	{
		if (count > 0)
		{
			GameState.shown_drag_images = true;
			EventBus.Publish(new ContentTypeChangedEvent(contentType));
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			// Get the direction of the ray
			Vector3 rayDirection = ray.direction;

			Vector3 instantiatePos = ray.origin + rayDirection * rayDistance;
			GameObject prefab = contentPreview.gameObject;
			Transform gridMatrixTransform = gameState.CurrentGridMatrix.transform;
			instantiatedObject = Instantiate(prefab, Vector3.zero, Quaternion.identity);
			instantiatedObject.transform.parent = gridMatrixTransform;
			instantiatedObject.transform.localRotation = Quaternion.identity;
			Util.SetLayerRecursively(instantiatedObject, "MaskLayer");
			EventBus.Publish(new ItemCountChangeEvent(content, -1));
			// DragHelper();

			EventBus.Publish(new ContentSelectionChangedEvent(this));
			selectionImage.enabled = true;
		}
		else
		{
			EventBus.Publish(new ContentSelectionChangedEvent(null));
		}
	}
	void DragHelper()
	{
		// Debug.Log("Drag helper called");
		if (instantiatedObject == null)
		{
			return;
		}
		// awkward fix
		instantiatedObject.transform.localRotation = Quaternion.identity;

		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		// Get the direction of the ray
		Vector3 rayDirection = ray.direction;
		if (selectedGrid != null)
		{
			instantiatedObject.transform.position = selectedGrid.transform.position;
			Util.SetLayerRecursively(instantiatedObject, "ContentCrate");
		}
		else
		{
			Vector3 newPos = ray.origin + rayDirection * rayDistance;
			instantiatedObject.transform.position = newPos;
			Util.SetLayerRecursively(instantiatedObject, "MaskLayer");
		}
	}
	public void OnDrag(PointerEventData eventData)
	{
		DragHelper();
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		if (instantiatedObject == null)
		{
			return;
		}
		// Enable raycast again
		// canvasGroup.blocksRaycasts = true;
		if (selectedGrid != null)
		{
			Util.SetLayerRecursively(instantiatedObject, "ContentCrate");
			EventBus.Publish(new AddContentEvent(selectedGrid, contentType, instantiatedObject.GetComponent<ContentPreview>()));
			instantiatedObject = null;
			selectedGrid = null;
		}
		else
		{
			EventBus.Publish(new ItemCountChangeEvent(content, 1));
			Destroy(instantiatedObject);
		}
		EventBus.Publish(new ContentTypeChangedEvent(Util.ContentType.None));
	}
	public void OnClick()
	{
		if (count > 0)
		{
			GameState.shown_drag_images = true;
			ToastManager.Toast("Drag!");
			EventBus.Publish(new ContentSelectionChangedEvent(this));
			selectionImage.enabled = true;
		}
		else
		{
			EventBus.Publish(new ContentSelectionChangedEvent(null));
		}
	}
	void OnContentRecycle(ContentRecycleEvent e)
	{
		if (e.content == content)
		{
			count++;
			text.text = count.ToString();
		}
	}
}
