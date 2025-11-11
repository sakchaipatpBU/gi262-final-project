using System;
using System.Collections;
using UnityEngine;

public class EnemyCharacter : Character
{
    [SerializeField] protected PlayerCharacter playerCharacter;
    [SerializeField] protected PlayerController playerController;

    [SerializeField] protected float detectionRange = 5f;
    [SerializeField] protected float attackCooldown = 1.5f;
    [SerializeField] protected float lastAttackTime;


    [SerializeField] protected int expDrop = 100;
    [SerializeField] protected int goldDrop = 10;

    [Header("Animation")]
    [SerializeField] protected Animator animator;
    [SerializeField] protected string currentAnimName;
    [SerializeField] protected string idleAnimName = "Idle";
    [SerializeField] protected string walkAnimName = "Walk";
    [SerializeField] protected string attackAnimName = "Attack";
    [SerializeField] protected string getHitAnimName = "Dmg";
    [SerializeField] protected string deadAnimName = "Die";
    protected Coroutine attackCoroutine;
    protected Coroutine dieCoroutine;


    public EnemyCharacter()
    {

    }
    public EnemyCharacter(string _characterName, float _hp = 100, 
        float _atk = 5, float _moveSpeed = 2)
    {
        characterName = _characterName;
        hp = _hp;
        atk = _atk;
        moveSpeed = _moveSpeed;
    }
    public EnemyCharacter(string _characterName, float _hp = 100,
        float _atk = 5, float _moveSpeed = 2, float _detectionRange = 2f,
        float _atkRange = 1f, float _attackCooldown = 1.5f)
    {
        characterName = _characterName;
        hp = _hp;
        atk = _atk;
        moveSpeed = _moveSpeed;
        detectionRange = _detectionRange;
        attackRange = _atkRange;
        attackCooldown = _attackCooldown;
    }

    public override void Start()
    {
        base.Start();
        rb = gameObject.GetComponent<Rigidbody2D>();
        playerCharacter = GameObject.Find("Player").GetComponent<PlayerCharacter>();
        playerController = playerCharacter.gameObject.GetComponent<PlayerController>();
        animator = gameObject.GetComponent<Animator>();
    }

    private void Update()
    {
        if (playerCharacter == null || isDead) return;

        AIBehavior();
    }
    private void AIBehavior()
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
            rb.linearVelocity = Vector2.zero; // stop when out of area
        }
    }
    public virtual void TryAttack()
    {
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            lastAttackTime = Time.time;
            PerformAttack(playerCharacter);
            // attack anim
            SetAnimation(attackAnimName);
            attackCoroutine = StartCoroutine(AttackCoroutine());
        }
    }
    private void MoveToPlayer()
    {
        if(playerCharacter.IsDead) return;
        Vector2 dir = (playerCharacter.transform.position - transform.position).normalized;
        rb.linearVelocity = dir * moveSpeed;

        // anim walk
        if(attackCoroutine != null)
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
        SetAnimation(getHitAnimName);
        return false;
    }
    public override void Dead()
    {
        base.Dead();
        rb.linearVelocity = Vector2.zero;
        // TO-DO add Effect drop item, gold / quest progress
        playerCharacter.AddExperience(expDrop);
        playerCharacter.AddGold(goldDrop);

        // quest progress
        QuestManager.Instance.ReportProgress(characterName, QuestObjectiveType.Kill);

        int x = UnityEngine.Random.Range(0, 2);
        ToggleXDirection((float)x);
        SetAnimation(deadAnimName);

        dieCoroutine = StartCoroutine(DieCoroutine());

    }
    IEnumerator AttackCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        SetAnimation(idleAnimName);
    }
    IEnumerator DieCoroutine()
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
