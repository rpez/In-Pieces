using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    private NavMeshAgent m_navMeshAgent;
    private Interactable m_currentTargetInteractable;

    private bool m_movementEnabled = true;

    public void OnTargetInteractableReached()
    {
        m_navMeshAgent.SetDestination(transform.position);
    }

    // Sets a flag that determines whether the player is registering movement commands
    public void SetMovementActive(bool active)
    {
        m_movementEnabled = active;
    }

    // Start is called before the first frame update
    void Start()
    {
        m_navMeshAgent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        // If movement is disabled, return
        if (!m_movementEnabled) return;

        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out var hit))
        {
            // Movement only
            if (Input.GetMouseButtonDown(1))
            {
                m_navMeshAgent.SetDestination(hit.point);
                CancelCurrentInteractionTarget();
            }
            // Interaction
            else if (Input.GetMouseButtonDown(0))
            {
                if (hit.transform.tag == "Interactable")
                {
                    CancelCurrentInteractionTarget();

                    Interactable obj = hit.transform.GetComponent<Interactable>();
                    obj.OnClicked();
                    m_navMeshAgent.SetDestination(obj.m_interactionPoint.transform.position);

                    m_currentTargetInteractable = obj;
                }
            }
        }
    }

    // Called when player clicks something else to cancecl current pathing towards interaction target
    private void CancelCurrentInteractionTarget()
    {
        if (m_currentTargetInteractable != null)
        {
            m_currentTargetInteractable.CancelClicked();
            m_currentTargetInteractable = null;
        }
    }
}