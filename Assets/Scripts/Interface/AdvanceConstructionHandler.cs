using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AdvanceConstructionHandler : MonoBehaviour {

    private bool _constructionComplete;

    public bool IsComplete()
    {
        return _constructionComplete;
    }

    public void AdvanceConstruction()
    {
        GameClientController gameClient = FindObjectOfType<GameClientController>();
        Transform shipGO = transform.parent.parent; // grandparent should be the card game object - better way to do this? search up 1 level at a time?
        Transform shipyardGO = shipGO.parent;
        Ship ship = (Ship)shipGO.GetComponent<CardLink>().Card;
        Shipyard shipyard = (Shipyard)shipyardGO.GetComponent<CardLink>().Card;

        if (_constructionComplete)
        {
            TryDeploy(gameClient, ship, shipyard);
        }
        else
        {
            TryAdvance(gameClient, ship, shipyard);
        }
    }

    private void TryAdvance(GameClientController gameClient, Ship ship, Shipyard shipyard)
    {
        Debug.Log(string.Format("Trying to advance construction of {0}({1}) on {2}({3})", ship.CardName, ship.CardId, shipyard.CardName, shipyard.CardId));
        if (gameClient.TryAdvanceConstruction(ship, shipyard))
        {
            Debug.Log("Advance successful, trying to update GUI");
            // successfully advanced - decrement the amount
            Text constructionRemaining = (Text)transform.parent.FindChild("ConstructionRemaining").GetComponent(typeof(Text));
            constructionRemaining.text = ship.ConstructionRemaining.ToString();

            if (ship.ConstructionRemaining == 0)
            {
                _constructionComplete = true;

                // change button text 
                Text button = (Text)transform.FindChild("AdvanceConstructionButtonText").GetComponent(typeof(Text));
                button.text = "Deploy";
            }
        }
    }

    private void TryDeploy(GameClientController gameClient, Ship ship, Shipyard shipyard)
    {
        Debug.Log(string.Format("Trying to deploy {0}({1}) from {2}({3})", ship.CardName, ship.CardId, shipyard.CardName, shipyard.CardId));
        if (gameClient.TryDeployShip(ship, shipyard))
        {
            Debug.Log("Deploy successful");            

            // remove this construction panel 
            Destroy(transform.parent.gameObject);
        }
    }
}
