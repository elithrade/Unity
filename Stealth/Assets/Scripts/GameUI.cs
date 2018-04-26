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
        Guard.OnPlayerSpotted += OnPlayerSpotted;
    }

    private void OnPlayerSpotted()
    {
        OnGameOver(GameLoseUI);
    }

    private void OnGameOver(GameObject gameOverUI)
    {
        _gameIsOver = true;
        gameOverUI.SetActive(true);
    }

    void Update()
    {
        if (_gameIsOver)
        {
            if (Input.GetKeyDown(KeyCode.Space))
                SceneManager.LoadScene(0);
        }
    }
}