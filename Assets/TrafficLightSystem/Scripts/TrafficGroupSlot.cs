using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

public class TrafficGroupSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public TrafficLightGroup groupRef;
    public TextMeshProUGUI label;

    public Transform originalParent;
    private Vector3 originalPosition;

    private bool droppedOnValidZone = false;

    public void Setup(TrafficLightGroup group)
    {
        groupRef = group;
        label.text = group.groupName;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        originalPosition = transform.position;

        GetComponent<CanvasGroup>().blocksRaycasts = false;

        // Flag'i resetle
        droppedOnValidZone = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        GetComponent<CanvasGroup>().blocksRaycasts = true;

        if (!droppedOnValidZone)
        {
            // Geri dön
            transform.SetParent(originalParent);
            transform.position = originalPosition;
        }
    }

    public void MarkAsDropped()
    {
        droppedOnValidZone = true;
    }
}
