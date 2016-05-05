using UnityEngine;
using System.Collections;

public class WeaponHandler : MonoBehaviour {

    public Ship HostShip;
    public Weapon Weapon;
    public int WeaponIndex;
    private Card Target;
    public bool BelongsToPlayer; // perhaps shouldnt need this?
    private Transform TargetTransform;
    public Transform Arrow;
    public Transform Arrowhead;

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
        Debug.Log(string.Format("Trying to target {2} with {3}'s {0} {1}", Weapon.WeaponType, Weapon.Damage, card.CardName, HostShip.CardName));
        Target = card;

        GameClientController gameClient = FindObjectOfType<GameClientController>();
        if (!gameClient.TryTarget(HostShip, WeaponIndex, Target))
        {
            ClearTarget();
        }

        // unregister callback
        gameClient.TargetingCallback = null;
    }

    public void SetOpponentTarget(Card card)
    {
        // this is for setting the target when it is an opponents weapon
        // something smells off here, some of this code should be in GameViewController, but for now...
        Target = card;
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
            var emission = Arrowhead.GetComponent<ParticleSystem>().emission;
            emission.enabled = false;
        }
        else
        {            
            // TODO - this code feels like it should be in GameViewController?
            // scale and position arrow so it links the weapon to its target
            GameViewController gameView = FindObjectOfType<GameViewController>();
            TargetTransform = gameView._transformById[Target.CardId];

            if (TargetTransform == null)
            {
                Debug.Log(string.Format("{0} {1} lost target", Weapon.WeaponType, Weapon.Damage) );
                return;
            }
            var vector = TargetTransform.position - this.transform.position;
            Arrow.localPosition = vector / 2;
            Arrow.localScale = new Vector3(3, vector.magnitude, 3);
            Arrow.localRotation = Quaternion.FromToRotation(Vector3.up, vector);
            Arrowhead.position = new Vector3(TargetTransform.position.x, TargetTransform.position.y, -5);
            var emission = Arrowhead.GetComponent<ParticleSystem>().emission;
            emission.enabled = true;
        }
        
        
    }
}
