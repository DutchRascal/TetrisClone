using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public bool m_musicEnabled = true;
    public bool m_fxEnabled = true;
    [Range(0, 1)]
    public float
        m_musicVolume = 1.0f,
        m_fxVolume = 1.0f;
    public AudioClip
        m_clearRowSound,
        m_moveSound,
        m_dropSound,
        m_gameOverSound,
        m_backgroundMusic;
}
