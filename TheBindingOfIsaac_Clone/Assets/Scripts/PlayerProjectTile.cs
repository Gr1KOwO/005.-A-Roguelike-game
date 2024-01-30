using System.Collections;
using UnityEngine;

public class PlayerProjectTile : MonoBehaviour
{
    [SerializeField] private Rigidbody rb = null;
    [SerializeField] private int damageToDeal = 20;
    [SerializeField] private float destroyAfterSeconds;
    [SerializeField] private float launchForce = 10f;
    [SerializeField] private int startdamageToDeal;
    [SerializeField] private int levelDamage=1;
    [SerializeField] private GameObject tileView;

    private void Start()
    {
        rb.velocity = transform.forward * launchForce;
    }

    private void Update()
    {
        StartCoroutine(DestroyAfterDelay());
    }

    private IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(destroyAfterSeconds);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<EnemyController>(out EnemyController enemy))
        {
            enemy.GetDamage(damageToDeal * levelDamage);
            DestroySelf();
        }

        if(other.TryGetComponent<DogBoss>(out DogBoss dogBoss))
        {
            dogBoss.Damage(damageToDeal * levelDamage);
            DestroySelf();
        }

        DestroySelf();
    }

    private void DestroySelf()
    {
        Destroy(gameObject);
    }

    public void GainDamage(int damage)
    {
        damageToDeal += damage;

    }

    public void DeactivateGainDamage()
    {
        damageToDeal = startdamageToDeal;
    }

    public void UpdateLevelDamage()
    {
        if(levelDamage<3)
        {
            levelDamage++;
        }
    }

    public void ResetLevelDamage()
    {
        levelDamage = 1;
    }

    public int GetLevelDamage()
    {
        return levelDamage;
    }

    public void SetLevelDamage(int levelDamage)
    {
        if (levelDamage <= 3)
        {
            this.levelDamage = levelDamage;
        }
    }

    public GameObject GetTileView()
    {
        return tileView;
    }

    public int GetAttack()
    {
        return levelDamage * damageToDeal;
    }
}
