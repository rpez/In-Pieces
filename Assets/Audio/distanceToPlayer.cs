using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD;
using FMODUnity;

public class distanceToPlayer : MonoBehaviour

{

    public GameObject player;
    private float distance;
    private FMOD.Studio.EventInstance instance;

    // Start is called before the first frame update
    void Start()
    {
        instance = FMODUnity.RuntimeManager.CreateInstance("event:/radio");
        instance.start();
    }

    // Update is called once per frame
    void Update()
    {

        distance = Vector3.Distance(transform.position, player.transform.position);

        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("distanceToPlayer", distance);

        // Debug
        UnityEngine.Debug.Log("PlayerToGoalDistance = " + distance);

    }
}