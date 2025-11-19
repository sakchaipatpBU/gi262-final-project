using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class EnemyProjectile : MonoBehaviour
{
    [SerializeField] private float speed = 6f;
    [SerializeField] private float damage;
    [SerializeField] private Vector2 direction;

    public void Initialize(Vector2 dir, float dmg)
    {
        direction = dir;
        damage = dmg;
        Destroy(gameObject, 5f); // auto destroy
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerCharacter player = other.GetComponent<PlayerCharacter>();
        if (player != null)
        {
            player.TakeDamage(damage);
            player.gameObject.GetComponent<PlayerController>().SetHitDirection(transform.position);
            Destroy(gameObject);
        }
        else if (!other.isTrigger && !other.GetComponent<EnemyCharacter>())
        {
            Destroy(gameObject);
        }
    }
}
