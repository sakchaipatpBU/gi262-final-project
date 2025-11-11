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
    [SerializeField] protected float moveSpeed;
    [SerializeField] protected float attackRange = 1f;
    public float AttackRange {  get { return attackRange; } }


    protected bool isDead = false;
    public bool IsDead {  get { return isDead; } }

    public virtual void Start()
    {
        hp = maxHp;
    }

    public abstract bool TakeDamage(float damage);
    
    
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
    public virtual void Dead()
    {
        isDead = true;
        hp = 0;
        Debug.Log($"{characterName} is dead!");
        // to-do add effect
    }
}
