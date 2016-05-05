using UnityEngine;
using System.Collections;

// TODO - most of this is copied from WeaponHandler, and is stuff that perhaps should live in GameViewController anyway - look at refactoring
public class MissileHandler : MonoBehaviour {

    private Card Target;
    private Transform TargetTransform;
    public Transform Arrow;
    public Transform Arrowhead;

    public void SetTarget(Card card)
    {
        Target = card;
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
