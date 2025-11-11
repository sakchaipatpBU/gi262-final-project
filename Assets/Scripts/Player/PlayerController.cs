using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private PlayerCharacter playerCharacter;
    
    private InputAction moveAction;
    private InputAction attackAction;
    private bool isActive = true;


    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private string currentAnimName;
    [SerializeField] private string idleAnimName;
    [SerializeField] private string attackAnimName;
    [SerializeField] private string walkAnimName;
    [SerializeField] private string getHitAnimName;
    [SerializeField] private string deadAnimName;
    [SerializeField] private int attackDirection = 1;
    [SerializeField] private int hitDirection = 1;


    [Header("Movement")]
    [SerializeField] private float directionX;
    [SerializeField] private float directionY;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float moveSpeedMultiplier;
    [SerializeField] private float smoothTime = 0.1f;

    [SerializeField] private Vector2 moveDirection;
    [SerializeField] private Vector2 currentVelocity;
    [SerializeField] private Vector2 targetVelocity;

    [Header("Attack")]
    [SerializeField] private bool canAttack = true;
    [SerializeField] private float attackCooldownTime = 0.5f;
    [SerializeField] private List<EnemyCharacter> enemies = new List<EnemyCharacter>();
    private Coroutine attackCooldownCoroutine;


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerCharacter = gameObject.GetComponent<PlayerCharacter>();
        animator = GetComponent<Animator>();
        moveAction = InputSystem.actions.FindAction("Move");
        attackAction = InputSystem.actions.FindAction("Attack");
        
        attackCooldownTime = animator.runtimeAnimatorController
            .animationClips.First(c => c.name
            .StartsWith("Human_Attack")).length;

        moveSpeed = playerCharacter.MoveSpeed;
        moveSpeedMultiplier = playerCharacter.MoveSpeedMultiplier;

        // setup animatiaon
        animator = playerCharacter.animator;
        idleAnimName = playerCharacter.idleAnimName;
        attackAnimName = playerCharacter.attackAnimName;
        walkAnimName = playerCharacter.walkAnimName;
        getHitAnimName = playerCharacter.getHitAnimName;
        deadAnimName = playerCharacter.deadAnimName;

        // look right side when start the game
        directionX = 1;
        directionY = 1;

        // set anim on start -> idle
        ToggleXDirection(directionX);
        ToggleYDirection(directionY);
        currentAnimName = idleAnimName;
        SetAnimation(idleAnimName);

        FindAllEnemyInScene();
    }
    private void Update()
    {
        
    }
    private void OnDisable()
    {
        rb.linearVelocity = Vector3.zero;
        SetAnimation(idleAnimName);
        animator.SetFloat("Speed", 0);
    }
    private void FixedUpdate()
    {
        if(!isActive)
        {
            rb.linearVelocity = Vector3.zero;
            animator.SetBool(deadAnimName, false);
            return;
        }

        if (canAttack && attackAction.IsPressed())
        {
            // cooldown starting..
            attackCooldownCoroutine = StartCoroutine(AttackActionCooldownCoroutine());

            // targetDirection depend on player look side
            if (directionX == 1)
            {
                if(directionY == 1)
                {
                    attackDirection = 4; // upper right side
                }
                else if(directionY == -1)
                {
                    attackDirection = 1; // lower right side
                }
                else
                {
                    Debug.Log("directionY is Not correct value");
                }
            }
            else if(directionX == -1)
            {
                if (directionY == 1)
                {
                    attackDirection = 3; // upper left side
                }
                else if (directionY == -1)
                {
                    attackDirection = 2; // lower left side
                }
                else
                {
                    Debug.Log("directionY is Not correct value");
                }
            }
            else
            {
                Debug.Log("directionX is Not correct value");
            }

            SetAnimatorDirection(attackDirection);
            SetTriggerAnimation(attackAnimName);
            AttackTarget();


            return;
        }
        moveDirection = moveAction.ReadValue<Vector2>();

        if(moveDirection == Vector2.zero)
        {
            // idle
            SetAnimation(idleAnimName);
            animator.SetFloat("Speed", 0);

            //return;
        }
        else
        {
            animator.SetFloat("Speed", moveSpeedMultiplier);
        }

        // x axis moving
        if (Mathf.Abs(moveDirection.x) > 0)
        {
            SetAnimation(walkAnimName);

            if (moveDirection.x > 0) // walk right
            {
                directionX = 1;

            }
            else if (moveDirection.x < 0) // walk left
            {
                directionX = -1;
            }

            targetVelocity.x = moveDirection.x * moveSpeed * moveSpeedMultiplier;


        }
        else // not walk on X axis
        {
            targetVelocity.x = 0;
        }

        // y axis moving
        if (Mathf.Abs(moveDirection.y) > 0)
        {
            SetAnimation(walkAnimName);

            if (moveDirection.y > 0) // walk up
            {
                directionY = 1;
            }
            else if (moveDirection.y < 0) // walk down
            {
                directionY = -1;
            }

            targetVelocity.y = moveDirection.y * moveSpeed * moveSpeedMultiplier;
        }
        else // not walk on Y axis -> look down
        {
            directionY = -1;
            targetVelocity.y = 0;
        }

        SetDirection(directionX, directionY);

        // smooth moving
        rb.linearVelocity = Vector2.SmoothDamp(rb.linearVelocity,
                targetVelocity, 
                ref currentVelocity, 
                smoothTime);

    }

    public void SetDirection(float x, float y)
    {
        ToggleXDirection(x);
        ToggleYDirection(y);
    }
    public void ToggleXDirection(float x)
    {
        animator.SetFloat("Horizontal", x);
    }
    public void ToggleYDirection(float y)
    {
        animator.SetFloat("Vertical", y);
    }
    public void SetAnimation(string animName)
    {
        if (animator != null)
        {
            if (currentAnimName != null)
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
    public void SetTriggerAnimation(string animName)
    {
        if(animator != null)
        {
            animator.SetTrigger(animName);
            Debug.Log($"{animName} animation was Triggered");
        }
        else
        {
            Debug.LogWarning($"SetTriggerAnimation : Animator Not found");
        }
    }
    private IEnumerator AttackActionCooldownCoroutine()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackCooldownTime);
        canAttack = true;
    }
    public void SetAnimatorDirection(int dir)
    {
        if (animator != null)
        {
            animator.SetFloat("Direction", dir);
        }
        else
        {
            Debug.LogWarning($"SetAnimatorDirection : Animator Not found");
        }
    }

    public void GetHitAnimation()
    {
        // set dir
        SetAnimatorDirection(hitDirection);
        // play anim
        SetTriggerAnimation(getHitAnimName);
    }
    public void SetHitDirection(Vector3 pos)
    {
        int dX = Math.Sign(pos.x - transform.position.x);
        int dY = Math.Sign(pos.y - transform.position.y);
        if(dX < 0)
        {
            if(dY < 0)
            {
                hitDirection = 2;
            }
            else
            {
                hitDirection = 3;
            }
        }
        else
        {
            if( dY < 0)
            {
                hitDirection = 1;
            }
            else
            {
                hitDirection = 4;
            }
        }

    }
    public void DeadAnimation()
    {
        // random 1 or 2 -> set animator. random
        int random = UnityEngine.Random.Range(0, 2);
        animator.SetFloat("Random", (float)random);
        // anim play
        SetAnimation(deadAnimName);

        isActive = false;
    }

    private void AttackTarget()
    {
        if (enemies.Count == 0) return;
        for (int i = enemies.Count - 1; i >= 0; i--)
        {
            bool isEnemyDead = false;

            float distance = Vector2.Distance(transform.position, enemies[i].transform.position);
            if (distance <= playerCharacter.AttackRange)
            {
                int dX = Math.Sign(enemies[i].transform.position.x - transform.position.x);
                if (dX == 1)
                {
                    if (attackDirection == 1 || attackDirection == 4)
                    {
                        isEnemyDead = enemies[i].GetComponent<EnemyCharacter>().TakeDamage(playerCharacter.Atk);
                    }
                }
                else if(dX == -1)
                {
                    if (attackDirection == 2 || attackDirection == 3)
                    {
                        isEnemyDead = enemies[i].GetComponent<EnemyCharacter>().TakeDamage(playerCharacter.Atk);
                    }
                }
            }
            if (isEnemyDead) enemies.Remove(enemies[i]);
        }
    }
    void FindAllEnemyInScene()
    {
        if(enemies.Count != 0)
        {
            enemies.Clear();
        }

        enemies = FindObjectsByType<EnemyCharacter>(FindObjectsSortMode.None).ToList();
    }

    public void UpdateMoveSpeedMultiplier()
    {
        moveSpeedMultiplier = playerCharacter.MoveSpeedMultiplier;
    }
}
