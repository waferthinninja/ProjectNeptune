using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class DropHandler : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public DropZoneType DropZoneType;

    public void OnPointerEnter(PointerEventData eventData)
    {
        //Debug.Log("OnPointerEnter");
        if (eventData.pointerDrag == null)
            return;

        //DragHandler d = eventData.pointerDrag.GetComponent<DragHandler>();
        //if (d != null)
        //{
           // d.placeholderParent = this.transform;
        //}
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //Debug.Log("OnPointerExit");
        if (eventData.pointerDrag == null)
            return;

        //DragHandler d = eventData.pointerDrag.GetComponent<DragHandler>();
       // if (d != null && d.placeholderParent == this.transform)
        //{
        //    d.placeholderParent = d.parentToReturnTo;
        //}
    }

    public void OnDrop(PointerEventData eventData)
    {

        DragHandler d = eventData.pointerDrag.GetComponent<DragHandler>();
        if (d != null)
        {
            CardLink cl = eventData.pointerDrag.GetComponent<CardLink>();
            if (cl != null)
            {
                Card card = cl.Card;
                Debug.Log(card.CardName + " was dropped on " + gameObject.name);

                if (card.CardType == CardType.SHIP && this.DropZoneType == DropZoneType.PLAYER_SHIPYARD)
                {
                    // try to host the ship on the shipyard
                    Shipyard shipyard = (Shipyard)this.GetComponent<CardLink>().Card;

                    GameClientController gameClient = FindObjectOfType<GameClientController>();
                    if (gameClient.TryHost((Ship)card, shipyard))
                    {
                        d.returnToParent = false;
                    }
                }                
            }
        }
    }
}
