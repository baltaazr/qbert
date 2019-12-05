using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{

  public AudioSource[] efxSources;
  public AudioSource musicSource;
  public static SoundManager instance = null;

  public float lowPitchRange = 0.95f;
  public float highPitchRange = 1.05f;

  public float volume = 1f;

  void Awake()
  {
    if (instance == null)
      instance = this;
    else if (instance != this)
      Destroy(gameObject);

    DontDestroyOnLoad(gameObject);
  }

  public void PlaySingle(AudioClip clip)
  {
    AudioSource availableSource = selectAvailable();
    availableSource.volume = volume;
    availableSource.clip = clip;
    availableSource.Play();
  }

  public void RandomizeSfx(params AudioClip[] clips)
  {
    AudioSource efxSource = selectAvailable();

    int randomIdx = Random.Range(0, clips.Length);
    float randomPitch = Random.Range(lowPitchRange, highPitchRange);

    efxSource.pitch = randomPitch;
    efxSource.clip = clips[randomIdx];
    efxSource.Play();
  }

  AudioSource selectAvailable()
  {
    int i = 0;
    while (efxSources[i].isPlaying && i < efxSources.Length - 1)
    {
      i += 1;
    }
    return efxSources[i];
  }

  public void SetVolume(Slider slider)
  {
    volume = slider.value;
    musicSource.volume = volume;
  }
}
