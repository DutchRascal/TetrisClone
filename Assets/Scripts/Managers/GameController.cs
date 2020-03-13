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
    SoundManager m_soundManager;
    ScoreManager m_scoreManager;
    Ghost m_ghost;
    Holder m_holder;

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
    public IconToggle m_rotIconToggle;
    bool m_clockwise = true;
    public bool m_isPaused = false;
    public GameObject m_pausePanel;


    void Start()
    {
        m_gameBoard = GameObject.FindObjectOfType<Board>();
        m_spawner = GameObject.FindObjectOfType<Spawner>();
        m_soundManager = GameObject.FindObjectOfType<SoundManager>();
        m_scoreManager = GameObject.FindObjectOfType<ScoreManager>();
        m_ghost = GameObject.FindObjectOfType<Ghost>();
        m_holder = GameObject.FindObjectOfType<Holder>();

        m_timeToNextKeyLeftRight = Time.time + m_keyRepeatRateLeftRight;
        m_timeToNextKeyDown = Time.time + m_keyRepeatRateDown;
        m_timeToNextKeyRotate = Time.time + m_keyRepeatRateRotate;

        if (m_gameOverPanel) { m_gameOverPanel.SetActive(false); }
        if (m_pausePanel) { m_pausePanel.SetActive(false); }
        if (!m_gameBoard) { Debug.LogWarning("GAMECONTROLLER START: There is no game board defined!"); }
        if (!m_soundManager) { Debug.LogWarning("GAMECONTROLLER START: There is no sound manager defined!"); }
        if (!m_scoreManager) { Debug.LogWarning("GAMECONTROLLER START: There is no score manager defined!"); }
        if (!m_ghost) { Debug.LogWarning("GAMECONTROLLER START: There is no ghost!"); }
        if (!m_holder) { Debug.LogWarning("GAMECONTROLLER START: There is no holder!"); }
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
    }

    void Update()
    {
        if (!m_gameBoard || !m_spawner || !m_activeShape || m_GameOver || !m_soundManager || !m_scoreManager) { return; }
        PlayerInput();
    }

    private void LateUpdate()
    {
        if (m_ghost)
        {
            m_ghost.DrawGhost(m_activeShape, m_gameBoard);
        }
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
                PlaySound(m_soundManager.m_errorSound, 1f);
            }
            else
            {
                PlaySound(m_soundManager.m_moveSound, 1f);
            }
        }
        else if (Input.GetButton("MoveLeft") && (Time.time > m_timeToNextKeyLeftRight) || Input.GetButtonDown("MoveLeft"))
        {
            m_activeShape.MoveLeft();
            m_timeToNextKeyLeftRight = Time.time + m_keyRepeatRateLeftRight;
            if (!m_gameBoard.IsValidPosition(m_activeShape))
            {
                m_activeShape.MoveRight();
                PlaySound(m_soundManager.m_errorSound);
            }
            else
            {
                PlaySound(m_soundManager.m_moveSound);
            }
        }
        else if (Input.GetButtonDown("Rotate") && (Time.time > m_timeToNextKeyRotate))
        {
            //m_activeShape.RotateRight();
            m_activeShape.RotataClockWise(m_clockwise);
            m_timeToNextKeyRotate = Time.time + m_keyRepeatRateRotate;
            if (!m_gameBoard.IsValidPosition(m_activeShape))
            {
                m_activeShape.RotateLeft();
                PlaySound(m_soundManager.m_errorSound);
            }
            else
            {
                PlaySound(m_soundManager.m_moveSound);
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
        else if (Input.GetButtonDown("ToggleRot"))
        {
            ToggleRotDirection();
        }
        else if (Input.GetButtonDown("Pause"))
        {
            TogglePause();
        }
        else if (Input.GetButtonDown("Hold"))
        {
            Hold();
        }
    }

    void PlaySound(AudioClip clip, float volMultipler = 1.0f)
    {
        if (clip && m_soundManager.m_fxEnabled)
        {
            AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position, Mathf.Clamp(m_soundManager.m_fxVolume * volMultipler, 0.05f, 1f));
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
        PlaySound(m_soundManager.m_gameOverSound, 5f);
        PlaySound(m_soundManager.m_gameOverVocalClip);
    }

    private void LandShape()
    {
        m_activeShape.MoveUp();
        m_gameBoard.StoreShapeInGrid(m_activeShape);
        if (m_ghost)
        {
            m_ghost.Reset();
        }
        if (m_holder)
        {
            m_holder.m_canRelease = true;
        }
        m_activeShape = m_spawner.SpawnShape();
        m_timeToNextKeyLeftRight = Time.time;
        m_timeToNextKeyDown = Time.time;
        m_timeToNextKeyRotate = Time.time;
        m_gameBoard.ClearAllRows();
        PlaySound(m_soundManager.m_dropSound);
        if (m_gameBoard.m_completedRows > 0)
        {
            m_scoreManager.ScoreLines(m_gameBoard.m_completedRows);
            if (m_gameBoard.m_completedRows > 1)
            {
                AudioClip randomVocal = m_soundManager.GetRandomClip(m_soundManager.m_vocalClips);
                PlaySound(randomVocal);
            }
            PlaySound(m_soundManager.m_clearRowSound);
        }
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ToggleRotDirection()
    {
        m_clockwise = !m_clockwise;
        if (m_rotIconToggle)
        {
            m_rotIconToggle.ToggleIcon(m_clockwise);
        }
    }

    public void TogglePause()
    {
        if (m_GameOver) { return; }
        m_isPaused = !m_isPaused;
        if (m_pausePanel)
        {
            m_pausePanel.SetActive(m_isPaused);
        }
        if (m_soundManager)
        {
            m_soundManager.m_musicSource.volume = (m_isPaused) ? m_soundManager.m_musicVolume * 0.25f : m_soundManager.m_musicVolume;
        }
        Time.timeScale = (m_isPaused) ? 0 : 1;
    }

    public void Hold()
    {
        if (!m_holder.m_heldShape)
        {
            m_holder.Catch(m_activeShape);
            m_activeShape = m_spawner.SpawnShape();
            PlaySound(m_soundManager.m_holdSound);
        }
        else if (m_holder.m_canRelease)
        {
            Shape shape = m_activeShape;
            m_activeShape = m_holder.Release();
            m_activeShape.transform.position = m_spawner.transform.position;
            m_holder.Catch(shape);
            PlaySound(m_soundManager.m_holdSound);
        }
        else
        {
            Debug.LogWarning("HOLDER WARNING!  Wait for cool down");
            PlaySound(m_soundManager.m_errorSound);
        }
        if (m_ghost)
        {
            m_ghost.Reset();
        }
    }
}
