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
    private float originalSpeed;

    private IEnumerator Start()
    {
        agent = GetComponent<NavMeshAgent>();
        target = FindObjectOfType<Player>().transform;
        animator = GetComponentInChildren<Animator>();
        originalSpeed = agent.speed;
        //target = Player.Instance.transform;

        while (hp > 0)
        {
            if (target)
                agent.destination = target.position;
            yield return new WaitForSeconds(Random.Range(0, 2f));
        }
    }

    internal void TakeHit(int damage, Vector3 toMoveDirection)
    {
        hp -= damage;
        //animator.Play("TakeHit");
        animator.Play(Random.Range(0, 2) == 0 ? "TakeHit1" : "TakeHit2", 0, 0);
        // 피격 이펙트 생성(피,..)

        //뒤로 밀려나자
        PushBackMove(toMoveDirection);

        agent.speed = 0;
        CancelInvoke(nameof(SetTakeHitSpeedCo));
        Invoke(nameof(SetTakeHitSpeedCo), TakeHitStopSpeedTime);
        //StartCoroutine(SetTakeHitSpeedCo());

        if (hp <= 0)
        {
            GetComponent<Collider>().enabled = false;
            Invoke(nameof(Die), 1);//1초 뒤에 die함수 실행
        }
    }

    public float moveBackDistance = 0.1f;
    public float moveBackNoise = 0.1f;

    private void PushBackMove(Vector3 toMoveDirection)
    {
        //랜덤하게 뒤로 밀려남
        toMoveDirection.x += Random.Range(-moveBackNoise, moveBackNoise);
        toMoveDirection.z += Random.Range(-moveBackNoise, moveBackNoise);
        toMoveDirection.y = 0;
        toMoveDirection.Normalize();
        transform.Translate(toMoveDirection * moveBackDistance, Space.World);
    }

    public float TakeHitStopSpeedTime = 0.1f;

    private void SetTakeHitSpeedCo()
    {
        //agent.speed = 0;
        //yield return new WaitForSeconds(TakeHitStopSpeedTime);
        agent.speed = originalSpeed;
    }

    private void Die()
    {
        animator.Play("Die");
        Destroy(gameObject, 1); //1초뒤에 파괴
    }
}