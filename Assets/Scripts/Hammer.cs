using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hammer : MonoBehaviour
{
    // Start is called before the first frame update
    public float period = 5.0f;
    public float delay = 0.0f;
    public float amplitude = 60.0f;
    void Start()
    {
        StartCoroutine(Swing());
	}
    IEnumerator Swing()
    {
        yield return new WaitForSeconds(delay);
        
        float time_elapsed = 0.0f;
		while (true)
        {
			Vector3 eular_angle = transform.eulerAngles;
			float pos = Mathf.Sin(time_elapsed / period * 2 * Mathf.PI) * amplitude;
            eular_angle.z = pos;
			transform.eulerAngles = eular_angle;
			yield return null;
			time_elapsed += Time.deltaTime;
		}
	}
    // Update is called once per frame
    void Update()
    {
        
    }
}
