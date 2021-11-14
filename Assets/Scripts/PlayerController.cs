using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    public string hit;

    private NavMeshAgent m_navMeshAgent;
    private Interactable m_currentTargetInteractable;
    private Interactable m_currentHovered;

    public void OnTargetInteractableReached()
    {
        m_navMeshAgent.SetDestination(transform.position);
    }

    // Start is called before the first frame update
    void Start()
    {
        m_navMeshAgent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out var hit))
        {
            if (hit.transform.tag == "Interactable")
            {
                if (m_currentHovered != null && m_currentHovered.gameObject != hit.transform.gameObject)
                {
                    CancelCurrentHover();
                }
                if (m_currentHovered == null)
                {
                    Interactable script = hit.transform.GetComponent<Interactable>();
                    script.OnHoverEnter();
                    m_currentHovered = script;
                }
            }
            else if (m_currentHovered != null)
            {
                CancelCurrentHover();
            }
            if (Input.GetMouseButtonDown(1))
            {
                m_navMeshAgent.SetDestination(hit.point);
                CancelCurrentInteractionTarget();
            }
            else if (Input.GetMouseButtonDown(0))
            {
                if (hit.transform.tag == "Interactable")
                {
                    CancelCurrentInteractionTarget();

                    Interactable script = hit.transform.GetComponent<Interactable>();
                    script.OnClicked();
                    m_navMeshAgent.SetDestination(script.m_interactionPoint.transform.position);

                    m_currentTargetInteractable = script;
                }
            }
        }
    }

    private void CancelCurrentInteractionTarget()
    {
        if (m_currentTargetInteractable != null)
        {
            m_currentTargetInteractable.CancelClicked();
            m_currentTargetInteractable = null;
        }
    }
    private void CancelCurrentHover()
    {
        if (m_currentHovered != null)
        {
            m_currentHovered.OnHoverExit();
            m_currentHovered = null;
        }
    }
}
