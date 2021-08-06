using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "New Weapon Info", menuName = "Scriptable Object/Weapon Info")]
public class WeaponInfo : MonoBehaviour
{
    public int damage = 20;
    public GameObject bullet;

    public Transform bulletPosition;
    public Light bulletLight;

    public AnimatorOverrideController overrideAnimator;

    public float delay = 0.2f;
    public int maxBulletCount = 6;
}