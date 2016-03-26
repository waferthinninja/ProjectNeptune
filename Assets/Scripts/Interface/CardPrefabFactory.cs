using UnityEngine;
using System;
using UnityEngine.UI;

public class CardPrefabFactory : MonoBehaviour {

    public Transform ShipPrefab;
    public Transform ShipyardPrefab;
    public Transform WeaponPrefab;

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
        cardLink.Card = card;

        // populate common elements
        var imageElement = transform.Find("Image").GetComponent<Image>();
        var image = Resources.Load<Sprite>("Images/Cards/" + card.ImageName);
        imageElement.sprite = image;
        
        var name = (Text)transform.Find("Name").GetComponent(typeof(Text));
        name.text = card.CardName;

        if (card is PlayableCard)
        {
            var cardCost = (Text)transform.Find("Cost").GetComponent(typeof(Text));
            cardCost.text = ((PlayableCard)card).BaseCost.ToString();
        }

        return transform;
    }

    private Transform CreateShipPrefab(Card card)
    {
        Transform transform = Instantiate(ShipPrefab);

        Ship ship = (Ship)card;

        var name = (Text)transform.Find("Size").GetComponent(typeof(Text));
        name.text = ship.Size.ToString();

        var health = (Text)transform.Find("Health").GetComponent(typeof(Text));
        health.text = string.Format("{0}/{1}", ship.CurrentHealth, ship.MaxHealth);

        var weaponsPanel = transform.Find("WeaponsPanel");
        foreach(Weapon weapon in ship.Weapons)
        {
            Transform weaponTransform = Instantiate(WeaponPrefab);
            var weaponText = (Text)weaponTransform.Find("WeaponText").GetComponent(typeof(Text));
            weaponText.text = string.Format("{0} {1}", weapon.WeaponType, weapon.Damage);
            weaponTransform.SetParent(weaponsPanel);
        }

        return transform;
    }

    private Transform CreateShipyardPrefab(Card card)
    {
        Transform transform = Instantiate(ShipyardPrefab);
        Shipyard shipyard = (Shipyard)card;
        
        var maxSize = (Text)transform.Find("MaxSize").GetComponent(typeof(Text));
        maxSize.text =  shipyard.MaxSize.ToString();
        var efficiency = (Text)transform.Find("Efficiency").GetComponent(typeof(Text));
        efficiency.text = shipyard.Efficiency.ToString();

        return transform;
    }
}
