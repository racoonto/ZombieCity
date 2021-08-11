using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System.Linq;
using UnityEngine.Animations.Rigging;

public partial class Player : Actor
{
    public enum StateType
    {
        Idle,
        Move,
        TakeHit,
        Roll,
        Die,
        Reload,
    }

    public bool isFiring = false;
    public StateType stateType = StateType.Idle;

    public WeaponInfo mainWeapon;
    public WeaponInfo subWeapon;

    public WeaponInfo currentWeapon;
    public Transform rightWeaponPosition;

    new private void Awake()
    {
        base.Awake();
        animator = GetComponentInChildren<Animator>();
        InitWeapon(mainWeapon);
        InitWeapon(subWeapon);

        ChangeWeapon(mainWeapon);

        SetCinemachinCamera();

        HealthUI.Instance.SetGauge(hp, maxHp);

        AmmoUI.Instance.SetBulletCount(BulletCountInClip
            , MaxBulletCountInClip
            , AllBulletCount + BulletCountInClip
            , MaxBulletCount);
    }

    private Coroutine settingLookAtTargetCoHandle;

    private void Start()
    {
        settingLookAtTargetCoHandle = StartCoroutine(SettingLookAtTargetCo());
    }

    private IEnumerator SettingLookAtTargetCo()
    {
        MultiAimConstraint multiAimConstraint = GetComponentInChildren<MultiAimConstraint>();
        RigBuilder rigBuilder = GetComponentInChildren<RigBuilder>();
        while (stateType != StateType.Die)
        {
            List<Zombie> allZombies = Zombie.Zombies;
            Transform lastTarget = null;
            if (allZombies.Count > 0)
            {
                var nearestZombie = allZombies.OrderBy(x => Vector3.Distance(x.transform.position, transform.position))
                    .First();

                if (lastTarget != nearestZombie.transform)
                {
                    lastTarget = nearestZombie.transform;
                    var array = multiAimConstraint.data.sourceObjects;
                    array.Clear();
                    array.Add(new WeightedTransform(lastTarget, 1));
                    multiAimConstraint.data.sourceObjects = array;
                    rigBuilder.Build();
                }
            }

            yield return new WaitForSeconds(1);
        }
    }

    internal void RetargetingLookat()
    {
        MultiAimConstraint multiAimConstraint = GetComponentInChildren<MultiAimConstraint>();
        multiAimConstraint.data.sourceObjects = new WeightedTransformArray();
        // multiAimConstraint.data.sourceObjects.Clear()시 실패했음. 이유모름.
        GetComponentInChildren<RigBuilder>().Build();

        StopCoroutine(settingLookAtTargetCoHandle);
        settingLookAtTargetCoHandle = StartCoroutine(SettingLookAtTargetCo());
    }

    private void InitWeapon(WeaponInfo weaponInfo)
    {
        if (weaponInfo)
        {
            weaponInfo = Instantiate(weaponInfo, transform);
            weaponInfo.Init();
            weaponInfo.gameObject.SetActive(false);
        }
    }

    private GameObject currentWeaponGo;

    private void ChangeWeapon(WeaponInfo _weaponInfo)
    {
        Destroy(currentWeaponGo);
        currentWeapon = _weaponInfo;

        animator.runtimeAnimatorController = currentWeapon.overrideAnimator;
        //rightWeaponPosition 부모
        var weaponInfo = Instantiate(currentWeapon, rightWeaponPosition);
        currentWeaponGo = weaponInfo.gameObject;
        weaponInfo.transform.localScale = currentWeapon.gameObject.transform.localScale;
        weaponInfo.transform.localPosition = currentWeapon.gameObject.transform.localPosition;
        weaponInfo.transform.localRotation = currentWeapon.gameObject.transform.localRotation;
        currentWeapon = weaponInfo;

        if (currentWeapon.attackCollider)
            currentWeapon.attackCollider.enabled = false;

        //bulletPosition = weaponInfo.bulletPosition;

        if (weaponInfo.bulletLight != null)
            bulletLight = weaponInfo.bulletLight.gameObject;
        shootDelay = currentWeapon.delay;
    }

    [ContextMenu("SetCinemachinCamera")]
    private void SetCinemachinCamera()
    {
        var vcs = FindObjectsOfType<CinemachineVirtualCamera>();
        foreach (var item in vcs)
        {
            item.Follow = transform;
            item.LookAt = transform;
        }
    }

    private void Update()
    {
        if (Time.deltaTime == 0)
            return;

        if (stateType == StateType.Die)
            return;

        if (stateType != StateType.Roll)
        {
            LookAtMouse();
            Move();
            Fire();
            Roll();
            ReloadBullet();
            if (Input.GetKeyDown(KeyCode.Tab))
                ToggleChangeWeapon();
        }
    }

