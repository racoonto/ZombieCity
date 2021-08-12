using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Granade : MonoBehaviour
{
    public ProjectileArc projectileArc;
    public Transform firePoint;
    public float currentAngle;
    public Cursor cursor;
    public float speed = 20;

    private void Start()
    {
        projectileArc = GetComponent<ProjectileArc>();
        firePoint = transform;
        cursor = FindObjectOfType<Cursor>();
    }

    private void Update()
    {
        SetTargetWithSpeed(cursor.transform.position, speed);
    }

    public void SetTargetWithSpeed(Vector3 point, float speed)
    {
        Vector3 direction = point - firePoint.position;
        float yOffset = direction.y;
        direction = Math3d.ProjectVectorOnPlane(Vector3.up, direction);
        float distance = direction.magnitude;

        float angle0, angle1;
        bool targetInRange = ProjectileMath.LaunchAngle(speed, distance, yOffset, Physics.gravity.magnitude, out angle0, out angle1);

        if (targetInRange)
            currentAngle = angle0;

        projectileArc.UpdateArc(speed, distance, Physics.gravity.magnitude, currentAngle, direction, targetInRange);
    }
}