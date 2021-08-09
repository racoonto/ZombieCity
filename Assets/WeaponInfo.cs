﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "New Weapon Info", menuName = "Scriptable Object/Weapon Info")]
public class WeaponInfo : MonoBehaviour
{
    public enum WeaponType
    {
        Gun,
        Melee, //근접공격. 총알없음
    }

    public WeaponType type;

    public int damage = 20;

    public AnimatorOverrideController overrideAnimator;

    public float delay = 0.2f;
    public float pushBackDistance = 0.1f;

    public int bulletCountInClip = 2; //탄창에 총알수
    public int MaxBulletCountInClip = 60; //탄창에 들어가는 최대수
    public int allBulletCount = 500; //가진 전체 총알수

    public int maxBulletCount = 500; // 최대로 가질 수 있는 총알수
    public float reloadTime = 1f;

    [Header("총")]
    public Transform bulletPosition;

    public Light bulletLight;

    internal void Init()
    {
        allBulletCount = Math.Min(allBulletCount, maxBulletCount);
        int reloadCount = MaxBulletCountInClip = Math.Min(allBulletCount, MaxBulletCountInClip);
        allBulletCount -= reloadCount;
        bulletCountInClip = reloadCount;
    }

    public GameObject bullet;

    [Header("근접공격")]
    public float attackStartTime = 0.1f;

    public float attackTime = 0.4f;
    public Collider attackCollider;
}