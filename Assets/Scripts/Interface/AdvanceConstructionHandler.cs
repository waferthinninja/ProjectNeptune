using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AdvanceConstructionHandler : MonoBehaviour {

    private bool ConstructionComplete;

    public void AdvanceConstruction()
    {
        GameClientController gameClient = FindObjectOfType<GameClientController>();
        Transform shipGO = transform.parent.parent; // grandparent should be the card game object - better way to do this? search up 1 level at a time?
        Transform shipyardGO = shipGO.parent;
        Ship ship = (Ship)shipGO.GetComponent<CardLink>().card;
        Shipyard shipyard = (Shipyard)shipyardGO.GetComponent<CardLink>().card;

        if (ConstructionComplete)
        {
            TryDeploy(gameClient, ship, shipGO, shipyard);
        }
        else
        {
            TryAdvance(gameClient, ship);
        }
    }

    private void TryAdvance(GameClientController gameClient, Ship ship)
    {
        Debug.Log("Trying to advance construction of " + ship.CardName);
        if (gameClient.TryAdvanceConstruction(ship))
        {
            Debug.Log("Advance successful, trying to update GUI");
            // successfully advanced - decrement the amount
            Text constructionRemaining = (Text)transform.parent.FindChild("ConstructionRemaining").GetComponent(typeof(Text));
            constructionRemaining.text = ship.ConstructionRemaining.ToString();

            if (ship.ConstructionRemaining == 0)
            {
                ConstructionComplete = true;

                // change button text 
                Text button = (Text)transform.FindChild("AdvanceConstructionButtonText").GetComponent(typeof(Text));
                button.text = "Deploy";
            }
        }
    }

    private void TryDeploy(GameClientController gameClient, Ship ship, Transform shipTransform, Shipyard shipyard)
    {
        Debug.Log("Trying to deploy " + ship.CardName);
        if (gameClient.TryDeploy(ship, shipyard))
        {
            Debug.Log("Deploy successful");

            // move to player ship area
            var playerShipArea = GameObject.Find("PlayerShipArea").transform;
            shipTransform.SetParent(playerShipArea);

            // remove this construction panel 
            Destroy(gameObject);
        }
    }
}
