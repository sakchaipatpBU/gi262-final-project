using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Character : MonoBehaviour
{
    public Rigidbody2D rb;


    [Header("Character Stats")]
    public string characterName;
    [SerializeField] protected float maxHp;
    [SerializeField] protected float hp;
    [SerializeField] protected float atk;
    [SerializeField] protected float moveSpeed;
    public float attackRange = 1f;


    protected bool isDead = false;

    public virtual void Start()
    {
        hp = maxHp;
    }

    public void TakeDamage(float damage)
    {
        if(isDead) return;

        hp -= damage;
        Debug.Log($"{characterName} got {damage} damage. Now {hp} / {maxHp} hp.");

        if (hp <= 0) Dead();
    }
    
    
    public void PerformAttack(Character target)
    {
        if (isDead) return;
        if (target == null) return;

        Debug.Log($"{characterName} attack {target.characterName} with {atk}");
        target.TakeDamage(atk);
    }
    public virtual void Dead()
    {
        isDead = true;
        hp = 0;
        Debug.Log($"{characterName} is dead!");
        // to-do add effect
    }
}
