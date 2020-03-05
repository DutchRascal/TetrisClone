using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    Board m_gameBoard;
    Spawner m_spawner;
    Shape m_activeShape;
    public float m_dropInterval = 0.9f;
    //m_dropInterval = 1f,
    float m_timeToDrop;

    [Range(0.02f, 1f)]
    public float m_keyRepeatRateLeftRight = 0.15f;
    float m_timeToNextKeyLeftRight;

    [Range(0.01f, 1f)]
    public float m_keyRepeatRateDown = 0.01f;
    float m_timeToNextKeyDown;

    [Range(0.02f, 1f)]
    public float m_keyRepeatRateRotate = 0.25f;
    float m_timeToNextKeyRotate;
    private bool m_GameOver = false;
    public GameObject m_gameOverPanel;
    SoundManager m_soundManager;

    void Start()
    {
        m_gameBoard = GameObject.FindObjectOfType<Board>();
        m_spawner = GameObject.FindObjectOfType<Spawner>();
        m_soundManager = GameObject.FindObjectOfType<SoundManager>();

        m_timeToNextKeyLeftRight = Time.time + m_keyRepeatRateLeftRight;
        m_timeToNextKeyDown = Time.time + m_keyRepeatRateDown;
        m_timeToNextKeyRotate = Time.time + m_keyRepeatRateRotate;

        if (m_gameOverPanel)
        {
            m_gameOverPanel.SetActive(false);
        }
        if (!m_gameBoard)
        {
            Debug.LogWarning("GAMECONTROLLER START: There is no game board defined!");
        }
        if (!m_soundManager)
        {
            Debug.LogWarning("GAMECONTROLLER START: There is no sound manager defined!");
        }
        if (!m_spawner)
        {
            Debug.LogWarning("GAMECONTROLLER START: There is no spawner defined!");
        }
        else
        {
            if (!m_activeShape)
            {
                m_activeShape = m_spawner.SpawnShape();
            }
            m_spawner.transform.position = Vectorf.Round(m_spawner.transform.position);
        }
        //if (m_soundManager.m_fxEnabled && m_soundManager.m_moveSound)
        //{
        //    AudioSource.PlayClipAtPoint(m_soundManager.m_moveSound, Camera.main.transform.position, m_soundManager.m_fxVolume);
        //}
    }

    void Update()
    {
        if (!m_gameBoard || !m_spawner || !m_activeShape || m_GameOver || !m_soundManager) { return; }
        PlayerInput();
    }

    private void PlayerInput()
    {
        if (Input.GetButton("MoveRight") && (Time.time > m_timeToNextKeyLeftRight) || Input.GetButtonDown("MoveRight"))
        {
            m_activeShape.MoveRight();
            m_timeToNextKeyLeftRight = Time.time + m_keyRepeatRateLeftRight;
            if (!m_gameBoard.IsValidPosition(m_activeShape))
            {
                m_activeShape.MoveLeft();
            }
        }
        else if (Input.GetButton("MoveLeft") && (Time.time > m_timeToNextKeyLeftRight) || Input.GetButtonDown("MoveLeft"))
        {
            m_activeShape.MoveLeft();
            m_timeToNextKeyLeftRight = Time.time + m_keyRepeatRateLeftRight;
            if (!m_gameBoard.IsValidPosition(m_activeShape))
            {
                m_activeShape.MoveRight();
            }
        }
        else if (Input.GetButtonDown("Rotate") && (Time.time > m_timeToNextKeyRotate))
        {
            m_activeShape.RotateRight();
            m_timeToNextKeyRotate = Time.time + m_keyRepeatRateRotate;
            if (!m_gameBoard.IsValidPosition(m_activeShape))
            {
                m_activeShape.RotateLeft();
            }
        }
        else if (Input.GetButton("MoveDown") && (Time.time > m_timeToNextKeyDown) || (Time.time > m_timeToDrop))
        {
            m_timeToDrop = Time.time + m_dropInterval;
            m_timeToNextKeyDown = Time.time + m_keyRepeatRateDown;
            m_activeShape.MoveDown();
            if (!m_gameBoard.IsValidPosition(m_activeShape))
            {
                if (m_gameBoard.IsOverLimit(m_activeShape))
                {
                    GameOver();
                }
                else
                {
                    LandShape();
                }
            }
        }
    }

    private void GameOver()
    {
        m_activeShape.MoveUp();
        m_GameOver = true;
        if (m_gameOverPanel)
        {
            m_gameOverPanel.SetActive(true);
        }
    }

    private void LandShape()
    {
        m_activeShape.MoveUp();
        m_gameBoard.StoreShapeInGrid(m_activeShape);
        m_activeShape = m_spawner.SpawnShape();
        m_timeToNextKeyLeftRight = Time.time;
        m_timeToNextKeyDown = Time.time;
        m_timeToNextKeyRotate = Time.time;
        m_gameBoard.ClearAllRows();
        if (m_soundManager.m_fxEnabled && m_soundManager.m_dropSound)
        {
            AudioSource.PlayClipAtPoint(m_soundManager.m_dropSound, Camera.main.transform.position, m_soundManager.m_musicVolume);
        }
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

}
