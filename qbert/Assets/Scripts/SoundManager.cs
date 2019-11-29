using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{

  public AudioSource[] efxSources;
  public static SoundManager instance = null;

  public float lowPitchRange = .95f;
  public float highPitchRange = 1.05f;

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
    availableSource.clip = clip;
    availableSource.Play();
  }

  public void RandomizeSfx(params AudioClip[] clips)
  {
    AudioSource availableSource = selectAvailable();
    int randomIndex = Random.Range(0, clips.Length);
    float randomPitch = Random.Range(lowPitchRange, highPitchRange);

    availableSource.pitch = randomPitch;
    availableSource.clip = clips[randomIndex];
    availableSource.Play();
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
}
