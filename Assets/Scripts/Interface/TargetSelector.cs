using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class TargetSelector : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));

        if (Input.GetMouseButtonUp(0))
        {
            Debug.Log("Left mouse button clicked");
            GameClientController gameClient = FindObjectOfType<GameClientController>();

            PointerEventData pointerData = new PointerEventData(EventSystem.current)
            {
                pointerId = -1,
            };

            pointerData.position = Input.mousePosition;

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);
                        
            foreach (RaycastResult result in results)
            {
                var cardLink = result.gameObject.GetComponent<CardLink>();
                if (cardLink != null)
                {
                    gameClient.TargetingCallback(cardLink.Card);
                    break;
                }
            }
            Destroy(this.gameObject);
        }
        if (Input.GetMouseButtonUp(1))
        {
            Debug.Log("Right mouse button clicked");
            Destroy(this.gameObject);
        }
    }
}
