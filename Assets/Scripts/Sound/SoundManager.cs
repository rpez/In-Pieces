using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD;
using FMODUnity;

public class SoundManager : Singleton<SoundManager>
{
    private FMOD.Studio.EventInstance m_stereoInstance;
    private bool m_musicPlaying;

    void Start()
    {
        // fmod stereo Music instance
        // m_stereoInstance = FMODUnity.RuntimeManager.CreateInstance("event:/StereoSpeakerMusic");
        

    }

    // BOOLS
    // MUSIC PLAYS OR NOT
    public void UpdateSoundsBoolean(string parameterName, bool boolValue)
    {
        if (!m_musicPlaying &&
            parameterName.Equals("STEREO_IS_PLAYING") &&
            boolValue)
        {
            UnityEngine.Debug.Log("Fmod: Stereo Music Start");
           
            m_stereoInstance = FMODUnity.RuntimeManager.CreateInstance("event:/StereoSpeakerMusic");
            m_stereoInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
            m_stereoInstance.start();
            m_musicPlaying = true;
        }
        else if (m_musicPlaying &&
                 parameterName.Equals("STEREO_IS_PLAYING") &&
                 !boolValue)
        {
            UnityEngine.Debug.Log("Fmod: Stereo Music Stop");
            m_stereoInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            m_musicPlaying = false;
        }
        else if (parameterName.Equals("STEREO_IS_ON") && boolValue)
        {
            UnityEngine.Debug.Log("Fmod: Stereo Switch On");
            FMODUnity.RuntimeManager.PlayOneShot("event:/StereoON");
        }
        else if (parameterName.Equals("STEREO_IS_ON") && !boolValue)
        {
            UnityEngine.Debug.Log("Fmod: Stereo Switch Off");
            FMODUnity.RuntimeManager.PlayOneShot("event:/StereoOff");
        }
        else if (parameterName.Equals("STEREO_BASS_BOOST") && boolValue)
        {
            UnityEngine.Debug.Log("Fmod: Stereo Bass Boost On");
            FMODUnity.RuntimeManager.StudioSystem.setParameterByName("BassBoost", 1);
        }
        else if (parameterName.Equals("STEREO_BASS_BOOST") && !boolValue)
        {
            UnityEngine.Debug.Log("Fmod: Stereo Bass Boost Off");
            FMODUnity.RuntimeManager.StudioSystem.setParameterByName("BassBoost", 0);
        }
        else if (parameterName.Equals("HAS_EARS") && boolValue)
        {
            FMODUnity.RuntimeManager.StudioSystem.setParameterByName("hasEars", 1);
        }
    }



    // INTS
    public void UpdateSoundsInteger(string parameterName, int intValue)
    {
        if (parameterName.Equals("INTRO_DRUNKENNESS_LEVEL") && intValue == 1)
        {
            UnityEngine.Debug.Log("Fmod: Intro Drunkenness Level 1");
            FMODUnity.RuntimeManager.StudioSystem.setParameterByName("Chug", 1);
        }
        else if (parameterName.Equals("INTRO_DRUNKENNESS_LEVEL") && intValue == 2)
        {
            UnityEngine.Debug.Log("Fmod: Intro Drunkenness Level 2");
            FMODUnity.RuntimeManager.StudioSystem.setParameterByName("Chug", 2);
        }
        else if (parameterName.Equals("INTRO_DRUNKENNESS_LEVEL") && intValue == 3)
        {
            UnityEngine.Debug.Log("Fmod: Intro Drunkenness Level 2");
            FMODUnity.RuntimeManager.StudioSystem.setParameterByName("Chug", 3);
        }
    }
}


/*   NOTES LUCIEN
 * // public bool STEREO_IS_ON { get; set; } = false;
        // public bool STEREO_BASS_BOOST { get; set; } = false;
        // public bool STEREO_IS_PLAYING { get; set; } = false;






     */