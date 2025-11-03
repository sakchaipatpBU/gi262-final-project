using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;

    
    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private string currentAnnimName;
    [SerializeField] private string idleAnimName = "Idle";
    [SerializeField] private string attackAnimName = "Attack";
    [SerializeField] private string walkAnimName = "Walking";
    [SerializeField] private string getHitAnimName = "Damage";
    [SerializeField] private string deadAnimName = "Dead";
    [SerializeField] private int targetDirection = 1;


    [Header("Movement")]
    [SerializeField] private InputAction moveAction;

    [SerializeField] private float directionX;
    [SerializeField] private float directionY;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float moveSpeedMultiplier = 1;
    public float MoveSpeedMultiplier
    {
        get { return moveSpeedMultiplier; }
        set
        {
            moveSpeedMultiplier = value; // ex. 1.1 , 1.25 , 2.5
        }
    }
    [SerializeField] private float smoothTime = 0.1f;

    [SerializeField] private Vector2 moveDirection;
    [SerializeField] private Vector2 currentVelocity;
    [SerializeField] private Vector2 targetVelocity;

    [Header("Attack")]
    private InputAction attackAction;
    private bool canAttack = true;
    private float attackCooldownTime = 0.5f;


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        animator = GetComponent<Animator>();
        moveAction = InputSystem.actions.FindAction("Move");
        attackAction = InputSystem.actions.FindAction("Attack");
        
        attackCooldownTime = animator.runtimeAnimatorController
            .animationClips.First(c => c.name
            .StartsWith("Human_Attack")).length;

        // look right side when start the game
        directionX = 1;
        directionY = 1;

        // set anim on start -> idle
        ToggleXDirection(directionX);
        ToggleYDirection(directionY);
        SetAnimation(idleAnimName);
    }

    private void FixedUpdate()
    {
        if(canAttack && attackAction.IsPressed())
        {
            // cooldown starting..
            StartCoroutine(AttackActionCooldownCoroutine());

            // targetDirection depend on player look side
            if (directionX == 1)
            {
                if(directionY == 1)
                {
                    targetDirection = 4; // upper right side
                }
                else if(directionY == -1)
                {
                    targetDirection = 1; // lower right side
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
                    targetDirection = 3; // upper left side
                }
                else if (directionY == -1)
                {
                    targetDirection = 2; // lower left side
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

            SetAnimatorDirection(targetDirection);
            SetTriggerAnimation(attackAnimName);


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
            if (currentAnnimName != null)
                TurnOffCurrentAnimation(); // stop old

            animator.SetBool(animName, true); // run new
            currentAnnimName = animName;
        }
        else
        {
            Debug.LogWarning($"SetAnimation : Animator Not found");
        }
    }
    public void TurnOffCurrentAnimation()
    {
        animator.SetBool(currentAnnimName, false);
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
}
