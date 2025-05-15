using UnityEngine;
using UnityEngine.EventSystems;

public class TrafficGroupDropZone : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        var incoming = eventData.pointerDrag?.transform;
        if (incoming == null) return;

        // E�er bu dropzone�da zaten bir �ocuk varsa, onunla yer de�i�tir
        if (transform.childCount > 0 && transform != incoming.GetComponent<TrafficGroupSlot>().originalParent)
        {
            Debug.LogError("Hoop �ocuk var zaten birader");
            var existing = transform.GetChild(0);

            // Mevcut �ocu�un parent'�n� al
            var previousParent = incoming.GetComponent<TrafficGroupSlot>().originalParent;

            // Eskiyi geri g�nder
            existing.SetParent(previousParent);
            // existing.SetSiblingIndex(incoming.GetSiblingIndex());
            existing.localPosition = Vector3.zero;
            existing.GetComponent<TrafficGroupSlot>().originalParent = previousParent;
        }

        // Yeni child'� yerle�tir
        incoming.SetParent(transform);
        incoming.localPosition = Vector3.zero;
        incoming.GetComponent<TrafficGroupSlot>().originalParent = transform;
    }
}
