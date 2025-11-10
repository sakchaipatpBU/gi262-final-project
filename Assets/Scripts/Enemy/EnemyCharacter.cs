using UnityEngine;


public class EnemyCharacter : Character
{
    [SerializeField] private PlayerCharacter playerCharacter;
    [SerializeField] private PlayerController playerController;

    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private float lastAttackTime;


    [SerializeField] private int expDrop = 100;
    [SerializeField] private int goldDrop = 10;


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
    private void TryAttack()
    {
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            lastAttackTime = Time.time;
            PerformAttack(playerCharacter);
        }
    }
    private void MoveToPlayer()
    {
        if(playerCharacter.IsDead) return;
        Vector2 dir = (playerCharacter.transform.position - transform.position).normalized;
        rb.linearVelocity = dir * moveSpeed;
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

        gameObject.SetActive(false);
    }
}
