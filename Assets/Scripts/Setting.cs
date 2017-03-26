using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using XInputDotNetPure;

public class Setting : MonoBehaviour {

    public bool IsSinglePlayer;

    private void Awake() {
        DontDestroyOnLoad(gameObject);
    }

    public void StartSinglePlayer() {
        IsSinglePlayer = true;
        SceneManager.LoadScene("Default", LoadSceneMode.Single);
    }

    public void StartTwoPlayers() {
        IsSinglePlayer = false;
        SceneManager.LoadScene("Default", LoadSceneMode.Single);
    }

    public void Exit() {
        Application.Quit();

    }
}
