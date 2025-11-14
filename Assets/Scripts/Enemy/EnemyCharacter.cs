using System;
using System.Collections;
using UnityEngine;

public abstract class EnemyCharacter : Character
{
    [SerializeField] protected PlayerCharacter playerCharacter;
    [SerializeField] protected PlayerController playerController;

    [SerializeField] protected float detectionRange = 5f;
    [SerializeField] protected float attackCooldown = 1.5f;
    [SerializeField] protected float lastAttackTime;

    [SerializeField] protected int expDrop = 100;
    [SerializeField] protected int goldDrop = 10;

    [Header("Animation")]
    [SerializeField] protected string currentAnimName;
    protected Coroutine attackCoroutine;
    protected Coroutine getHitCoroutine;
    protected Coroutine dieCoroutine;
    private void Awake()
    {
        playerCharacter = GameObject.Find("Player").GetComponent<PlayerCharacter>();
        playerController = playerCharacter.gameObject.GetComponent<PlayerController>();
    }
    public override void Start()
    {
        base.Start();
    }
    private void Update()
    {
        if (playerCharacter == null || isDead) return;

        AIBehavior();
    }
    public abstract void AIBehavior();
    public abstract void TryAttack();
    public abstract void PerformAttack(Character target);
    public abstract IEnumerator AttackAnimCoroutine();
    public virtual void MoveToPlayer()
    {
        if (playerCharacter.IsDead) return;
        Vector2 dir = (playerCharacter.transform.position - transform.position).normalized;
        rb.linearVelocity = dir * moveSpeed;

        // anim walk
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
        }
        float x = Mathf.Sign(dir.x);
        float y = Mathf.Sign(dir.y);
        SetDirection(x, y);
        SetAnimation(walkAnimName);
    }
    public override bool TakeDamage(float damage)
    {
        if (isDead) return true;

        hp -= damage;
        Debug.Log($"{characterName} got {damage} damage. Now {hp} / {maxHp} hp.");

        if (hp <= 0)
        {
            Dead();
            return true;
        }
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
        }
        if (getHitCoroutine != null)
        {
            StopCoroutine(getHitCoroutine);
        }
        getHitCoroutine = StartCoroutine(GetHitCoroutine());

        return false;
    }
    public override void Dead()
    {
        isDead = true;
        hp = 0;
        Debug.Log($"{characterName} is dead!");
        // to-do add effect

        rb.linearVelocity = Vector2.zero;
        // TO-DO add Effect drop item, gold / quest progress

        playerCharacter.AddExperience(expDrop);
        playerCharacter.AddGold(goldDrop);
        playerCharacter.GainHp(50);

        // quest progress
        QuestManager.Instance.ReportProgress(characterName, QuestObjectiveType.Kill);

        int x = UnityEngine.Random.Range(0, 2);
        ToggleXDirection((float)x);
        SetAnimation(deadAnimName);

        dieCoroutine = StartCoroutine(DieCoroutine());
    }
    protected IEnumerator GetHitCoroutine()
    {
        SetAnimation(getHitAnimName);
        yield return new WaitForSeconds(0.5f);
        SetAnimation(idleAnimName);
    }
    protected IEnumerator DieCoroutine()
    {
        yield return new WaitForSeconds(1.5f);
        gameObject.SetActive(false);
    }

    #region Animation
    public void SetDirection(float x, float y)
    {
        ToggleXDirection(x);
        ToggleYDirection(y);
    }
    public void ToggleXDirection(float x)
    {
        animator.SetFloat("X", x);
    }
    public void ToggleYDirection(float y)
    {
        animator.SetFloat("Y", y);
    }
    public void SetAnimation(string animName)
    {
        if (animator != null)
        {
            if (currentAnimName != null && !String.IsNullOrEmpty(currentAnimName))
                TurnOffCurrentAnimation(); // stop old

            animator.SetBool(animName, true); // run new
            currentAnimName = animName;
        }
        else
        {
            Debug.LogWarning($"SetAnimation : Animator Not found");
        }
    }
    public void TurnOffCurrentAnimation()
    {
        animator.SetBool(currentAnimName, false);
    }
    #endregion
}
