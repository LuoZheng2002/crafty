using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//public class UpdateWASDEvent
//{
//    public bool wa;
//    public bool sd;
//    public UpdateWASDEvent(bool wa, bool sd)
//    {
//        this.wa = wa;
//        this.sd = sd;
//    }
//}

public class PlayButtonsDisplayer : MonoBehaviour
{
    // Start is called before the first frame update
    Image w_img;
    Image a_img;
    Image s_img;
    Image d_img;
    static Color transparent = new Color(1, 1, 1, 0.2f);
    void Start()
    {
        w_img = transform.Find("W").GetComponent<Image>();
        a_img = transform.Find("A").GetComponent<Image>();
		s_img = transform.Find("S").GetComponent<Image>();
		d_img = transform.Find("D").GetComponent<Image>();
	}
    bool ws = false;
    bool ad = false;
    public void UpdateWASD(bool ws, bool ad)
    {
        this.ws = ws;
        this.ad = ad;
        // ToastManager.Toast($"WASD: {e.wa}, {e.sd}");
        if (ws)
        {
            w_img.color = Color.white;
            a_img.color = Color.white;
        }
        else
        {
            w_img.color = transparent;
            a_img.color = transparent;
        }
		if (ad)
		{
			s_img.color = Color.white;
			d_img.color = Color.white;
		}
		else
		{
			s_img.color = transparent;
			d_img.color = transparent;
		}
	}

    // Update is called once per frame
    void Update()
    {
        if (!ws)
        {
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S))
            {
                ToastManager.Toast("No components controlled by W/S");
            }
        }
        if (!ad)
        {
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D))
            {
                ToastManager.Toast("No components controlled by A/D");
            }
        }
    }
}
