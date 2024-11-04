using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MenuCanvas : MonoBehaviour
{
    IEnumerator GoToGameHelper()
    {
        BlackoutCanvas.Inst.Blackout(1.5f, true);
        yield return new WaitForSeconds(1.5f);
		SceneManager.LoadScene(1);
	}
    public void GoToGame()
    {
        StartCoroutine(GoToGameHelper());
    }
}
