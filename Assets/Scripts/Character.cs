using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Character : MonoBehaviour
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
    public float attackRange = 1f;


    protected bool isDead = false;
    public bool IsDead {  get { return isDead; } }

    public virtual void Start()
    {
        hp = maxHp;
    }

    public virtual bool TakeDamage(float damage)
    {
        if(isDead) return true;

        hp -= damage;
        Debug.Log($"{characterName} got {damage} damage. Now {hp} / {maxHp} hp.");

        if (hp <= 0)
        {
            Dead();
            return true;
        }

        return false;
    }
    
    
    public void PerformAttack(Character target)
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
        //anim attack


    }
    public virtual void Dead()
    {
        isDead = true;
        hp = 0;
        Debug.Log($"{characterName} is dead!");
        // to-do add effect
    }
}
