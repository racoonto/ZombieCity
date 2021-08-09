using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class Zombie : Actor
{
    public Transform target;
    private NavMeshAgent agent;
    private float originalSpeed;

    private IEnumerator Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        target = FindObjectOfType<Player>().transform;  //
        originalSpeed = agent.speed;
        attackCollider = transform.Find("AttackRange").GetComponent<SphereCollider>();

        CurrentFsm = ChaseFSM;

        while (true) // 상태를 무한히 반복해서 실행하는 부분.
        {
            var previousFSM = CurrentFsm;

            //print($"{CurrentFsm.Method} 시작됨");

            fsmHandle = StartCoroutine(CurrentFsm());

            // FSM 안에서 에러 발생시 무한 루프 도는 것을 방지 하기 위해서 추가함.
            if (fsmHandle == null && previousFSM == CurrentFsm)
                yield return null;

            while (fsmHandle != null)
                yield return null;
        }
    }

    private Coroutine fsmHandle;

    protected Func<IEnumerator> CurrentFsm
    {
        get { return m_currentFsm; }
        set
        {
            if (fsmHandle != null)
                StopCoroutine(fsmHandle);

            m_currentFsm = value;
            fsmHandle = null;
        }
    }

    private Func<IEnumerator> m_currentFsm;

    private IEnumerator ChaseFSM()
    {
        if (target)
            agent.destination = target.position;
        yield return new WaitForSeconds(Random.Range(0.5f, 2f));

        SetFsm_SelectAttackTargetOrAttackOrChase();
    }

    private void SetFsm_SelectAttackTargetOrAttackOrChase()
    {
        if (IsAttackableTarget())
        {
            // 타겟이 공격 범위 안에 들어왔는가?
            if (TargetIsInAttackArea()) // 들어왔다면
                CurrentFsm = AttackFSM;
            else
                CurrentFsm = ChaseFSM;
        }
        else
        {
            print("배회하기 구현해야함");
            // 공격 가능한 타겟 찾기.
            // 공격 가느한 타겟이 없다면
            //-> 배회하기 혹은 제자리 가만 있기.
        }
    }

    private bool IsAttackableTarget()
    {
        if (target.GetComponent<Player>().stateType
            == Player.StateType.Die)
            return false;

        return true;
    }

    public float attackDistance = 3;

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, attackDistance);
    }

    private bool TargetIsInAttackArea()
    {
        float distance = Vector3.Distance(transform.position, target.position);
        return distance < attackDistance;
    }

    public float attackTime = 0.4f;
    public float attackAnimationTime = 0.8f;
    public SphereCollider attackCollider;
    public LayerMask enemyLayer;
    public int power = 20;

    private IEnumerator AttackFSM()
    {
        // 타겟 바라보기
        var lookAtPosition = target.position;
        lookAtPosition.y = transform.position.y;
        transform.LookAt(lookAtPosition);

        //공격 애니메이션 하기.
        animator.SetTrigger("Attack");

        // 이동 스피드 0으로 하기.
        agent.speed = 0;

        // 공격타이밍까지 대기(특정 시간 지나면)
        yield return new WaitForSeconds(attackTime);

        // 충돌메시 사용해서 충돌 감지하기.
        Collider[] enemyColliders = Physics.OverlapSphere(
            attackCollider.transform.position
            , attackCollider.radius, enemyLayer);
        foreach (var item in enemyColliders)
        {
            item.GetComponent<Player>().TakeHit(power);
        }

        // 공격 애니메이션 끝날때까지 대기
        yield return new WaitForSeconds(attackAnimationTime - attackTime);

        // 이동스피드 복구
        SetOriginalSpeed();

        // FSM지정.
        SetFsm_SelectAttackTargetOrAttackOrChase();
    }

    internal void TakeHit(int damage, Vector3 toMoveDirection, float pushBackDistance = 0.1f)
    {
        base.TakeHit(damage);

        if (hp <= 0)
        {
            GetComponent<Collider>().enabled = false;
            animator.SetBool("Die", true);
        }

        // 뒤로 밀려나게하자.
        PushBackMove(toMoveDirection, pushBackDistance);

        CurrentFsm = TakeHitFSM;
    }

    private IEnumerator TakeHitFSM()
    {
        animator.Play(Random.Range(0, 2) == 0 ? "TakeHit1" : "TakeHit2", 0, 0);
        // 피격 이펙트 생성(피,..)

        // 이동 스피드를 잠시 0으로 만들자.
        agent.speed = 0;

        yield return new WaitForSeconds(TakeHitStopSpeedTime); // 피격 모션 끝나기까지 기다림.

        if (hp <= 0)
        {
            Die();
            yield break;
        }
        else
        {
            SetOriginalSpeed();
        }

        CurrentFsm = ChaseFSM;
    }

    public int rewardScore = 100;
    public float onDieDestroyDelay = 0.1f;
    public Material dieMaterial;
    public float dieMaterialDuration = 1.5f;

    private void Die()
    {
        StageManager.Instance.AddScore(rewardScore);

        //메터리얼 교체
        var renderers = GetComponentsInChildren<Renderer>(true);
        foreach (var item in renderers)
        {
            item.sharedMaterial = dieMaterial;
        }

        dieMaterial.SetFloat("_Progress", 1);
        DOTween.To(() => 1f, (x) => dieMaterial.SetFloat("_Progress", x), 0.14f, dieMaterialDuration).SetDelay(onDieDestroyDelay).OnComplete(() => Destroy(gameObject));

        //dieMaterial.SetFloat("_Progress", 0.5f);

        //교체되는 동안 보여주고 파괴

        //animator.Play("Die");
        //Destroy(gameObject, onDieDestroyDelay);
    }

    public float moveBackDistance = 0.1f;
    public float moveBackNoise = 0.1f;
    public float moveBackDuration = 0.5f;
    public Ease moveBackEase = Ease.OutQuart;

    private void PushBackMove(Vector3 toMoveDirection, float _moveBackDistance)
    {
        toMoveDirection.x += Random.Range(-moveBackNoise, moveBackNoise);
        toMoveDirection.z += Random.Range(-moveBackNoise, moveBackNoise);
        toMoveDirection.y = 0;
        toMoveDirection.Normalize();
        //transform.Translate(toMoveDirection * moveBackDistance, Space.World);

        transform.DOMove(transform.position + toMoveDirection * _moveBackDistance * moveBackDistance, moveBackDuration)
            .SetEase(moveBackEase);
    }

    public float TakeHitStopSpeedTime = 0.1f;

    private void SetOriginalSpeed()
    {
        agent.speed = originalSpeed;
    }
}