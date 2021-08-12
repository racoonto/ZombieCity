﻿using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Attach this to any object that is either a ParticleSystem, or has one or more ParticleSystems
/// as child objects. After all ParticleSystems (including the children and this object's) have finished emitting
/// the object will self-destruct, or alternatively if an audio source is playing will self-destruct after both it
/// and the particle systems have finished playing
/// </summary>
public class ParticleMaster : MonoBehaviour
{
    [SerializeField]
    private bool waitForAudioSource = true;

    [SerializeField]
    private bool emitOnAwake = true;

    private List<ParticleSystem> particles;

    private void Awake()
    {
        particles = new List<ParticleSystem>();

        foreach (Transform child in transform)
        {
            if (child.GetComponent<ParticleSystem>() != null)
            {
                particles.Add(child.GetComponent<ParticleSystem>());
            }
        }

        if (gameObject.GetComponent<ParticleSystem>() != null)
        {
            particles.Add(gameObject.GetComponent<ParticleSystem>());
        }

        if (!emitOnAwake)
        {
            foreach (var particle in particles)
            {
                var emission = particle.emission;
                emission.enabled = false;
                //particle.enableEmission = false;
            }
        }
    }

    public void BeginEmission()
    {
        foreach (var particle in particles)
        {
            //particle.enableEmission = true;
            var emission = particle.emission;
            emission.enabled = true;
        }
    }

    public void Emit(bool emit)
    {
        foreach (var particle in particles)
        {
            var main = particle.main;
            main.loop = emit;
            //particle.loop = emit;
        }
    }

    private void Update()
    {
        if (waitForAudioSource && GetComponent<AudioSource>() && GetComponent<AudioSource>().isPlaying)
            return;

        foreach (var particle in particles)
        {
            if (particle.IsAlive())
            {
                return;
            }
        }

        Destroy(gameObject);
    }
}