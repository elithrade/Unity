using UnityEngine;
using UnityEngine.EventSystems;

public sealed class DragHandler :
    MonoBehaviour,
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler
{
    public static GameObject itemBeingDragged;

    private Vector3 _startPosition;
    private Transform _startParent;

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Sets item being dragged to the current games object
        itemBeingDragged = gameObject;
        // Remember the start position
        _startPosition = transform.position;
        // Remember the start parent, slot holding the item
        _startParent = transform.parent;
        // Allows pass event through the item being dragged
        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        GetComponent<CanvasGroup>().blocksRaycasts = true;
        itemBeingDragged = null;

        if (transform.parent != _startParent)
        {
            return;
        }

        transform.position = _startPosition;
    }
}
