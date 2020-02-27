using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    Board m_gameBoard;
    Spawner m_spawner;
    Shape m_activeShape;
    float
        m_dropInterval = 0.1f,
        //m_dropInterval = 1f,
        m_timeToDrop,
        m_timeToNextKey;

    [Range(0.02f, 1f)]
    public float m_keyRepeatRate = 0.25f;

    // Start is called before the first frame update
    void Start()
    {
        m_timeToNextKey = Time.time;
        m_gameBoard = GameObject.FindObjectOfType<Board>();
        m_spawner = GameObject.FindObjectOfType<Spawner>();

        if (m_spawner)
        {
            if (m_activeShape == null)
            {
                m_activeShape = m_spawner.SpawnShape();
            }

            m_spawner.transform.position = Vectorf.Round(m_spawner.transform.position);
        }
        if (!m_gameBoard)
        {
            Debug.LogWarning("GAMECONTROLLER START: There is no game board defined!");
        }
        if (!m_spawner)
        {
            Debug.LogWarning("GAMECONTROLLER START: There is no spawner defined!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        PlayerInput();
        Spawn();
    }

    private void Spawn()
    {
        if (!m_gameBoard || !m_spawner)
        {
            return;
        }
        if (Time.time > m_timeToDrop)
        {
            m_timeToDrop = Time.time + m_dropInterval;
            if (m_activeShape)
            {
                m_activeShape.MoveDown();
                if (!m_gameBoard.IsValidPosition(m_activeShape))
                {
                    m_activeShape.MoveUp();
                    m_gameBoard.StoreShapeInGrid(m_activeShape);
                    if (m_spawner)
                    {
                        m_activeShape = m_spawner.SpawnShape();
                    }
                }
            }
        }
    }

    private void PlayerInput()
    {
        if (Input.GetButton("MoveRight") && (Time.time > m_timeToNextKey) || Input.GetButtonDown("MoveRight"))
        {
            m_activeShape.MoveRight();
            m_timeToNextKey = Time.time + m_keyRepeatRate;
            if (m_gameBoard.IsValidPosition(m_activeShape))
            {
                Debug.Log("Move right");
            }
            else
            {
                m_activeShape.MoveLeft();
                Debug.Log("Hit the right boundary");
            }
        }
    }
}
