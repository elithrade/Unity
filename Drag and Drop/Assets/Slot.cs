using UnityEngine;
using UnityEngine.EventSystems;

public sealed class Slot : MonoBehaviour, IDropHandler
{
    public GameObject item
    {
        get
        {
            // Gets the current item in the slot, there is only 1 item really
            if (transform.childCount > 0)
            {
                return transform.GetChild(0).gameObject;
            }

            return null;
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (item == null)
        {
            // If slot is empty, set as parent of the item being dragged
            DragHandler.itemBeingDragged.transform.SetParent(transform);
            // ExecuteHierarchy calls all the game objects above the current one
            // Literally calls all the game objects implements IHasChanged interface then call HasChanged() on each one
            ExecuteEvents.ExecuteHierarchy<IHasChanged>(gameObject, null, (x, y) => x.HasChanged());
        }
    }
}
