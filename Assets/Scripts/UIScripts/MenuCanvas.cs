using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MenuCanvas : MonoBehaviour
{
    IEnumerator GoToGameHelper()
    {
        yield return BlackoutCanvas.Inst.Blackout(1.5f, 0.0f, 1.0f);
		SceneManager.LoadScene(1);
	}
    public void GoToGame()
    {
        StartCoroutine(GoToGameHelper());
    }
}
