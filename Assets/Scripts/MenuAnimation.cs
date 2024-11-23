using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuAnimation : MonoBehaviour
{
	public bool is_transition3 = false;
	private void Start()
	{
		EventBus.Subscribe<MenuExplodeEvent>(OnMenuExplode);
		if (is_transition3)
		{
			StartCoroutine(AddImpulse());
			StartCoroutine(Countdown());
		}
	}
	public void Transition2Blackout()
	{
		BlackoutCanvas.Inst.BlackoutAsync(1.0f, 1.0f, 0.0f);
	}
	IEnumerator Countdown()
	{
		yield return new WaitForSeconds(4.0f);
		yield return BlackoutCanvas.Inst.Blackout(1.0f, 0.0f, 1.0f);
		SceneManager.LoadScene(4);
	}
	void OnMenuExplode(MenuExplodeEvent e)
	{
		Animator animator = GetComponent<Animator>();
		animator.SetTrigger("explode");
	}
	public void OnAnimationEnd()
    {
        StartCoroutine(AnimationEndHelper());
	}
    public void OnTransition1End()
    {
        StartCoroutine(Transition1Helper());
	}
    IEnumerator AnimationEndHelper()
    {
        yield return BlackoutCanvas.Inst.Blackout(1.0f, 0.0f, 1.0f);
        SceneManager.LoadScene(1);
    }
    public void Transition1Start()
    {
        BlackoutCanvas.Inst.BlackoutAsync(1.0f, 1.0f, 0.0f);
	}
	IEnumerator Transition1Helper()
	{
		yield return BlackoutCanvas.Inst.Blackout(1.0f, 0.0f, 1.0f);
		SceneManager.LoadScene(2);
	}
	public Rigidbody pig_rb;
	public float shake_intensity = 10.0f;
	public Transform camera_transform;
	public IEnumerator AddImpulse()
	{
		pig_rb.AddTorque(Random.insideUnitSphere * 1000.0f, ForceMode.Impulse);
		while (true)
		{
			// yield return new WaitForSeconds(1.0f);
			
			float x = (Mathf.PerlinNoise(Time.time * 10, 0) - 0.5f) * 2 * shake_intensity;
			float y = (Mathf.PerlinNoise(0, Time.time * 10) - 0.5f) * 2 * shake_intensity;
			float z = (Mathf.PerlinNoise(0, Time.time * 10) - 0.5f) * 2 * shake_intensity;
			camera_transform.localPosition = new Vector3(x, y, z);
			yield return null;
		}
	}
}
