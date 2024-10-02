using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseRaycast : MonoBehaviour
{
    // Start is called before the first frame update
    
	private RaycastHit[] hits = new RaycastHit[10]; // Adjust size as needed
													// Update is called once per frame

	 
	GameObject lastSelectedObject = null;
	void Update()
    {
		
	}
}
