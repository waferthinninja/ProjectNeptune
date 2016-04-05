using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class DropHandler : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public DropZoneType DropZoneType;
    public GameClientController GameClientController;

    public void OnStart()
    {
    }

    public void OnPointerEnter(PointerEventData eventData)
    {        
    }

    public void OnPointerExit(PointerEventData eventData)
    {        
    }

    public void OnDrop(PointerEventData eventData)
    {

        DragHandler d = eventData.pointerDrag.GetComponent<DragHandler>();
        if (d != null)
        {
            CardLink cl = eventData.pointerDrag.GetComponent<CardLink>();
            if (cl != null)
            {
                if (GameClientController == null)
                    GameClientController = FindObjectOfType<GameClientController>();

                Card card = cl.Card;
                Debug.Log(card.CardName + " was dropped on " + gameObject.name);

                if (card.CardType == CardType.SHIP && DropZoneType == DropZoneType.PLAYER_SHIPYARD)
                {
                    // try to host the ship on the shipyard
                    Shipyard shipyard = (Shipyard)GetComponent<CardLink>().Card;
                    
                    if (GameClientController.TryHost((Ship)card, shipyard))
                    {
                        d.returnToParent = false;
                    }
                }    
                
                if (card.CardType == CardType.SHIPYARD && DropZoneType == DropZoneType.PLAYER_CONSTRUCTION_AREA)
                {
                    if (GameClientController.TryPlayShipyard((Shipyard)card))
                    {
                        d.returnToParent = false;
                    }
                    
                }    
                
                if (card.CardType == CardType.OPERATION)
                {
                    if (GameClientController.TryPlayOperation((Operation)card))
                    {
                        d.returnToParent = false;
                    }
                }        
            }
        }
    }
}
