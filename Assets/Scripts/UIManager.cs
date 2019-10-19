﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private Text _scoreText, _gameOverText, _restartText;
    [SerializeField]
    private Sprite[] _livesSprite;
    [SerializeField]
    private Image _livesImg;
    [SerializeField]
    private Player _player;
    [SerializeField]
    private float _flickerDelay = 0.5f;
    private bool _isGameOver = false;


    // Start is called before the first frame update
    void Start()
    {
        _gameOverText.gameObject.SetActive(false);
        _restartText.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (_player)
        {
            _scoreText.text = ($"Score: {_player.GetScore()}");
        }
        _gameOverText.gameObject.SetActive(GameOver(_player.GetLives()));

        if (_isGameOver)
        {
            StartCoroutine(GameOverFlickerRoutine());
            _restartText.gameObject.SetActive(true);
            NewGame();
        }
        else
        {
            _restartText.gameObject.SetActive(false);
        }

        QuitGame();
    }

    public void UpdateLives(int lives)
    {
        _livesImg.sprite = _livesSprite[lives];
    }

    public bool GameOver(int lives)
    {
        if (lives <= 0) {
            _isGameOver = true;
            return _isGameOver;
        }
        return _isGameOver;
    }

    IEnumerator GameOverFlickerRoutine()
    {
        while (_isGameOver)
        {
            yield return new WaitForSeconds(_flickerDelay);
            _gameOverText.text = "";
            yield return new WaitForSeconds(_flickerDelay);
            _gameOverText.text = "GAME OVER";
        }
    }

    void NewGame()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            SceneManager.LoadScene(0);
        }
    }

    private void QuitGame()
    {
        if(Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Q))
        {
            Application.Quit();
        }
    }
}
