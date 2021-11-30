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
    public void UpdateSounds(string parameterName, bool boolValue)
    {
        if (!m_musicPlaying &&
            parameterName.Equals("STEREO_IS_PLAYING") &&
            boolValue)
        {
            m_stereoInstance.start();
            m_musicPlaying = true;
        }
        else if (m_musicPlaying &&
                 parameterName.Equals("STEREO_IS_PLAYING") &&
                 !boolValue)
        {
            m_stereoInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            m_musicPlaying = false;
        }
        else if (parameterName.Equals("STEREO_IS_ON") && boolValue)
            FMODUnity.RuntimeManager.PlayOneShot("event:/StereoON");
        else if (parameterName.Equals("STEREO_IS_ON") && !boolValue)
            FMODUnity.RuntimeManager.PlayOneShot("event:/StereoOff");
        else if (parameterName.Equals("STEREO_BASS_BOOST") && boolValue)
            FMODUnity.RuntimeManager.StudioSystem.setParameterByName("BassBoost", 1);
        else if (parameterName.Equals("STEREO_BASS_BOOST") && !boolValue)
            FMODUnity.RuntimeManager.StudioSystem.setParameterByName("BassBoost", 0);
    }
}


/*   NOTES LUCIEN
 * // public bool STEREO_IS_ON { get; set; } = false;
        // public bool STEREO_BASS_BOOST { get; set; } = false;
        // public bool STEREO_IS_PLAYING { get; set; } = false;






     */