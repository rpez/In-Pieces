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

        m_currentState = GameManager.Instance.GetStateValue<bool>(m_trackedState);
    }

    // Called when interaction end, disables the trigger
    public override void OnExitInteraction()
    {
        if (m_currentState != GameManager.Instance.GetStateValue<bool>(m_trackedState))
        {
            m_currentState = GameManager.Instance.GetStateValue<bool>(m_trackedState);
            LevelManager.Instance.InitiateTransition(() =>
            {
                m_actor.TravelToWaypoint(m_waypoint.position);
            },
            () =>
            {
                base.OnExitInteraction();
                m_actor = null;
            });
        }
        else
        {
            base.OnExitInteraction();
            m_actor = null;
        }
    }
}
