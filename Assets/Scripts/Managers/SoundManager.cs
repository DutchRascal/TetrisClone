﻿using UnityEngine;

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
        m_backgroundMusic,
        m_errorSound,
        m_holdSound,
        m_levelUpSound;
    public AudioSource m_musicSource;
    public AudioClip[] m_musicClips;
    public AudioClip[] m_vocalClips;
    public AudioClip m_gameOverVocalClip;
    public IconToggle m_musicIconToggle;
    public IconToggle m_fxIconToggle;


    private void Start()
    {
        PlayBackgroundMusic(GetRandomClip(m_musicClips));
    }

    private void Update()
    {
    }

    public void PlayBackgroundMusic(AudioClip musicClip)
    {
        if (!m_musicEnabled || !musicClip || !m_musicSource) { return; }
        m_musicSource.Stop();
        m_musicSource.clip = musicClip;
        m_musicSource.volume = m_musicVolume;
        m_musicSource.loop = true;
        m_musicSource.Play();
    }

    void UpdateMusic()
    {
        if (m_musicSource.isPlaying != m_musicEnabled)
        {
            if (m_musicEnabled)
            {
                PlayBackgroundMusic(m_backgroundMusic);
            }
            else
            {
                m_musicSource.Stop();
            }
        }
    }

    public void ToggleMusic()
    {
        m_musicEnabled = !m_musicEnabled;
        UpdateMusic();
        if (m_musicIconToggle)
        {
            m_musicIconToggle.ToggleIcon(m_musicEnabled);
        }
    }

    public AudioClip GetRandomClip(AudioClip[] clips)
    {
        return clips[Random.Range(0, clips.Length)];
    }

    public void ToggleFX()
    {
        m_fxEnabled = !m_fxEnabled;
        if (m_fxIconToggle)
        {
            m_fxIconToggle.ToggleIcon(m_fxEnabled);
        }
    }
}
