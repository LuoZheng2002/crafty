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

public class ContentUsedEvent
{
	public Util.Content content;
    public ContentUsedEvent(Util.Content content)
    {
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

public class ResetCountEvent
{

}
public class ImageDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
	public int initial_count = 5;
	int count = 0;
	public float rayDistance = 5.0f;
	ContentPreview instantiatedPreview = null;
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
	static ImageDragHandler current;
	public static ImageDragHandler Current
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
		EventBus.Subscribe<ItemCountChangeEvent>(OnItemCountChange);
		EventBus.Subscribe<ResetCountEvent>(ResetCount);
		EventBus.Subscribe<ContentRecycleEvent>(OnContentRecycle);
		EventBus.Subscribe<ContentUsedEvent>(OnContentUsed);
		count = initial_count;
		Text.text = count.ToString();

		gameState = GameObject.Find("GameState").GetComponent<GameState>();
		selectionImage = transform.Find("Selection").GetComponent<Image>();
		selectionImage.enabled = false;
		Debug.Assert(selectionImage != null);
	}
	public void ResetCount(ResetCountEvent e)
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
			Transform gridMatrixTransform = GridMatrix.Current.transform;

			instantiatedPreview = ContentInstantiator.Instance.InstantiateContent(content, gridMatrixTransform, instantiatePos, false, 0);
			// instantiatedObject.transform.parent = gridMatrixTransform;
			// instantiatedObject.transform.localRotation = Quaternion.identity;
			Util.SetLayerRecursively(instantiatedPreview.gameObject, "MaskLayer");
			EventBus.Publish(new ItemCountChangeEvent(content, -1));
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
			EventBus.Publish(new ItemCountChangeEvent(content, 1));
			Destroy(instantiatedPreview.gameObject);
		}
		EventBus.Publish(new ContentTypeChangedEvent(Util.ContentType.None));
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
	void OnContentRecycle(ContentRecycleEvent e)
	{
		if (e.content == content)
		{
			count++;
			text.text = count.ToString();
		}
	}
	void OnContentUsed(ContentUsedEvent e)
	{
		if (e.content == content)
		{
			count--;
			text.text = count.ToString();
		}
	}
}
