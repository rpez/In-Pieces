using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD;
using FMODUnity;


public class Hallway : MonoBehaviour
{
    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            //changing global parameter in fmod. you have to check global paramter in fmod edit paramter window
            FMODUnity.RuntimeManager.StudioSystem.setParameterByName("inHallway", 1);

            UnityEngine.Debug.Log("inHallway");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            FMODUnity.RuntimeManager.StudioSystem.setParameterByName("inHallway", 0);
            UnityEngine.Debug.Log("NotInHallway");
        }
    }
}
