using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuConfirm : MonoBehaviour
{
	private void Start()
	{
		gameObject.SetActive(false);
	}
	public void OnYesClicked()
    {
        gameObject.SetActive(false);
        SceneManager.LoadScene(0);
    }
    public void OnCancelClicked()
    {
        gameObject.SetActive(false);
    }
}
