using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    public AudioClip buildClip;
    public AudioClip playClip;
    public AudioClip clearClip;
    public AudioClip menuClip;
    public AudioClip snore;
    public AudioClip scream;
    AudioSource musicSource;
    AudioSource soundEffectSource;
    public float min_snore_interval = 10.0f;
    public float max_snore_interval = 30.0f;
    bool can_snore = false;
    // Start is called before the first frame update
    void Start()
    {
        var audioSources = GetComponents<AudioSource>();
        musicSource = audioSources[0];
        soundEffectSource = audioSources[1];
        EventBus.Subscribe<GameStateChangedEvent>(OnGameStateChanged);
        EventBus.Subscribe<ScreamEvent>(Scream);
        StartCoroutine(Snore());
    }
    void Scream(ScreamEvent e)
    {
        if (soundEffectSource.isPlaying)
        {
            soundEffectSource.Stop();
        }
        soundEffectSource.clip = scream;
        soundEffectSource.loop = false;
        soundEffectSource.Play();
    }
    IEnumerator Snore()
    {
        while(true)
        {
            yield return new WaitForSeconds(Random.Range(min_snore_interval, max_snore_interval));
            if (!can_snore)
            {
                continue;
            }
            if (!soundEffectSource.isPlaying)
            {
                soundEffectSource.clip = snore;
                soundEffectSource.loop = false;
                soundEffectSource.Play();
            }
        }
    }
    void OnGameStateChanged(GameStateChangedEvent e)
    {
        if (e.state == Util.GameStateType.Play)
        {
            can_snore = true;
        }
        else
        {
            can_snore = false;
        }
        if (e.state == Util.GameStateType.Intro)
		{
			if (musicSource.isPlaying)
			{
				musicSource.Stop();
			}
			musicSource.clip = menuClip;
			musicSource.loop = true;
			musicSource.Play();
		}
        else if (e.state == Util.GameStateType.Build)
        {
            if (musicSource.isPlaying)
            {
                musicSource.Stop();
            }
            musicSource.clip = buildClip;
            musicSource.loop = true;
            musicSource.Play();
        }
        else if (e.state == Util.GameStateType.Play)
        {
			if (musicSource.isPlaying)
			{
				musicSource.Stop();
			}
			musicSource.clip = playClip;
			musicSource.loop = true;
			musicSource.Play();
		}
        else if (e.state == Util.GameStateType.Outro)
        {
			if (musicSource.isPlaying)
			{
				musicSource.Stop();
			}
			musicSource.clip = clearClip;
			musicSource.loop = false;
			musicSource.Play();
		}
        else
        {
            if (musicSource.isPlaying)
            {
                musicSource.Stop();
            }
        }
    }
}
