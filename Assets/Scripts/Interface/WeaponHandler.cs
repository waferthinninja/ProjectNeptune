﻿using UnityEngine;
using System.Collections;

public class WeaponHandler : MonoBehaviour {

    public Ship HostShip;
    public Weapon Weapon;
    public int WeaponIndex;
    private Card Target;
    public bool BelongsToPlayer; // perhaps shouldnt need this?
    public Transform TargetTransform;
    public Transform Arrow; 

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
        Debug.Log(string.Format("Clearing target for {0}'s {1} {2}", HostShip.CardName, Weapon.WeaponType, Weapon.Damage));
        Target = null;
    }

    void Update()
    {
        if (Target == null)
        {
            Arrow.localScale = new Vector3();
        }
        else
        {            
            // scale and position arrow so it links the weapon to its target
            GameViewController gameView = FindObjectOfType<GameViewController>();
            TargetTransform = gameView._transformById[Target.CardId];
            var vector = TargetTransform.position - this.transform.position;
            Arrow.localPosition = vector / 2;
            Arrow.localScale = new Vector3(3, vector.magnitude, 3);
            Arrow.localRotation = Quaternion.FromToRotation(Vector3.up, vector);
        }
        
        
    }
}
