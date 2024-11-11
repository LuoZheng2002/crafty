using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obsidian : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		GameState.Inst.StartCoroutine(TriggerHelper());
		gameObject.SetActive(false);
	}
	IEnumerator TriggerHelper()
	{
		yield return LineCanvas.Top.DisplayLine("Shirley", "Woohoo! We've got the enchanted obsidian!");
		yield return new WaitForSeconds(4.0f);
		LineCanvas.Bottom.Hide();
	}
}
