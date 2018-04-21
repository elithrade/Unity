using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
    public GameObject GameOverScreen;
    public Text SecondsSurvivedText;

    private bool _gameOver;
    private PlayerController _playerController;

    void Start()
    {
        _playerController = FindObjectOfType<PlayerController>();
        _playerController.OnPlayerDeath += OnGameOver;
    }

    void Destroy()
    {
        _playerController.OnPlayerDeath -= OnGameOver;
    }

    void Update ()
    {
        if (_gameOver && Input.GetKeyDown(KeyCode.Space))
        {
            // Reload the scene by it's index
            SceneManager.LoadScene(0);
            _gameOver = false;
        }
    }

    private void OnGameOver()
    {
        SecondsSurvivedText.text = Mathf.RoundToInt(Time.timeSinceLevelLoad).ToString();
        GameOverScreen.SetActive(true);
        _gameOver = true;
    }
}
