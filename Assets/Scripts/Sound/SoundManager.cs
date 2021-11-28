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
        m_stereoInstance = FMODUnity.RuntimeManager.CreateInstance("event:/StereoSpeakerMusic");


    }

    // MUSIC PLAYS OR NOT
    public void UpdateSounds()
    {
        // The m_musicPlaying boolean is to ensure that the music won't start on top of already playing music
        if (!m_musicPlaying && GameManager.Instance.GetStateValue<bool>("STEREO_IS_PLAYING"))
        {
            m_stereoInstance.start ();
            m_musicPlaying = true;
        }
            
        if (m_musicPlaying && !GameManager.Instance.GetStateValue<bool>("STEREO_IS_PLAYING"))
        {
            m_stereoInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            // m_stereoInstance.release();
            m_musicPlaying = false;
        }


        // ON and OFF Power Switch Stereo
        if (GameManager.Instance.GetStateValue<bool>("STEREO_IS_ON"))
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/StereoON");
        }
        else 
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/StereoOff");
        }


        // BASS BOOST
        if (GameManager.Instance.GetStateValue<bool>("STEREO_BASS_BOOST"))
        {
            FMODUnity.RuntimeManager.StudioSystem.setParameterByName("BassBoost", 1);
        }
        else
        {
            FMODUnity.RuntimeManager.StudioSystem.setParameterByName("BassBoost", 0);
        }






    }
}


/*   NOTES LUCIEN
 * // public bool STEREO_IS_ON { get; set; } = false;
        // public bool STEREO_BASS_BOOST { get; set; } = false;
        // public bool STEREO_IS_PLAYING { get; set; } = false;






     */