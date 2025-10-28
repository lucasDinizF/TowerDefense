using UnityEngine;

public class TurretScript : MonoBehaviour
{
    [Header("Configurações")]
    public float range = 5f;
    public float fireRate = 1f;
    public float rotationSpeed = 200f;

    [Header("Referências")]
    public GameObject projectilePrefab;
    public Transform firePoint;

    private float fireCountdown = 0f;
    private Transform target;

    void Awake()
    {

        if (firePoint == null)
        {
            GameObject fp = new GameObject("FirePoint");
            fp.transform.parent = transform;
            fp.transform.localPosition = new Vector3(1f, 0f, 0f);
            fp.transform.localRotation = Quaternion.identity;
            firePoint = fp.transform;
            Debug.Log("✅ FirePoint criado automaticamente!");
        }


        if (projectilePrefab == null)
        {
            projectilePrefab = Resources.Load<GameObject>("Projectile");
            if (projectilePrefab != null)
                Debug.Log("✅ ProjectilePrefab carregado automaticamente via Resources!");
            else
                Debug.LogWarning("⚠ Não encontrou o prefab Projectile na pasta Resources!");
        }
    }

    void Update()
    {
        UpdateTarget();

        if (target == null)
            return;

        RotateTowardsTarget();

        if (fireCountdown <= 0f)
        {
            Shoot();
            fireCountdown = 1f / fireRate;
        }

        fireCountdown -= Time.deltaTime;
    }

    void UpdateTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;

        foreach (GameObject enemy in enemies)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy < shortestDistance)
            {
                shortestDistance = distanceToEnemy;
                nearestEnemy = enemy;
            }
        }

        if (nearestEnemy != null && shortestDistance <= range)
            target = nearestEnemy.transform;
        else
            target = null;
    }

    void RotateTowardsTarget()
    {
        if (target == null) return;

        Vector3 dir = target.position - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
    }

    void Shoot()
    {
        if (projectilePrefab == null || firePoint == null)
        {
            Debug.LogWarning("⚠ Falta atribuir ProjectilePrefab ou FirePoint!");
            return;
        }

        GameObject projGO = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        Projectile proj = projGO.GetComponent<Projectile>();
        if (proj != null)
            proj.Seek(target);

        Debug.Log("💥 Disparo executado!");
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}

