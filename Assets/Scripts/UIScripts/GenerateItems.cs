using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GenerateItemsEvent
{

}

public class GenerateItems : MonoBehaviour
{
    public ContentPreview piggyPreview;
    public ContentPreview cratePreview;
    public ContentPreview wheelPreview;
    public Sprite piggySprite;
    public Sprite crateSprite;
    public Sprite wheelSprite;
    Dictionary<Util.Content, (ContentPreview, Sprite)> contentInfos;
    public GameObject imagePrefab;
    // Start is called before the first frame update
    Transform content;
    GameState gameState;
    void Start()
    {
        contentInfos = new()
        {
            {Util.Content.Pig, (piggyPreview, piggySprite) },
            {Util.Content.WoodenCrate, (cratePreview, crateSprite) },
            {Util.Content.Wheel, (wheelPreview, wheelSprite) },
        };

        content = transform.Find("Scroll View").Find("Viewport").Find("Content");
        gameState = GameObject.Find("GameState").GetComponent<GameState>();
        Debug.Assert(gameState != null);
        Debug.Assert(content != null);
        EventBus.Subscribe<GenerateItemsEvent>(Popularize);
        Popularize(new GenerateItemsEvent());
    }
    public void Popularize(GenerateItemsEvent e)
    {
		foreach (Transform child in content.transform)
		{
			Destroy(child.gameObject);
		}
        var items = Util.LevelItems[gameState.current_level_num];
        foreach((var c, int count) in items)
        {
            GameObject image = Instantiate(imagePrefab, content.transform);
            image.transform.parent = content.transform;
            var contentType = Util.ContentInfos[c];

            ImageDragHandler imageDragHandler = image.GetComponent<ImageDragHandler>();
            Debug.Assert(imageDragHandler != null);
            imageDragHandler.content = c;
            imageDragHandler.contentType = contentType;
            (var contentPreview, var contentSprite) = contentInfos[c];
            imageDragHandler.contentPreview = contentPreview;
            imageDragHandler.initial_count = count;
            Debug.Assert(imageDragHandler.contentPreview != null);
            Image img = image.GetComponent<Image>();
            Debug.Assert(img != null);
            img.sprite = contentSprite;
            Debug.Assert(img.sprite != null);
        }
	}

    // Update is called once per frame
    void Update()
    {
        
    }
}
