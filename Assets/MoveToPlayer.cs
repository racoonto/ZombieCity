using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;

public class MoveToPlayer : MonoBehaviour
{
    private NavMeshAgent agent;
    public float maxSpeed = 20;
    public float duration = 3;

    private bool alreadyDone = false;

    private IEnumerator OnTriggerEnter(Collider other)
    {
        if (alreadyDone)
            yield break; //코루틴 정지

        if (other.CompareTag("Player"))
        {
            alreadyDone = true;
            agent = GetComponent<NavMeshAgent>();

            DOTween.To(() => agent.speed, (x) => agent.speed = x, maxSpeed, duration);

            while (true)
            {
                agent.destination = other.transform.position;
                yield return null;
            }
        }
    }
}