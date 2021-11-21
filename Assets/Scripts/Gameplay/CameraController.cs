using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject m_target;

    private Vector3 m_offset;

    // Start is called before the first frame update
    void Start()
    {
        m_offset = transform.position - m_target.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = m_target.transform.position + m_offset;
    }
}
