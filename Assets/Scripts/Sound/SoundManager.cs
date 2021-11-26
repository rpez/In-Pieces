using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    private FMOD.Studio.EventInstance m_stereoInstance;

    private bool m_musicPlaying;

    void Start()
    {
        // fmod stereo Music instance
        //m_stereoInstance = FMODUnity.RuntimeManager.CreateInstance("event:/Stereo");

    }

    // This function gets called every time a dialog action is taken
    public void UpdateSounds()
    {
        // The m_musicPlaying boolean is to ensure that the music won't start on top of already playing music
        if (!m_musicPlaying && GameManager.Instance.GetStateValue<bool>("STEREO_IS_ON"))
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/metalSalsa");
            m_musicPlaying = true;
        }
            
        if (m_musicPlaying && !GameManager.Instance.GetStateValue<bool>("STEREO_IS_ON"))
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/StereoOff");
            m_musicPlaying = false;
        }
    }
}
