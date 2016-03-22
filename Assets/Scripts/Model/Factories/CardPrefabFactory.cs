using UnityEngine;
using System;
using UnityEngine.UI;

public class CardPrefabFactory : MonoBehaviour {

    public Transform ShipPrefab;
    public Transform ShipyardPrefab;

    public Transform CreateCardPrefab(Card card)
    {
        Transform transform;
        switch (card.CardType)
        {
            case CardType.SHIP:
                transform = CreateShipPrefab(card);
                break;
            case CardType.SHIPYARD:
                transform = CreateShipyardPrefab(card);
                break;
            default:
                throw new Exception("Invalid card type");
        }

        // link the prefab to its "model" card
        var cardLink = transform.GetComponent<CardLink>();
        cardLink.card = card;

        return transform;
    }

    private Transform CreateShipPrefab(Card card)
    {
        Transform transform = Instantiate(ShipPrefab);
        Ship ship = (Ship)card;

        var name = (Text)transform.Find("Name").GetComponent(typeof(Text));
        name.text = ship.CardName;
        var cardCost = (Text)transform.Find("Cost").GetComponent(typeof(Text));
        cardCost.text = ship.BaseCost.ToString();

        return transform;
    }

    private Transform CreateShipyardPrefab(Card card)
    {
        Transform transform = Instantiate(ShipyardPrefab);
        Shipyard shipyard = (Shipyard)card;

        var shipyardName = (Text)transform.Find("Name").GetComponent(typeof(Text));
        shipyardName.text = shipyard.CardName;
        var maxSize = (Text)transform.Find("MaxSize").GetComponent(typeof(Text));
        maxSize.text = "Max size: " + shipyard.MaxSize.ToString();
        var efficiency = (Text)transform.Find("Efficiency").GetComponent(typeof(Text));
        efficiency.text = "Efficiency: " + shipyard.Efficiency.ToString();

        return transform;
    }
}
