using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    public int power = 40;
    public float destroyDelay = 3;
    public float damageArea = 5;
    public GameObject destroyEffect;

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(destroyDelay);
        //좀비들한테 대미지 주자.

        var attackables = Physics.OverlapSphere(transform.position, damageArea, 1 << LayerMask.NameToLayer("Attackable"));
        foreach (var item in attackables)
        {
            var moveDirection = transform.position - item.transform.position;
            var zombie = item.GetComponent<Zombie>();
            if (zombie)
                zombie.TakeHit(power, moveDirection);
        }

        //폭발 이펙트 표시
        Instantiate(destroyEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}