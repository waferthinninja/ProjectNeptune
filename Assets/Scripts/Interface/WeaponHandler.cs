using UnityEngine;
using System.Collections;

public class WeaponHandler : MonoBehaviour {

    public Ship HostShip;
    public Weapon Weapon;
    public int WeaponIndex;
    public Card Target;
    public bool BelongsToPlayer; // perhaps shouldnt need this, convenient

    public void StartTargeting()
    {
        Debug.Log("StartTargeting");
        // register that we want to be told when something gets targeted
        GameClientController gameClient = FindObjectOfType<GameClientController>();
        gameClient.TargetingCallback = SetTarget;
        gameClient.EnterTargetingMode();
    }    

    public void SetTarget(Card card)
    {
        // otherwise set the target
        Debug.Log(string.Format("Trying to targeting {2} with {3}'s {0} {1}", Weapon.WeaponType, Weapon.Damage, card.CardName, HostShip.CardName));
        Target = card;

        GameClientController gameClient = FindObjectOfType<GameClientController>();
        if (!gameClient.TryTarget(HostShip, WeaponIndex, Target))
        {
            ClearTarget();
        }

        // unregister callback
        gameClient.TargetingCallback = null;
    }

    public void ClearTarget()
    {
        Target = null;
    }
}
