using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public float speed = 6f;
    private float damage;
    private Vector2 direction;

    public void Initialize(Vector2 dir, float dmg)
    {
        direction = dir;
        damage = dmg;
        Destroy(gameObject, 5f); // bullet auto destroy
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
            Destroy(gameObject);
        }
        else if (!other.isTrigger && !other.GetComponent<EnemyCharacter>())
        {
            Destroy(gameObject);
        }
    }
}
