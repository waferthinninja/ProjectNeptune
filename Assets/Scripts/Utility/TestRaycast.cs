using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Text;

public class TestRaycast : MonoBehaviour {

		
	// Update is called once per frame
	void Update () {
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            pointerId = -1,
        };

        pointerData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        StringBuilder sb = new StringBuilder();
        foreach (RaycastResult result in results)
        {
            sb.Append(result.gameObject.name);
            sb.Append("->");
        }
        Debug.Log(sb.ToString());
    }
}
