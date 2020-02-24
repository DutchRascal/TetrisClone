using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    Board m_gameBoard;
    Spawner m_spawner;

    // Start is called before the first frame update
    void Start()
    {
        m_gameBoard = GameObject.FindObjectOfType<Board>();
        m_spawner = GameObject.FindObjectOfType<Spawner>();
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

    }
}
