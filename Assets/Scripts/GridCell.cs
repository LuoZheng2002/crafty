using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCell : MonoBehaviour
{
    public Material idleMaterial;
    public Material activeMaterial;
    public Material selectedMaterial;
    public Vec3 Pos { get; set; }
    Renderer rr;
    Renderer RR { get { if (rr == null) rr = GetComponent<Renderer>(); return rr; } }
    bool active;
    public bool Active {  get { return active; } }  
    bool selected = false;

	public void SetActive(bool is_active)
    {
        active = is_active;
        if (selected)
        {
            RR.material = selectedMaterial;
        }
        else if (active)
        {
			RR.material = activeMaterial;
		}
        else
        {
			RR.material = idleMaterial;
		}
    }
	public void Select()
    {
        selected = true;
        RR.material = selectedMaterial;
    }
    public void Deselect()
    {
        selected = false;
        if (active)
        {
            RR.material = activeMaterial;
        }
        else
        {
            RR.material = idleMaterial;
        }
    }
}
