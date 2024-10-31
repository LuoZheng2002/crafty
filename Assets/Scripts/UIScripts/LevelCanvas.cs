using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelCanvas : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject levelButtonPrefab;
    void Start()
    {
        Transform panel = transform.Find("Panel");
		for (int i = 0; i < panel.childCount;i++)
        {
            Destroy(panel.GetChild(i).gameObject);
        }
		for (int i = 1;i <= Util.WaypointItems.Count;i++)
        {
            GameObject button = Instantiate(levelButtonPrefab);
            button.transform.SetParent(panel);
            Text text = button.transform.Find("Text").GetComponent<Text>();
            text.text = $"Level {i}";
            Debug.Assert(text != null);
            if (i <= GameState.unlocked_levels)
            {
                text.color = Color.black;
            }
            else
            {
                text.color = Color.gray;
            }
            LevelButton levelButton = button.GetComponent<LevelButton>();
            Debug.Assert(levelButton != null);
            levelButton.level_num = i;
        }
    }
   
}
