using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ImageDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
	public float rayDistance = 5.0f;
	GameObject instantiatedObject = null;
	GridCell selectedGrid;
	public ContentPreview contentPreview;
	private RectTransform rectTransform;
	private CanvasGroup canvasGroup;
	GridMatrix gridMatrix;
	public Util.ContentType contentType;
	public Util.Content content;
	void Awake()
	{
		rectTransform = GetComponent<RectTransform>();
		canvasGroup = GetComponent<CanvasGroup>();
	}
	private void Start()
	{
		gridMatrix = GameObject.Find("GridMatrix").GetComponent<GridMatrix>();
		gridMatrix.GridSelectionChanged += OnSelectionChanged;
	}
	void OnSelectionChanged(GridCell selection)
	{
		selectedGrid = selection;
		if (instantiatedObject != null)
		{
			DragHelper();
		}
	}
	public void OnBeginDrag(PointerEventData eventData)
	{
		gridMatrix.currentContentType = contentType;
		// Disable raycast to allow dropping
		// canvasGroup.blocksRaycasts = false;
		// Debug.Log("On begin drag");
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		// Get the direction of the ray
		Vector3 rayDirection = ray.direction;

		Vector3 instantiatePos = ray.origin + rayDirection * rayDistance;
		GameObject prefab = contentPreview.gameObject;
		instantiatedObject = Instantiate(prefab, instantiatePos, Quaternion.identity);
	}
	void DragHelper()
	{
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
		gridMatrix.currentContentType = Util.ContentType.None;
		// Enable raycast again
		// canvasGroup.blocksRaycasts = true;
		if (selectedGrid != null)
		{
			gridMatrix.AddContent(contentType, instantiatedObject.GetComponent<ContentPreview>());
			instantiatedObject = null;
		}
		else
		{
			Destroy(instantiatedObject);
		}
	}
}
