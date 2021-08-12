using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Player : Actor
{
    public int BulletCountInClip
    {
        get { return currentWeapon.bulletCountInClip; }       // 탄창에 총알수
        set { currentWeapon.bulletCountInClip = value; }
    }

    public int MaxBulletCountInClip => currentWeapon.maxBulletCountInClip;  // 탄창에 들어가는 최대수

    public int AllBulletCount
    {
        get => currentWeapon.allBulletCount;              // 가진 전체 총알수.
        set => currentWeapon.allBulletCount = value;
    }

    public int MaxBulletCount => currentWeapon.maxBulletCount;              // 최대로 가질 수 있는 총알수.
    public float ReloadTime => currentWeapon.reloadTime;

    public GameObject Bullet => currentWeapon.bullet;
    public Transform BulletPosition => currentWeapon.bulletPosition;

    private float shootDelayEndTime;

    private void Fire()
    {
        if (Input.GetMouseButton(0))
        {
            if (BulletCountInClip > 0)
            {
                isFiring = true;
                if (shootDelayEndTime < Time.time)
                {
                    BulletCountInClip--;
                    animator.SetTrigger("StartFire");
                    //animator.SetBool("Fire", true);
                    AmmoUI.Instance.SetBulletCount(BulletCountInClip
                        , MaxBulletCountInClip
                        , AllBulletCount + BulletCountInClip
                        , MaxBulletCount);

                    shootDelayEndTime = Time.time + shootDelay;
                    switch (currentWeapon.type)
                    {
                        case WeaponInfo.WeaponType.Gun:
                            IncreaseRecoil();
                            currentWeapon.StartCoroutine(InstantiateBulletAndFlashBulletCo());
                            break;

                        case WeaponInfo.WeaponType.Melee:
                            // 무기의 컬라이더를 활성화 하자.
                            currentWeapon.StartCoroutine(MeleeAttackCo());
                            break;

                        case WeaponInfo.WeaponType.Throw:
                            // 무기의 컬라이더를 활성화 하자.
                            currentWeapon.StartCoroutine(ThrowAttackCo());
                            break;
                    }
                }
            }
            else
            {
                if (reloadAlertDelayEndTime < Time.time)
                {
                    reloadAlertDelayEndTime = Time.time + reloadAlertDelay;
                    // 리로드 글자 표시하자.
                    CreateTextEffect("Reload!", "TalkEffect", transform.position
                        , Color.white, transform);
                }
            }
        }
        else
        {
            Endfiring();
        }
    }

    private IEnumerator ThrowAttackCo()
    {
        currentWeapon.GetComponentInChildren<GrenadeLauncher>().ThrowObject();

        yield return null;
    }

    [SerializeField] private float reloadAlertDelay = 1f;
    private float reloadAlertDelayEndTime;

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
        GameObject bulletGo = Instantiate(Bullet, BulletPosition.position, CalculateRecoil(transform.rotation));
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