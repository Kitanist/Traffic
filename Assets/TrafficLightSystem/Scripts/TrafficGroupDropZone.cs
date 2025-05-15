using UnityEngine;
using UnityEngine.EventSystems;

public class TrafficGroupDropZone : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        var incoming = eventData.pointerDrag?.transform;
        if (incoming == null) return;

        // Eðer bu dropzone’da zaten bir çocuk varsa, onunla yer deðiþtir
        if (transform.childCount > 0 && transform != incoming.GetComponent<TrafficGroupSlot>().originalParent)
        {
            Debug.LogError("Hoop çocuk var zaten birader");
            var existing = transform.GetChild(0);

            // Mevcut çocuðun parent'ýný al
            var previousParent = incoming.GetComponent<TrafficGroupSlot>().originalParent;

            // Eskiyi geri gönder
            existing.SetParent(previousParent);
            // existing.SetSiblingIndex(incoming.GetSiblingIndex());
            existing.localPosition = Vector3.zero;
            existing.GetComponent<TrafficGroupSlot>().originalParent = previousParent;
        }

        // Yeni child'ý yerleþtir
        incoming.SetParent(transform);
        incoming.localPosition = Vector3.zero;
        incoming.GetComponent<TrafficGroupSlot>().originalParent = transform;
    }
}
