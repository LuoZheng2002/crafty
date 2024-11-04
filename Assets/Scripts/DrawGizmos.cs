using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawGizmos : MonoBehaviour
{
	public Color gizmoColor = Color.yellow;
	public Color gizmoColor2 = Color.blue;
	public float gizmoSize = 0.5f;
	public Mesh mesh;

	void OnDrawGizmos()
	{
		Gizmos.color = gizmoColor;
		// Gizmos.DrawSphere(transform.position, gizmoSize);
		Gizmos.DrawWireCube(transform.position, Vector3.one);
		Gizmos.color = gizmoColor2;
		Gizmos.DrawMesh(mesh, transform.position, transform.rotation * Quaternion.Euler(new Vector3(90, 0, 0)));
	}
}
