using System.Collections;
using UnityEngine;

public class RangeEnemy : EnemyCharacter
{
    [Header("Ranged Attack")]
    public GameObject projectilePrefab;
    public Transform firePoint;

    [SerializeField] private float castAttackTime;
    private bool canMove;

    public override void Start()
    {
        base.Start();
        canMove = true;
    }
    public override void AIBehavior()
    {
        float distance = Vector2.Distance(transform.position, playerCharacter.transform.position);

        if (distance <= attackRange)
        {
            rb.linearVelocity = Vector2.zero;
            TryAttack();
        }
        else if (distance <= detectionRange && canMove)
        {
            MoveToPlayer();
        }
        else
        {
            SetAnimation(idleAnimName);
            rb.linearVelocity = Vector2.zero; // stop when out of area
        }
    }
    public override void TryAttack()
    {
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            lastAttackTime = Time.time;
            PerformAttack(playerCharacter);
            SetAnimation(idleAnimName);
            attackCoroutine = StartCoroutine(AttackAnimCoroutine());
        }
    }
    public override void PerformAttack(Character target)
    {
        if (isDead) return;
        if (target == null) return;

        if (projectilePrefab != null && firePoint != null)
        {
            Vector2 dir = (playerCharacter.transform.position - firePoint.position).normalized;
            GameObject bullet = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
            bullet.GetComponent<EnemyProjectile>().Initialize(dir, atk);
        }
    }
    public override IEnumerator AttackAnimCoroutine()
    {
        canMove = false;
        yield return new WaitForSeconds(castAttackTime);
        canMove = true;
    }
}
