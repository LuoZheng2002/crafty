using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StarDisplayer : MonoBehaviour
{
	Text text;
	private void Start()
	{
		EventBus.Subscribe<StarCollectedEvent>(OnStarCollected);
		text = transform.Find("StarCount").GetComponent<Text>();
		text.text = "0";
	}
	void OnStarCollected(StarCollectedEvent e)
	{
		text.text = Starbox.CollectCount.ToString();
	}
}
