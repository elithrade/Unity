using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour
{
    public GameObject GameLoseUI;
    public GameObject GameWinUI;

    private bool _gameIsOver;

    private void Start()
    {
        // Note this script is linked to canvas
        // If canvas is set to inactive the script won't be called
        Guard.OnPlayerSpotted += OnPlayerSpotted;
        Player.OnGameFinished += OnGameFinshed;
    }

    private void OnGameFinshed()
    {
        OnGameOver(GameWinUI);
    }

    private void OnPlayerSpotted()
    {
        OnGameOver(GameLoseUI);
    }

    private void OnGameOver(GameObject gameOverUI)
    {
        _gameIsOver = true;

        gameOverUI.SetActive(true);
        Guard.OnPlayerSpotted -= OnPlayerSpotted;
        Player.OnGameFinished -= OnGameFinshed;
    }

    private void Update()
    {
        if (_gameIsOver)
        {
            if (Input.GetKeyDown(KeyCode.Space))
                SceneManager.LoadScene(0);
        }
    }
}