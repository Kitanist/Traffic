using UnityEngine.Events;
using UnityEngine;

public class UnityGameEventListener : MonoBehaviour , IGameEventListener
{
    [SerializeField]
    private GameEvent @event;

    [SerializeField]
    private UnityEvent response;
    public void OnEnable()
    {
        if (@event != null) @event.RegisterListener(this);
    }
    private void OnDisable()
    {
        @event.UnRegisterListener(this);
    }
    public void OnEventRaised()
    {
        response?.Invoke();
    }
}
