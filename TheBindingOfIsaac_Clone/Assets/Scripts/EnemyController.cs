using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 2f;
    [SerializeField] private float shootingRange = 10f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private Transform tilePoint;
    [SerializeField] private float fireRate = 1f;
    [SerializeField] private int health;
    [SerializeField] private bool shouldDropItem;
    [SerializeField] private float itemDropPercent;
    [SerializeField] private GameObject[] itemsToDrop;
    [SerializeField] private int chaseDamage;
    [SerializeField] private float runAwayDistance = 5f;
    [SerializeField] private Animator anim;

    [SerializeField] private float levitationSpeed;
    [SerializeField] private float maxLevitationHeight;
    [SerializeField] private bool isFlying;


    [Header("Behavior Flags")]
    [SerializeField] private bool shouldShootPlayer;
    [SerializeField] private bool shouldChasePlayer;
    [SerializeField] private bool shouldRunAway;
    [SerializeField] private bool shouldPatrol;
    private GameObject player;

    private Rigidbody rb;
    private float nextFireTime = 0f;
    private bool isMovingToRandomPoint;
    private Vector3 moveDirection;

    private bool isWalking, isHit, isDie;
    private void Start()

    {
        rb = GetComponent<Rigidbody>();
        player = Restart.instance.GetPlayer();
        shouldDropItem = Random.Range(0f, 1f) < 0.5f;
    }

    private void Update()
    {
        if(player == null)
        {
            return;
        }
        if (Vector3.Distance(transform.position, player.transform.position) < attackRange && shouldChasePlayer)
        {
            AttackPlayer();
        }

        if (shouldPatrol && !isMovingToRandomPoint)
        {
            Patrol();
        }

        if (shouldRunAway && Vector3.Distance(transform.position, player.transform.position) < runAwayDistance)
        {
            RunAway();
        }

        if (shouldShootPlayer && Vector3.Distance(transform.position, player.transform.position) < shootingRange)
        {
            ShootAtPlayer();
        }

        if (isFlying)
        {
            isWalking = true;
            Levitate();
        }

        isEnemyLive();
        UpdateAnimation();
        isWalking = false;
    }

    private void FixedUpdate()
    {
        isWalking = true;
        Move();
    }

    private void Move()
    {
        if ((!isHit || !isDie))
        {
            isWalking = true;
            rb.velocity = new Vector3(moveDirection.x, rb.velocity.y, moveDirection.z);
        }
        else
        {
            isWalking = false;
        }
    }

    private void Levitate()
    {
        //Ограничение по высоте
        if (transform.position.y >= maxLevitationHeight)
        {
            rb.velocity = Vector3.zero; // Остановить подъем, если достигнута максимальная высота
            return;
        }

        // Применение силы вверх
        rb.AddForce(Vector3.up * levitationSpeed, ForceMode.Acceleration);
    }

    void ShootAtPlayer()
    {
        if (Time.time >= nextFireTime)
        {
            Instantiate(tilePrefab, tilePoint.position, tilePoint.rotation);
            nextFireTime = Time.time + 1f / fireRate;
        }
    }

    private void AttackPlayer()
    {
        Vector3 attackDirection = (PlayerController.instance.gameObject.transform.position - transform.position).normalized;
        Quaternion toAttackRotation = Quaternion.LookRotation(attackDirection, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, toAttackRotation, rotationSpeed * Time.deltaTime);

        moveDirection = attackDirection * moveSpeed;
    }

    private void Patrol()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 1f, LayerMask.GetMask("Wall")))
        {
            // Если луч столкнулся со стеной, изменяем направление
            ChangeDirection();
        }
        else
        {

            transform.Translate(transform.forward * moveSpeed * Time.deltaTime, Space.World);
        }
    }

    private void ChangeDirection()
    {
        transform.Rotate(0f, 90f, 0f);
    }

    private void UpdateAnimation()
    {
        anim.SetBool("isWalking", isWalking);
        anim.SetBool("isHit", isHit);
        anim.SetBool("isDie", isDie);

    }

    void RunAway()
    {
        TurnTowardsPlayer();
        moveDirection = (transform.position - PlayerController.instance.gameObject.transform.position).normalized * moveSpeed;
    }

    private void TurnTowardsPlayer()
    {
        Vector3 direction = (PlayerController.instance.gameObject.transform.position - transform.position).normalized;
        Quaternion toRotation = Quaternion.LookRotation(direction, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
    }

    public void GetDamage(int damage)
    {
        isHit = true;
        health -= damage;
        isHit = false;
    }

    private void isEnemyLive()
    {
        if(health<=0)
        {
            isDie = true;
            isWalking = false;
            isHit = false;
            DestroyEnemy();
        }
    }

    public void DestroyEnemy()
    {
        Destroy(gameObject);

        if (shouldDropItem && Random.Range(0f, 100f) < itemDropPercent)
        {
            int randomItem = Random.Range(0, itemsToDrop.Length);
            Instantiate(itemsToDrop[randomItem], transform.position, transform.rotation);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerController>(out PlayerController player))
        {
            player.GetDamage(chaseDamage);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent<PlayerController>(out PlayerController player))
        {
            player.GetDamage(chaseDamage);
        }
    }
}
