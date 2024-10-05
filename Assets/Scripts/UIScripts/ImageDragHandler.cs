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
// to do: add content event
public class AddContentEvent
{
	public Util.ContentType contentType;
	public ContentPreview content;
    public AddContentEvent(Util.ContentType contentType, ContentPreview content)
    {
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
		count = initial_count;
		Text.text = count.ToString();

		gameState = GameObject.Find("GameState").GetComponent<GameState>();
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
	void OnSelectionChanged(GridSelectionChangedEvent e)
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
			EventBus.Publish(new ContentTypeChangedEvent(contentType));
			// gridMatrix.currentContentType = contentType;
			// Disable raycast to allow dropping
			// canvasGroup.blocksRaycasts = false;
			// Debug.Log("On begin drag");
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			// Get the direction of the ray
			Vector3 rayDirection = ray.direction;

			Vector3 instantiatePos = ray.origin + rayDirection * rayDistance;
			GameObject prefab = contentPreview.gameObject;
			Transform gridMatrixTransform = gameState.CurrentGridMatrix.transform;
			instantiatedObject = Instantiate(prefab, instantiatePos, Quaternion.identity);
			instantiatedObject.transform.parent = gridMatrixTransform;
			instantiatedObject.transform.localRotation = Quaternion.identity;
			EventBus.Publish(new ItemCountChangeEvent(content, -1));
		}
	}
	void DragHelper()
	{
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
		}
		else
		{
			Vector3 newPos = ray.origin + rayDirection * rayDistance;
			instantiatedObject.transform.position = newPos;
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
		EventBus.Publish(new ContentTypeChangedEvent(Util.ContentType.None));
		// Enable raycast again
		// canvasGroup.blocksRaycasts = true;
		if (selectedGrid != null)
		{
			// gridMatrix.AddContent(contentType, instantiatedObject.GetComponent<ContentPreview>());
			EventBus.Publish(new AddContentEvent(contentType, instantiatedObject.GetComponent<ContentPreview>()));
			instantiatedObject = null;
		}
		else
		{
			EventBus.Publish(new ItemCountChangeEvent(content, 1));
			Destroy(instantiatedObject);
		}
	}
}
