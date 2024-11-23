using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

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
            DragImage.DetachAll();
            gameObject.SetActive(false);
        });
        itemBar = transform.Find("ItemBar");
        rightButton = itemBar.Find("Right");
        Debug.Assert(rightButton != null);
    }
    public void Show()
    {
        gameObject.SetActive(true);
        if (GameState.Inst.IntroducePreset)
        {
            GameState.Inst.IntroducePreset = false;
			// TODO for Gabriel: introduce preset
		}
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
    bool CanClickRight()
    {
        return nonzero_images.Count >= items_offset + 1 + 5;
	}
    bool CanClickLeft()
    {
        return items_offset > 0;
	}
    public void OnRightClicked()
    {
        if (CanClickRight())
        {
            items_offset++;
            UpdateImages();
        }
        button_scale.ScaleStop();
    }
    public void OnLeftClicked()
    {
        if (CanClickLeft())
        {
            items_offset--;
            UpdateImages();
        }
    }
	public void InitializeItems()
    {
        DragImage.ClearCountAll();
        var items = GameSave.Inventory;
        foreach (var item in items)
        {
            DragImage.DragImages[item.Key].SetInitialCount(item.Value);
        }
        if (GameSave.IsMainStory)
        {
            DragImage.DragImages[Util.Component.Partner].SetInitialCount(1);
        }
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
        Debug.Log("Tested");
        if (CanClickRight())
        {
            Debug.Log("Succeed");
            Util.Delay(this, 5, () =>
            {
                button_scale.ScaleStart();
            });
        }
    }
    public ButtonScale button_scale;
    public Sprite one_selected;
	public Sprite two_selected;
	public Sprite three_selected;
    public Sprite one_idle;
    public Sprite two_idle;
	public Sprite three_idle;
    public Image one_image;
    public Image two_image;
	public Image three_image;
	public void OnOneClicked()
    {
        one_image.sprite = one_selected;
		two_image.sprite = two_idle;
        three_image.sprite = three_idle;
        OnNumberClicked(0);
	}
    public void OnTwoClicked()
    {
        one_image.sprite = one_idle;
		two_image.sprite = two_selected;
		three_image.sprite = three_idle;
		OnNumberClicked(1);
	}
    public void OnThreeClicked()
    {
        one_image.sprite = one_idle;
		two_image.sprite = two_idle;
		three_image.sprite = three_selected;
		OnNumberClicked(2);
	}
    void OnNumberClicked(int number)
    {
        EventBus.Publish(new OtherItemSelectedEvent());
        Util.Delay(this, () =>
        {
            GameSave.CurrentMemory.Memorize();
            if (GameSave.GridMemories[number].Empty())
            {
                GameSave.GridMemories[number].MemCrates = (Util.Component[,,])GameSave.CurrentMemory.MemCrates.Clone();
				GameSave.GridMemories[number].MemAccessories = (Util.Component[,,])GameSave.CurrentMemory.MemAccessories.Clone();
				GameSave.GridMemories[number].MemLoads = (Util.Component[,,])GameSave.CurrentMemory.MemLoads.Clone();
                GameSave.GridMemories[number].AccessoryDirections = (int[,,])GameSave.CurrentMemory.AccessoryDirections.Clone();
			}
            GridMatrix.Inst.DesignNumber = number;
        });        
    }
    public GameObject design_numbers;
    public void ShowDesignNumbers()
    {
        design_numbers.SetActive(true);
	}
    public void HideDesignNumbers()
    {
        design_numbers.SetActive(false);
    }
}
