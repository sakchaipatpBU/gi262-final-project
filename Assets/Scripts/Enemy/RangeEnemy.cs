using System.Collections;
using UnityEngine;

public class RangeEnemy : EnemyCharacter
{
    [Header("Ranged Attack")]
    public GameObject projectilePrefab;
    public Transform firePoint;

    public override void Start()
    {
        base.Start();
        detectionRange = 8f;
    }
    public override void TryAttack()
    {
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            lastAttackTime = Time.time;
            //SetAnimation(attackAnimName);
            SetAnimation(idleAnimName);
            StartCoroutine(ShootProjectile());
        }
    }

    IEnumerator ShootProjectile()
    {
        yield return new WaitForSeconds(0.3f); // timing ก่อนยิง
        if (projectilePrefab != null && firePoint != null)
        {
            Vector2 dir = (playerCharacter.transform.position - firePoint.position).normalized;
            GameObject bullet = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
            bullet.GetComponent<EnemyProjectile>().Initialize(dir, atk);
        }
        yield return new WaitForSeconds(0.5f);
    }
}
