using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public sealed class Inventory : MonoBehaviour, IHasChanged
{
    [SerializeField]
    public Transform panel;

    [SerializeField]
    public Text inventoryText;

    public void HasChanged()
    {
        var stringBuilder = new StringBuilder();
        foreach (Transform slotTransform in panel)
        {
            var item = slotTransform.GetComponent<Slot>().item;
            if (item != null)
            {
                stringBuilder.Append(item.name);
                stringBuilder.Append(" ");
            }
        }

        inventoryText.text = stringBuilder.ToString();
    }

    // Use this for initialization
    void Start()
    {
        HasChanged();
    }
}
