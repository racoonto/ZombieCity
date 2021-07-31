using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Zombie : MonoBehaviour
{
    private Transform target;
    private NavMeshAgent agent;
    private Animator animator;
    public int hp = 100;

    private IEnumerator Start()
    {
        agent = GetComponent<NavMeshAgent>();
        target = FindObjectOfType<Player>().transform;
        animator = GetComponentInChildren<Animator>();
        //target = Player.Instance.transform;

        while (hp > 0)
        {
            if (target)
                agent.destination = target.position;
            yield return new WaitForSeconds(Random.Range(0, 2f));
        }
    }

    internal void TakeHit(int damage)
    {
        hp -= damage;
        animator.Play("TakeHit");
        // 피격 이펙트 생성(피,..)
        if (hp <= 0)
        {
            GetComponent<Collider>().enabled = false;
            Invoke(nameof(Die), 1);//1초 뒤에 die함수 실행
        }
    }

    private void Die()
    {
        animator.Play("Die");
        Destroy(gameObject, 1); //1초뒤에 파괴
    }
}