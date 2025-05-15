using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEventTrigger : MonoBehaviour
{
    [SerializeField] private GameEvent m_event;
    private void Start()
    {
        m_event.Raise();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            m_event.Raise();
        }
    }
}
