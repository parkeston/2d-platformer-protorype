using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonHolder : MonoBehaviour {

	public void PauseGame()
    {
        GameManager.instance.PauseGame();
    }

    public void StartGame()
    {
        GameManager.instance.StartGame();
    }

    public void QuitGame()
    {
        GameManager.instance.QuitGame();
    }
}
