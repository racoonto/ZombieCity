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

    private IEnumerator OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
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