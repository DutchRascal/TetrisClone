using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Board : MonoBehaviour
{
    public Transform m_emptySprite;
    //public int m_height = 30;
    public int m_height = 10;
    public int m_width = 10;

    private void Start()
    {
        DrawEmptyCells();
    }
    void DrawEmptyCells()
    {
        for (int x = 0; x < m_width; x++)
        {
            for (int y = 0; y < m_height; y++)
            {
                Vector3 position = new Vector3(x - 3.5f, y - 10.4f, 0);
                Instantiate(m_emptySprite, position, Quaternion.Euler(0, 0, 0));
            }
        }
    }
}
