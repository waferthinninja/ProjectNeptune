using UnityEngine;
using System;
using UnityEngine.UI;

public class CardPrefabFactory : MonoBehaviour {

    public Transform UnknownCardPrefab;
    public Transform ShipPrefab;
    public Transform ShipyardPrefab;
    public Transform HomeworldPrefab;
    public Transform OperationPrefab;

    public Transform WeaponPrefab;

    public Transform CreateCardPrefab(Card card, bool belongsToPlayer)
    {
        //Debug.Log("Creating prefab of " + card.CardCodename);
        Transform transform;
        switch (card.CardType)
        {
            case CardType.UNKNOWN:
                transform = CreateUnknownPrefab(card);
                return transform; // dont need anything more for this "fake" card                
            case CardType.SHIP:
                transform = CreateShipPrefab(card, belongsToPlayer);
                break;
            case CardType.SHIPYARD:
                transform = CreateShipyardPrefab(card);
                break;
            case CardType.HOMEWORLD:
                transform = CreateHomeworldPrefab(card);
                break;
            case CardType.OPERATION:
                transform = CreateOperationPrefab(card);
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

        var cardText = (Text)transform.Find("CardText").GetComponent(typeof(Text));
        cardText.text = card.CardText;

        if (card is PlayableCard)
        {
            var cardCost = (Text)transform.Find("Cost").GetComponent(typeof(Text));
            cardCost.text = ((PlayableCard)card).BaseCost.ToString();
        }

        return transform;
    }

    private Transform CreateUnknownPrefab(Card card)
    {
        Transform transform = Instantiate(UnknownCardPrefab);

        return transform;
    }

    private Transform CreateOperationPrefab(Card card)
    {
        Transform transform = Instantiate(OperationPrefab);

        return transform;
    }

    private Transform CreateHomeworldPrefab(Card card)
    {
        Transform transform = Instantiate(HomeworldPrefab);

        Homeworld homeworld = (Homeworld)card;

        var health = (Text)transform.Find("Health").GetComponent(typeof(Text));
        health.text = string.Format("{0}/{1}", homeworld.CurrentHealth, homeworld.MaxHealth);

        return transform;
    }

    private Transform CreateShipPrefab(Card card, bool belongsToPlayer)
    {
        //Debug.Log("Instantiating " + card.CardName + (belongsToPlayer ? " belonging to Player" : " belonging to Opponent"));
        Transform transform = Instantiate(ShipPrefab);

        Ship ship = (Ship)card;

        var name = (Text)transform.Find("Size").GetComponent(typeof(Text));
        name.text = ship.Size.ToString();

        var health = (Text)transform.Find("Health").GetComponent(typeof(Text));
        health.text = string.Format("{0}/{1}", ship.CurrentHealth, ship.MaxHealth);

        // add weapons
        var weaponsPanel = transform.Find("WeaponsPanel");
        for (int i = 0; i < ship.Weapons.Count; i++)
        {
            Weapon weapon = ship.Weapons[i];
            Transform weaponTransform = Instantiate(WeaponPrefab);
            var weaponText = (Text)weaponTransform.Find("WeaponText").GetComponent(typeof(Text));
            weaponText.text = string.Format("{0} {1}", weapon.WeaponType, weapon.Damage);
            weaponTransform.SetParent(weaponsPanel);

            // link to weapon
            var weaponHandler = weaponTransform.GetComponent<WeaponHandler>();
            weaponHandler.HostShip = ship;
            weaponHandler.Weapon = weapon;
            weaponHandler.WeaponIndex = i;
            weaponHandler.BelongsToPlayer = belongsToPlayer;
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
        
        var health = (Text)transform.Find("Health").GetComponent(typeof(Text));
        health.text = string.Format("{0}/{1}", shipyard.CurrentHealth, shipyard.MaxHealth);

        return transform;
    }
}
