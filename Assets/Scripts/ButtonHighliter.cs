using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHighlighter : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private bool isHighlighted = false;

    // This method is called when the mouse pointer enters the button area
    public void OnPointerEnter(PointerEventData eventData)
    {
        isHighlighted = true;
        Debug.Log("Button is highlighted");
    }

    // This method is called when the mouse pointer exits the button area
    public void OnPointerExit(PointerEventData eventData)
    {
        isHighlighted = false;
        Debug.Log("Button is no longer highlighted");
    }

    // Use this method to check the highlight status
    public bool IsHighlighted()
    {
        return isHighlighted;
    }
}
