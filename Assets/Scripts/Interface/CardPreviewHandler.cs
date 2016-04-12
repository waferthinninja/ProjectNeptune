using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardPreviewHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public CardPrefabFactory CardPrefabFactory;
    private Transform CardPreviewPanel;

    void Start()
    {
        CardPreviewPanel = GameObject.Find("CardPreview").transform;
        CardPrefabFactory = GameObject.Find("CardPrefabFactory").transform.GetComponent<CardPrefabFactory>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // clear the preview panel
        foreach (Transform child in CardPreviewPanel)
        {
            Destroy(child.gameObject);
        }

        // add a scaled version of the card to the preview panel
        Transform clone = CardPrefabFactory.CreateCardPrefab(CardFactory.CreateCard(GetComponent<CardLink>().Card.CardCodename), false);
        clone.localScale = new Vector3(2, 2); // TODO - remove hard coding, work it out from sizes?
        clone.SetParent(CardPreviewPanel);
        clone.localPosition = new Vector3(0, 0, 0);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // clear the preview panel
        foreach (Transform child in CardPreviewPanel)
        {
            Destroy(child.gameObject);
        }

    }

    
	

}
