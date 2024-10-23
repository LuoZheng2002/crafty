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
    static AudioPlayer inst;
    public static AudioPlayer Inst
    {
        get { Debug.Assert(inst != null, "Audio player not set");return inst; }
    }
    void Start()
    {
        Debug.Assert(inst == null, "Audio Player already instantiated");
        inst = this;
        var audioSources = GetComponents<AudioSource>();
        musicSource = audioSources[0];
        soundEffectSource = audioSources[1];
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
    public void TransitionToIntro()
    {
		can_snore = false;
		if (musicSource.isPlaying)
		{
			musicSource.Stop();
		}
		musicSource.clip = menuClip;
		musicSource.loop = true;
		musicSource.Play();
	}
    public void TransitionToBuild()
    {
		if (musicSource.isPlaying)
		{
			musicSource.Stop();
		}
		musicSource.clip = buildClip;
		musicSource.loop = true;
		musicSource.Play();
	}
    public void TransitionToPlay()
    {
		if (musicSource.isPlaying)
		{
			musicSource.Stop();
		}
		musicSource.clip = playClip;
		musicSource.loop = true;
		musicSource.Play();
	}
    public void TransitionToOutro()
    {
		if (musicSource.isPlaying)
		{
			musicSource.Stop();
		}
		musicSource.clip = clearClip;
		musicSource.loop = false;
		musicSource.Play();
	}
}
