using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlayButtonsDisplayer : MonoBehaviour
{
    public static PlayButtonsDisplayer Inst
    {
        get { Debug.Assert(inst != null, "PlayButtonDisplayer is not set"); return inst; }
    }
    static PlayButtonsDisplayer inst;
    // Start is called before the first frame update
    Image w_img;
    Image a_img;
    Image s_img;
    Image d_img;
    static Color transparent = new Color(1, 1, 1, 0.05f);
    void Start()
    {
        w_img = transform.Find("W").GetComponent<Image>();
        a_img = transform.Find("A").GetComponent<Image>();
		s_img = transform.Find("S").GetComponent<Image>();
		d_img = transform.Find("D").GetComponent<Image>();
        Debug.Assert(inst == null, "PlayButtonsDisplayer is already set");
        inst = this;
	}
	private void OnDestroy()
	{
		inst = null;
	}
	bool ws = false;
    bool ad = false;
    bool q = false;
    public void UpdateWASD(bool ws, bool ad, bool q)
    {
        this.ws = ws;
        this.ad = ad;
        this.q = q;
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
                // ToastManager.Toast("No components controlled by W/S");
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.W)||Input.GetKeyDown(KeyCode.S))
            {
                AudioPlayer.Inst.MotorStart();
            }
        }
        if (q && Input.GetMouseButtonDown(1))
        {
            AudioPlayer.Inst.RocketStart();
        }
        if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.S) || Input.GetMouseButtonUp(1))
        {
            AudioPlayer.Inst.StopSoundEffect();
        }

        if (!ad)
        {
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D))
            {
                // ToastManager.Toast("No components controlled by A/D");
            }
        }
    }
}
