using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using System;

public class MoveToPlayer : MonoBehaviour
{
    private NavMeshAgent agent;
    public float maxSpeed = 20;
    public float duration = 3;

    private bool alreadyDone = false;
    private TweenerCore<float, float, FloatOptions> tweenResult;

    private void OnTriggerEnter(Collider other)
    {
        if (alreadyDone)
            return; // 코루틴 정지

        if (other.CompareTag("Player"))
        {
            alreadyDone = true;
            agent = GetComponent<NavMeshAgent>();
            tweenResult = DOTween.To(() => agent.speed, (x) => agent.speed = x, maxSpeed, duration);

            setDestinationCoHandle = StartCoroutine(SetDestinationCo(other.transform));
        }
    }

    public void StopCoroutine()
    {
        StopCoroutine(setDestinationCoHandle);
    }

    private Coroutine setDestinationCoHandle;

    private IEnumerator SetDestinationCo(Transform tr)
    {
        while (true)
        {
            agent.destination = tr.position;
            yield return null;
        }
    }

    private void OnDestroy()
    {
        tweenResult.Kill();
    }
}