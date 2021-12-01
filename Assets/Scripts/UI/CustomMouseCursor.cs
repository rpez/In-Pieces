using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomMouseCursor : MonoBehaviour
{
    void Awake()
    {
        Cursor.visible = false;
    }

    void Update()
    {
        Vector2 cursorPos = Input.mousePosition;
        transform.position = cursorPos;
    }
}