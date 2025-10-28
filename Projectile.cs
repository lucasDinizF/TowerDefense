using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Transform target;
    public float speed = 5f;
    public float damage = 10f;
    public float lifetime = 3f;

    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Seek(Transform _target)
    {
        target = _target;
    }

    void Start()
    {
        Destroy(gameObject, lifetime);

        if (spriteRenderer != null)
            StartCoroutine(AnimateProjectile());
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 direction = target.position - transform.position;
        float distanceThisFrame = speed * Time.deltaTime;

        if (direction.magnitude <= distanceThisFrame)
        {
            HitTarget();
            return;
        }

        transform.Translate(direction.normalized * distanceThisFrame, Space.World);

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void HitTarget()
    {
        if (target != null)
        {
            Enemy e = target.GetComponent<Enemy>();
            if (e != null)
                e.TakeDamage(damage);
        }

        Destroy(gameObject);
    }

    System.Collections.IEnumerator AnimateProjectile()
    {
        Vector3 originalScale = transform.localScale;
        while (true)
        {
            transform.localScale = originalScale * 1.2f;
            spriteRenderer.color = Color.yellow;
            yield return new WaitForSeconds(0.1f);

            transform.localScale = originalScale;
            spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(0.1f);
        }
    }

}
