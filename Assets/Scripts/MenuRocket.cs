using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuRocket : MonoBehaviour
{
    Rigidbody rb;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        StartCoroutine(StartHelper());
	}
    IEnumerator StartHelper()
    {
        yield return new WaitForSeconds(2.0f);
        BoosterPreview boosterPreview = GetComponent<BoosterPreview>();
        Debug.Assert(boosterPreview != null);
        boosterPreview.PlayParticle();
        while(true)
        {
            rb.AddForce(Vector3.up * 200.0f, ForceMode.Force);
            yield return null;
		}
	}
}
