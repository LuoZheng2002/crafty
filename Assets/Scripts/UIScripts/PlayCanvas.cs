using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayCanvas : MonoBehaviour
{
    static PlayCanvas inst;
    public GameObject rocket;
	public GameObject umbrella;

    public float RocketFuel { get; set; } = 1;
    public float rocket_consumption_speed = 0.2f;
	public static PlayCanvas Inst
    {
        get { Debug.Assert(inst != null, "Play Canvas not set"); return inst; }
    }
	private void Start()
	{
        Debug.Assert(inst == null, "Play canvas already set");
        inst = this;
        Util.Delay(this, () =>
        {
            gameObject.SetActive(false);
        });
	}
	private void OnEnable()
	{
		RocketFuel = 1;
		SetRocketFill(RocketFuel);
	}
	public void ShowRocket()
    {
        rocket.SetActive(true);
	}
    public void HideRocket()
    {
        rocket.SetActive(false);
	}
    public Image rocket_fuel_image;
    public void SetRocketFill(float amount)
    {
        Debug.Log($"Rocket fill amount: {amount}");
		rocket_fuel_image.fillAmount = amount;
	}
    public void ShowUmbrella()
    {
        umbrella.SetActive(true);
	}
    public void HideUmbrella()
    {
        umbrella.SetActive(false);
	}
    public void Show()
    {
        gameObject.SetActive(true);
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }
	private void Update()
	{
		if (Input.GetMouseButton(1))
        {
			RocketFuel -= rocket_consumption_speed * Time.deltaTime;
			SetRocketFill(RocketFuel);

		}
	}

	public void OnStoryClicked()
	{

	}
    public GameObject story;
    public void ShowStory()
    {
        story.SetActive(true);
    }
	public void HideStory()
	{
		story.SetActive(false);
	}
}
