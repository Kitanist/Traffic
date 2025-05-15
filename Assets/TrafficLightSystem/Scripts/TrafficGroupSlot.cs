using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

public class TrafficGroupSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public TrafficLightGroup groupRef;
    public TextMeshProUGUI label;

    public Transform originalParent;
    
    private int originalIndex;

    public void Setup(TrafficLightGroup group)
    {
        groupRef = group;
        label.text = group.groupName;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        
        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
      
        
        GetComponent<CanvasGroup>().blocksRaycasts = true;
    }
}
