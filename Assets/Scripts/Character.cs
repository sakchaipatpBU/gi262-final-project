using UnityEngine;

public abstract class Character : MonoBehaviour
{
    public Rigidbody2D rb;

    [Header("Character Stats")]
    public string characterName;
    [SerializeField] protected float maxHp;
    public float MaxHp { get { return maxHp; } }
    [SerializeField] protected float hp;
    public float Hp { get { return hp; } }

    [SerializeField] protected float atk;
    public float Atk { get { return atk; } set { atk = value; } }
    [SerializeField] protected float moveSpeed;
    public float MoveSpeed { get { return moveSpeed; } }
    [SerializeField] protected float attackRange = 1f;
    public float AttackRange {  get { return attackRange; } }
    protected bool isDead = false;
    public bool IsDead {  get { return isDead; } }



    [Header("Animation")]
    public Animator animator;
    //public string currentAnimName;
    public string idleAnimName = "Idle";
    public string attackAnimName = "Attack";
    public string walkAnimName = "Walking";
    public string getHitAnimName = "Damage";
    public string deadAnimName = "Dead";
    public int attackDirection = 1;
    public int hitDirection = 1;



    public virtual void Start()
    {
        hp = maxHp;
    }

    public abstract bool TakeDamage(float damage);
    public abstract void Dead();

    public virtual void PerformAttack(Character target)
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

    }
    
}
