using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;

public class DragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Transform parentToReturnTo;
    public bool returnToParent;

    public void OnBeginDrag(PointerEventData eventData)
    {
        parentToReturnTo = this.transform.parent;
        returnToParent = true;
        var gameClientGUI = GameObject.Find("GameClientGUI").transform;
        this.transform.SetParent(gameClientGUI);

        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        this.transform.position = new Vector3(point.x, point.y, 0);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // return to starting parent
        if (returnToParent)
        {
            this.transform.SetParent(parentToReturnTo);
        }

        GetComponent<CanvasGroup>().blocksRaycasts = true;
    }

    
}
