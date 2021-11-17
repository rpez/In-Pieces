using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    private NavMeshAgent m_navMeshAgent;
    private Interactable m_currentTargetInteractable;

    public void OnTargetInteractableReached()
    {
        m_navMeshAgent.SetDestination(transform.position);
    }

    // Start is called before the first frame update
    void Start()
    {
        m_navMeshAgent = GetComponent<NavMeshAgent>();
        DialogueUIConsole m_exampleUIConsole = gameObject.AddComponent<DialogueUIConsole>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out var hit))
        {
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

                    Interactable obj = hit.transform.GetComponent<Interactable>();
                    obj.OnClicked();
                    m_navMeshAgent.SetDestination(obj.m_interactionPoint.transform.position);

                    m_currentTargetInteractable = obj;
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
}