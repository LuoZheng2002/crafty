using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    public AudioClip buildClip;
    public AudioClip playClip;
    AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        EventBus.Subscribe<GameStateChangedEvent>(OnGameStateChanged);
    }
    void OnGameStateChanged(GameStateChangedEvent e)
    {
        if (e.state == Util.GameStateType.Build)
        {
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
            audioSource.clip = buildClip;
            audioSource.loop = true;
            audioSource.Play();
        }
        else if (e.state == Util.GameStateType.Play)
        {
			if (audioSource.isPlaying)
			{
				audioSource.Stop();
			}
			audioSource.clip = playClip;
			audioSource.loop = true;
			audioSource.Play();
		}
        else
        {
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
    }
}
