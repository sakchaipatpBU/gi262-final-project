using System.Collections;
using UnityEngine;

public class MeleeEnemy : EnemyCharacter
{

    public override void AIBehavior()
    {
        float distance = Vector2.Distance(transform.position, playerCharacter.transform.position);

        if (distance <= attackRange)
        {
            rb.linearVelocity = Vector2.zero;
            TryAttack();
        }
        else if (distance <= detectionRange)
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
            SetAnimation(attackAnimName);
            attackCoroutine = StartCoroutine(AttackAnimCoroutine());
        }
    }

    public override void PerformAttack(Character target)
    {
        if (isDead) return;
        if (target == null) return;

        float distance = Vector2.Distance(transform.position, target.transform.position);
        if (distance <= attackRange)
        {
            Debug.Log($"{characterName} attack {target.characterName} with {atk}");
            target.gameObject.GetComponent<PlayerController>().SetHitDirection(transform.position);
            target.TakeDamage(atk);
        }
        else
        {
            Debug.Log($"{characterName} attack {target.characterName} , But Not Hit");
        }
        SoundManager.Instance.PlaySFX("Sword_Hit", 0.3f);
    }
    public override IEnumerator AttackAnimCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        SetAnimation(idleAnimName);
    }
}
