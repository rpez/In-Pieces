using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    // This function gets called every time a dialog action is taken
    public void UpdateSounds()
    {
        if (GameManager.Instance.GetStateValue<bool>("EARS_QUEST_LEARNED_OF_STEREO"))
        {

        }
    }
}
