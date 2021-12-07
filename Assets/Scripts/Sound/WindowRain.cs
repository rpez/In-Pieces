using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD;
using FMODUnity;


public class WindowRain : MonoBehaviour
{
    public GameObject Player;
    private float distance;
    private FMOD.Studio.EventInstance instance;

    // Start is called before the first frame update
    void Start()
    {
        distance = 20;
        instance = FMODUnity.RuntimeManager.CreateInstance("event:/WindowRain");
        instance.start();
    }

    // Update is called once per frame
    void Update()
    {
        distance = Vector3.Distance(transform.position, Player.transform.position);

        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("PlayerToWindowDist", distance);
    }
}
