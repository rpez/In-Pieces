using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionInteractable : Interactable
{
    public string m_trackedState;
    public Transform m_waypoint;

    private PlayerController m_actor;
    private bool m_currentState;

    // Called by the interaction trigger when player enters it
    // Initiates whatever will happen when interacted with
    public override void OnInteract(PlayerController actor)
    {
        base.OnInteract(actor);
        m_actor = actor;

        m_currentState = GameManager.Instance.State.UPSTAIRS;
    }

    // Called when interaction end, disables the trigger
    public override void OnExitInteraction()
    {
        base.OnExitInteraction();

        if (m_currentState != GameManager.Instance.State.UPSTAIRS)
        {
            m_currentState = GameManager.Instance.State.UPSTAIRS;
            m_actor.TravelToWaypoint(m_waypoint.position);
            m_actor = null;
        }
    }
}