    private void ReloadBullet()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(ReloadBulletCo());
        }
    }

    private IEnumerator ReloadBulletCo()
    {
        stateType = StateType.Reload;
        animator.SetTrigger("Reload");
        int reloadCount = Math.Min(AllBulletCount, MaxBulletCountInClip);

        AmmoUI.Instance.StartReload(reloadCount
            , MaxBulletCountInClip
            , AllBulletCount
            , MaxBulletCount
            , ReloadTime);
        yield return new WaitForSeconds(ReloadTime);
        stateType = StateType.Idle;
        BulletCountInClip = reloadCount;
        AllBulletCount -= reloadCount;
    }

    private bool toggleWeapon = false;

    private void ToggleChangeWeapon()
    {
        ChangeWeapon(toggleWeapon == true ? mainWeapon : subWeapon);
        toggleWeapon = !toggleWeapon;
    }

    private void Roll()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            StartCoroutine(RollCo());
    }

    public AnimationCurve rollingSpeedAC;

    public float rollingSpeedUserMultipy = 1;

    private IEnumerator RollCo()
    {
        Endfiring();

        // 회전 방향은 처음 바라보던 방향으로 고정.
        // 총알 금지. 움직이는거 금지.마우스 바라보는거 금지.
        stateType = StateType.Roll;
        // 구르는 애니메이션 재생.
        animator.SetTrigger("Roll");
        // 구르는 동안 이동 스피드를 빠르게 하기.
        float starTime = Time.time;
        float endTime = starTime + rollingSpeedAC[rollingSpeedAC.length - 1].time;
        while (endTime > Time.time)
        {
            float time = Time.time - starTime;
            float rollingSpeedMultipy = rollingSpeedAC.Evaluate(time) * rollingSpeedUserMultipy;
            //print($"{time}:{rollingSpeedMultipy} : {rollingSpeedAC[rollingSpeedAC.length - 1].time}");

            transform.Translate(transform.forward * speed * rollingSpeedMultipy * Time.deltaTime, Space.World);
            yield return null;
        }
        stateType = StateType.Idle;
    }

    private Plane plane = new Plane(new Vector3(0, 1, 0), 0);

    private void LookAtMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (plane.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            Vector3 dir = hitPoint - transform.position;
            dir.y = 0;
            //dir.y = transform.position.y;
            dir.Normalize();
            transform.forward = dir;
        }
    }

    private void Move()
    {
        Vector3 move = Vector3.zero;
        if (Input.GetKey(KeyCode.W)) move.z = 1;
        if (Input.GetKey(KeyCode.S)) move.z = -1;   // 누름
        if (Input.GetKey(KeyCode.A)) move.x = -1;
        if (Input.GetKey(KeyCode.D)) move.x = 1;
        if (move != Vector3.zero)
        {
            Vector3 relateMove;
            relateMove = Camera.main.transform.forward * move.z; // 0, -1, 0
            relateMove += Camera.main.transform.right * move.x;
            relateMove.y = 0;
            move = relateMove;

            move.Normalize(); // z : -1, x : 0

            float _speed = isFiring ? speedWhileShooting : speed;
            transform.Translate(move * _speed * Time.deltaTime, Space.World);

            //뱡향을 앵글로 전환하기
            //앵글을 방향으로 전환하기
            float forwardAngle = transform.forward.VectorToDegree(); //0~360각도
            float moveDegree = move.VectorToDegree(); // 방향 -> 앵글
            float dirRadian = (moveDegree - forwardAngle + 90) * Mathf.PI / 180; // 0~360; 앵글을 라디안으로
            Vector3 dir;
            dir.x = Mathf.Cos(dirRadian);
            dir.z = Mathf.Sin(dirRadian);

            animator.SetFloat("DirX", dir.x);
            animator.SetFloat("DirY", dir.z);

            ////* transform.forward 는 마우스 방향이다
            //if (Mathf.RoundToInt(transform.forward.x) == 1 || Mathf.RoundToInt(transform.forward.x) == -1)
            //{
            //    animator.SetFloat("DirX", transform.forward.z * move.z);
            //    animator.SetFloat("DirY", transform.forward.x * move.x);
            //}
            //else
            //{
            //    animator.SetFloat("DirX", transform.forward.x * move.x);
            //    animator.SetFloat("DirY", transform.forward.z * move.z);
            //}
        }

        //animator.SetFloat("DirX", transform.forward.x);
        //animator.SetFloat("DirY", transform.forward.z);

        animator.SetFloat("Speed", move.sqrMagnitude);
    }

    new internal void TakeHit(int damage)
    {
        base.TakeHit(damage);
        HealthUI.Instance.SetGauge(hp, maxHp);
        animator.SetTrigger("TakeHit");

        if (hp <= 0)
        {
            StartCoroutine(DieCo());
        }
    }

    public float diePreDelayTime = 0.3f;

    private IEnumerator DieCo()
    {
        stateType = StateType.Die;
        GetComponent<Collider>().enabled = false;
        yield return new WaitForSeconds(diePreDelayTime);
        animator.SetTrigger("Die");
    }

    public float speed = 5;
    public float speedWhileShooting = 3;

    public void OnZombieEnter(Collider other)
    {
        var zombie = other.GetComponent<Zombie>();
        zombie.TakeHit(currentWeapon.damage
            , currentWeapon.gameObject.transform.forward
            , currentWeapon.pushBackDistance);
    }
}