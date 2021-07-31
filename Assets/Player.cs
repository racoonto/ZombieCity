using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        Vector3 move = Vector3.zero;
        if (Input.GetKey(KeyCode.W))
            move.z = 1;
        if (Input.GetKey(KeyCode.S))
            move.z = -1;
        if (Input.GetKey(KeyCode.A))
            move.x = -1;
        if (Input.GetKey(KeyCode.D))
            move.x = 1;

        if (move != Vector3.zero)
        {
            move.Normalize();
            transform.Translate(move * speed * Time.deltaTime, Space.World);
            transform.forward = move;
        }

        //if (Input.GetKeyDown(KeyCode.Alpha1))
        //{
        //    animator.Play("Shoot");
        //}
        //if (Input.GetKeyDown(KeyCode.Alpha2))
        //{
        //    animator.Play("Run");
        //}

        if (Input.GetKey(KeyCode.Space))
        {
            //Camera.main.ScreenToWorldPoint(Input.mousePosition); //월드상 마우스위치

            animator.Play("Shoot");
            Instantiate(bullet, bulletPosition.position, transform.rotation);
        }
    }

    public float speed = 5;
    public GameObject bullet;
    public Transform bulletPosition;
}