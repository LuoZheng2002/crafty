using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelButton : MonoBehaviour
{
    public int level_num = 1;
	public void OnLevelClicked()
	{
		if (level_num > GameState.unlocked_levels)
		{
			return;
		}
		GameState.start_level = level_num;
		SceneManager.LoadScene(1);
	}
}
