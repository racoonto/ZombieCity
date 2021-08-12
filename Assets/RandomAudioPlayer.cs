﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RandomAudioPlayer : MonoBehaviour
{
    [System.Serializable]
    public class AudioInfo
    {
        public AudioClip clip;
        public float ratio;
        public float minVolume = 1f;
        public float maxVolume = 1f;
    }

    public List<AudioInfo> audios;

    public float maxRandomTime = 30;
    public float gMinVolume = 0.5f;
    public float gMaxVolume = 1f;

    private IEnumerator Start()
    {
        AudioSource audioSource = GetComponent<AudioSource>();

        while (true)
        {
            yield return new WaitForSeconds(Random.Range(0, maxRandomTime));
            AudioInfo info = audios.OrderBy(x => Random.Range(0, x.ratio)).Last();
            audioSource.volume = Random.Range(info.minVolume * gMinVolume, info.maxVolume * gMaxVolume);
            audioSource.clip = info.clip;
            audioSource.Play();

            //audioSource.clip = audios.OrderBy(x => Random.Range(0, x.ratio)).Last().clip;
            //audioSource.Play();

            yield return new WaitForSeconds(audioSource.clip.length);
        }
    }
}