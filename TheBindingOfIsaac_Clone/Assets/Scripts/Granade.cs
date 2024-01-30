using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Granade : MonoBehaviour
{
    public float delay = 2.0f;
    public float radius = 3.0f;
    public float explosionForce = 700.0f;
    private int damage = 100;
    bool hasExploded=false;
    float countdown;

    [SerializeField] private GameObject explosion;

    void Start()
    {
        countdown = delay;
    }

    // Update is called once per frame
    void Update()
    {
        countdown-=Time.deltaTime;
        if(countdown<=0f && !hasExploded)
        {
            Explode();
            hasExploded = true;
        }
    }

    private void Explode()
    {
        Instantiate(explosion,transform.position,transform.rotation);
        Collider [] colliders = Physics.OverlapSphere(transform.position, radius);
        foreach (Collider collider in colliders)
        {
            Rigidbody rb = collider.GetComponent<Rigidbody>();
            if(rb!=null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, radius);
            }

            // Определяем расстояние от центра взрыва до объекта
            float distance = Vector3.Distance(transform.position, collider.transform.position);

            // Вычисляем урон на основе расстояния
            int calculatedDamage = CalculateDamage(distance);

            collider.TryGetComponent<EnemyController>(out EnemyController enemyController);
            collider.TryGetComponent<PlayerController>(out PlayerController playerController);

            collider.TryGetComponent<DogBoss>(out DogBoss dogBoss);
            collider.TryGetComponent<Breakables>(out Breakables breakables);

            if(dogBoss!=null)
            {
                dogBoss.Damage(calculatedDamage);
                dogBoss.Stanning();
            }

            if (enemyController != null)
            {
                enemyController.GetDamage(calculatedDamage);
            }
            if(playerController != null)
            {
                playerController.GetDamage(calculatedDamage);
            }

            if(breakables != null)
            {
                breakables.Smash();
            }
        }
        Destroy(gameObject);
    }
    // Функция для вычисления урона в зависимости от расстояния от взрыва
    private int CalculateDamage(float distance)
    {
        // Расчет урона с учетом радиуса взрыва
        float damagePercentage = Mathf.Clamp01(1f - (distance / radius));
        int calculatedDamage = Mathf.RoundToInt(damage * damagePercentage);
        return calculatedDamage;
    }

}
