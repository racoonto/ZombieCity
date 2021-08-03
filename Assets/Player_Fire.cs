using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Player : MonoBehaviour
{
    public GameObject bullet;
    public Transform bulletPosition;

    private float shootDelayEndTime;

    private void Fire()
    {
        if (Input.GetMouseButton(0))
        {
            if (shootDelayEndTime < Time.time)
            {
                animator.SetBool("Fire", true);
                shootDelayEndTime = Time.time + shootDelay;
                IncreaseRecoil();
                StartCoroutine(InstantiateBulletAndFlashBulletCo());
            }
        }
        else
        {
            animator.SetBool("Fire", false);
            DecreaseRecoil();
        }
    }

    private GameObject bulletLight;
    public float bulletFlashTime = 0.001f;

    private IEnumerator InstantiateBulletAndFlashBulletCo()
    {
        yield return null;
        Instantiate(bullet, bulletPosition.position, CalculateRecoil(transform.rotation));

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