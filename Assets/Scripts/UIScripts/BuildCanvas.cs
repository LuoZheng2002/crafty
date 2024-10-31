using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuildCanvas : MonoBehaviour
{
    public static BuildCanvas Inst
    {
        get { Debug.Assert(inst != null, "Build Canvas not set");return inst; }
    }
    static BuildCanvas inst;
    Transform itemBar;
    Transform rightButton;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(inst == null, "Build Canvas Already Set");
        inst = this;
        Util.Delay(this, () =>
        {
            gameObject.SetActive(false);
        });
        itemBar = transform.Find("ItemBar");
        rightButton = itemBar.Find("Right");
        Debug.Assert(rightButton != null);
    }
    public void Show()
    {
        gameObject.SetActive(true);
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }
    int items_offset = 0;
    List<DragImage> nonzero_images;
    void UpdateImages()
    {
		DragImage.DetachAll();
        rightButton.SetParent(null);
        Queue<DragImage> emptyImages = new Queue<DragImage>(DragImage.EmptyImages.Values);
        for (int i = 0; i < 5; i++)
        {
            if (i + items_offset < nonzero_images.Count)
            {
                nonzero_images[i + items_offset].transform.SetParent(itemBar);
            }
            else
            {
                Debug.Assert(emptyImages.Count > 0);
                emptyImages.Dequeue().transform.SetParent(itemBar);
            }
        }
        rightButton.SetParent(itemBar);
	}
    public void OnRightClicked()
    {
        if (nonzero_images.Count >= items_offset+1+5)
        {
            items_offset++;
            UpdateImages();
        }
    }
    public void OnLeftClicked()
    {
        if (items_offset >0)
        {
            items_offset--;
            UpdateImages();
        }
    }
	public void InitializeItems()
    {
        nonzero_images = new();
        foreach (var dragImage in DragImage.DragImages)
        {
            if (dragImage.Value.Count > 0)
            {
                nonzero_images.Add(dragImage.Value);
            }
        }
        items_offset = 0;
        UpdateImages();
    }
}
