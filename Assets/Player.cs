using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Player : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        bulletLight = GetComponentInChildren<Light>(true).gameObject;
    }

    private void Update()
    {
        if (Time.deltaTime == 0)
            return;

        if (stateType != StateType.Roll)
        {
            LookAtMouse();
            Move();
            Fire();
        }

        Roll();
    }

    private void Roll()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            StartCoroutine(RollCo());
    }

    public AnimationCurve rollingSpeedAC;

    public float rollingSpeedUserMultiply = 1;

    public enum StateType
    {
        Idle,
        Move,
        Attack,
        TakeHit,
        Roll,
        Die,
    }

    public StateType stateType = StateType.Idle;

    private IEnumerator RollCo()
    {
        animator.SetBool("Fire", false);
        DecreaseRecoil();

        stateType = StateType.Roll;
        //구르는 애니메이션 재생.
        animator.SetTrigger("Roll");

        //구르는 동안 이동 스피드를 빠르게 하기
        float startTime = Time.time;
        float endTime = startTime + rollingSpeedAC[rollingSpeedAC.length - 1].time;
        while (endTime > Time.time)
        {
            float time = Time.time - startTime;
            float rollingSpeedmultiply = rollingSpeedAC.Evaluate(time) * rollingSpeedUserMultiply;

            //회전 방향은 처음 바라보던 방향으로 고정.
            transform.Translate(transform.forward * speed * rollingSpeedmultiply * Time.deltaTime, Space.World);

            yield return null;
        }
        stateType = StateType.Idle;

        //총알 금지. 움직이는거 금지. 마우스 바라보는 거 금지.
    }

    private Plane plane = new Plane(new Vector3(0, 1, 0), 0);

    private void LookAtMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (plane.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            Vector3 dir = hitPoint - transform.position;
            dir.y = transform.position.y;
            dir.Normalize();
            transform.forward = dir;
        }
    }

    //private void Fire()
    //{
    //    if (Input.GetKey(KeyCode.Mouse0))
    //    {
    //        //animator.Play("Shoot");
    //        animator.SetBool("Fire", true);
    //        Instantiate(bullet, bulletPosition.position, transform.rotation);
    //    }
    //    else
    //    {
    //        animator.SetBool("Fire", false);
    //    }
    //}

    private void Move()
    {
        Vector3 move = Vector3.zero;
        if (Input.GetKey(KeyCode.W)) move.z = 1;
        if (Input.GetKey(KeyCode.S)) move.z = -1;
        if (Input.GetKey(KeyCode.A)) move.x = -1;
        if (Input.GetKey(KeyCode.D)) move.x = 1;
        if (move != Vector3.zero)
        {
            //카메라 기준으로 이동하는 코드
            Vector3 relateMove;
            relateMove = Camera.main.transform.forward * move.z;
            relateMove += Camera.main.transform.right * move.x;
            relateMove.y = 0;
            move = relateMove;

            move.Normalize();
            transform.Translate(move * speed * Time.deltaTime, Space.World);
            //transform.forward = move;
        }

        animator.SetFloat("DirX", move.x);
        animator.SetFloat("DirY", move.z);
        animator.SetFloat("Speed", move.sqrMagnitude);
    }

    public float speed = 5;
    //public GameObject bullet;
    //public Transform bulletPosition;
}