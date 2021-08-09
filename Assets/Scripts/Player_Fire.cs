﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Player : Actor
{
    public int BulletCountInClip //탄창에 총알수
    {
        get { return currentWeapon.bulletCountInClip; }
        set { currentWeapon.bulletCountInClip = value; }
    }

    public int MaxBulletCountClip => currentWeapon.MaxBulletCountInClip; //탄창에 들어가는 최대수

    public int allBulletCount //가진 전체 총알수
    {
        get => currentWeapon.allBulletCount;
        set => currentWeapon.allBulletCount = value;
    }

    public int maxBulletCount => currentWeapon.maxBulletCount; // 최대로 가질 수 있는 총알수
    public float reloadTime => currentWeapon.reloadTime;

    public GameObject bullet => currentWeapon.bullet;
    public Transform bulletPosition => currentWeapon.bulletPosition;

    private float shootDelayEndTime;

    private void Fire()
    {
        if (Input.GetMouseButton(0) && BulletCountInClip > 0)
        {
            isFiring = true;
            if (shootDelayEndTime < Time.time)
            {
                BulletCountInClip--;

                animator.SetTrigger("StartFire");
                AmmoUI.Instance.SetBulletCount(BulletCountInClip, MaxBulletCountClip, allBulletCount + BulletCountInClip, maxBulletCount);

                shootDelayEndTime = Time.time + shootDelay;
                switch (currentWeapon.type)
                {
                    case WeaponInfo.WeaponType.Gun:
                        IncreaseRecoil();
                        currentWeapon.StartCoroutine(InstantiateBulletAndFlashBulletCo());
                        break;

                    case WeaponInfo.WeaponType.Melee:
                        //무기의 콜라이더를 활성화 하자.
                        currentWeapon.StartCoroutine(MeleeAttackCo());
                        break;
                }
            }
        }
        else
        {
            Endfiring();
        }
    }

    private IEnumerator MeleeAttackCo()
    {
        yield return new WaitForSeconds(currentWeapon.attackStartTime);
        currentWeapon.attackCollider.enabled = true;
        yield return new WaitForSeconds(currentWeapon.attackTime);
        currentWeapon.attackCollider.enabled = false;
    }

    private void Endfiring()
    {
        //animator.SetBool("Fire", false);
        DecreaseRecoil();
        isFiring = false;
    }

    private GameObject bulletLight;
    public float bulletFlashTime = 0.001f;

    private IEnumerator InstantiateBulletAndFlashBulletCo()
    {
        yield return null; // 총쏘는 애니메이션 시작후에 총알 발사하기 위해서 1Frame쉼
        GameObject bulletGo = Instantiate(bullet, bulletPosition.position, CalculateRecoil(transform.rotation));
        bulletGo.GetComponent<Bullet>().pushBackDistance = currentWeapon.pushBackDistance;

        bulletLight.SetActive(true);
        yield return new WaitForSeconds(bulletFlashTime);
        bulletLight.SetActive(false);
    }

    private float recoilValue = 0f;
    private float recoilMaxValue = 1.5f;
    private float recoilLerpValue = 0.1f;

    private void IncreaseRecoil()
    {
        recoilValue = Mathf.Lerp(recoilValue, recoilMaxValue, recoilLerpValue);
    }

    private void DecreaseRecoil()
    {
        recoilValue = Mathf.Lerp(recoilValue, 0, recoilLerpValue);
    }

    private Vector3 recoil;

    private Quaternion CalculateRecoil(Quaternion rotation)
    {
        recoil = new Vector3(Random.Range(-recoilValue, recoilValue), Random.Range(-recoilValue, recoilValue), 0);
        return Quaternion.Euler(rotation.eulerAngles + recoil);
    }

    [SerializeField] private float shootDelay = 0.05f;
}