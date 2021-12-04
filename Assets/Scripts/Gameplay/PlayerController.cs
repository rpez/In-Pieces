using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    // The meshes for different body parts, set in editor
    public GameObject m_eyesMesh;
    public GameObject m_earsMesh;
    public GameObject m_handsMesh;
    public GameObject m_legsMesh;

    private NavMeshAgent m_navMeshAgent;
    private Interactable m_currentTargetInteractable;
    private Interactable m_currentHovered;

    private Animator m_animator;

    private bool m_movementEnabled = true;

    public void OnTargetInteractableReached()
    {
        m_navMeshAgent.SetDestination(transform.position);
    }

    // Sets a flag that determines whether the player is registering movement commands
    public void SetMovementActive(bool active)
    {
        SetWalkingAnimation(false);
        m_movementEnabled = active;
        m_navMeshAgent.enabled = active;
    }

    public void TravelToWaypoint(Vector3 location)
    {
        m_navMeshAgent.enabled = false;
        transform.position = location;
    }

    // Start is called before the first frame update
    void Start()
    {
        m_navMeshAgent = GetComponent<NavMeshAgent>();
        m_animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_navMeshAgent.enabled)
        {
            if (!m_navMeshAgent.pathPending)
            {
                if (m_navMeshAgent.remainingDistance <= m_navMeshAgent.stoppingDistance)
                {
                    if (!m_navMeshAgent.hasPath || m_navMeshAgent.velocity.sqrMagnitude == 0f)
                    {
                        SetWalkingAnimation(false);
                    }
                }
            }
        }

        // If movement is disabled, return
        if (!m_movementEnabled) return;

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
            // Movement only
            if (Input.GetMouseButtonDown(1))
            {
                m_navMeshAgent.SetDestination(hit.point);
                CancelCurrentInteractionTarget();

                SetWalkingAnimation(true);
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

                    SetWalkingAnimation(true);
                }
            }
        }
    }

    private void SetWalkingAnimation(bool active)
    {
        if (active)
        {
            m_animator.Play("Walk");
        }
        else
        {
            m_animator.Play("Idle");
        }
    }

    public void UpdateBodyParts()
    {
        m_eyesMesh.SetActive(GameManager.Instance.GetStateValue<bool>("HAS_EYES"));
        m_earsMesh.SetActive(GameManager.Instance.GetStateValue<bool>("HAS_EARS"));
        m_handsMesh.SetActive(GameManager.Instance.GetStateValue<bool>("HAS_HAND"));
        m_legsMesh.SetActive(GameManager.Instance.GetStateValue<bool>("HAS_LEGS"));
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

    private void CancelCurrentHover()
    {
        if (m_currentHovered != null)
        {
            m_currentHovered.OnHoverExit();
            m_currentHovered = null;
        }
    }
}