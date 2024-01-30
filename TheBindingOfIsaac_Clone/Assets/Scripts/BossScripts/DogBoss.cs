using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class DogBoss : MonoBehaviour
{

    private enum BossState
    {
        Idle,
        Walk,
        Attack01,
        Defence,
        Rage,
        Die,
        Stanning
    }

    [SerializeField] private int Health;
    [SerializeField] private int currentHealth;
    [SerializeField] private float speed;
    [SerializeField] private float rageSpeed;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float timeStanning;
    [SerializeField] private Animator animator;
    [SerializeField]private bool isDie, isRage,isStanning, isAttack;
    [SerializeField] private Transform player;
    [SerializeField] private float attackRange;
    [SerializeField] private float rageCooldownTime;
    [SerializeField] private GameObject rageAttackCollider;

    [SerializeField]private BossState currentState = BossState.Idle;
    private bool isRageCooldown = false;

    private void Start()
    {
        currentHealth = Health;
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        player = PlayerController.instance.transform;
        StartCoroutine(RandomStateTransition());
        UIController.instance.VisualBossHealth(!isDie);
        UIController.instance.bossHealthSlider.maxValue = Health;
        UIController.instance.bossHealthSlider.value = currentHealth;
        UIController.instance.bossNameText.text = gameObject.name;
    }

    private void Update()
    {
        Die();
        if (!isDie && !isStanning)
        {
            HandleState();
        }
        RotateTowardsPlayer();
    }

    private void FixedUpdate()
    {
        if (currentState == BossState.Walk && !isStanning)
            MoveTowardsPlayer();
        else if (currentState == BossState.Rage)
            MoveTowardsPlayerWithRage();
    }

    private void Die()
    {
        if (currentHealth <= 0)
        {
            isDie = true;
            currentState = BossState.Die;
            animator.SetBool("isDie", true);
            UIController.instance.VisualBossHealth(!isDie);
            Destroy(gameObject, 1f);
        }
    }

    private void HandleState()
    {
        switch (currentState)
        {
            case BossState.Idle:
                Debug.Log($"Distance : {Vector3.Distance(transform.position, player.position)}");
                if (Vector3.Distance(transform.position, player.position) > attackRange)
                {
                    currentState = BossState.Walk;
                }
                break;
            case BossState.Walk:
                if (Vector3.Distance(transform.position, player.position) <= attackRange)
                {
                    if (currentHealth <= Health/2)
                    {
                        RandomStateTransition();
                    }
                    else
                    {
                        currentState = BossState.Attack01;
                    }
                }
                break;
            case BossState.Attack01:
                if (Vector3.Distance(transform.position, player.position) <= attackRange)
                {
                    isAttack = true;
                    animator.SetBool("Attack01Trigger", isAttack);
                }
                else
                {
                    isAttack = false;
                    animator.SetBool("Attack01Trigger", isAttack);
                    currentState = BossState.Walk;
                }
                break;
            case BossState.Rage:
                StartRageAttack();
                break;
            case BossState.Defence:
                StartDefence();
                break;
            case BossState.Stanning:
                StopCoroutine(RageCooldown());
                isStanning = true;
                isRageCooldown = false;
                isRage = false;
                isAttack = false;
                animator.SetBool("isStanning", isStanning);
                StartCoroutine(StunCoroutine());
                break;
        }
    }

    private void MoveTowardsPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        rb.MovePosition(transform.position + direction * speed * Time.fixedDeltaTime);
    }

    private void MoveTowardsPlayerWithRage()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        rb.MovePosition(transform.position + direction * rageSpeed * Time.fixedDeltaTime);
    }

    private void RotateTowardsPlayer()
    {
        Vector3 lookDirection = (player.position - transform.position).normalized;
        Quaternion rotation = Quaternion.LookRotation(new Vector3(lookDirection.x, 0, lookDirection.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 5f);
    }

    private void StartRageAttack()
    {
        if (!isRage && !isRageCooldown)
        {
            isRage = true;
            animator.SetBool("isRage", isRage);
            rageAttackCollider.SetActive(true);
            StartCoroutine(RageCooldown());
            
        }
    }

    private IEnumerator RageCooldown()
    {
        isRageCooldown = true;
        yield return new WaitForSeconds(rageCooldownTime);
        isRageCooldown = false;
        isRage = false;
        animator.SetBool("isRage", isRage);
        rageAttackCollider.SetActive(false);
        currentState = BossState.Walk;
    }

    private void StartDefence()
    {
        animator.SetBool("isDefence", true);
        StartCoroutine(StopDefence());
    }

    private IEnumerator StopDefence()
    {
        yield return new WaitForSeconds(timeStanning);
        animator.SetBool("isDefence", false);
        currentState = BossState.Rage;
    }

    public void Stanning()
    {
        currentState = BossState.Stanning;
    }

    private IEnumerator StunCoroutine()
    {
        yield return new WaitForSeconds(timeStanning);
        isStanning = false;
        animator.SetBool("isStanning", isStanning);
        currentState = BossState.Walk;
    }

    private IEnumerator RandomStateTransition()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(5f, 9f)); // Рандомный интервал между переходами
            if (!isStanning && currentState == BossState.Walk)
            {
                var rand = Random.Range(0, 5);
                currentState = rand < 2 ? BossState.Defence : rand>=2 && rand<3? BossState.Attack01: BossState.Rage;
            }
        }
    }

    public void Damage(int damage)
    {
        currentHealth -= damage;
        UIController.instance.bossHealthSlider.value = currentHealth;
    }
}
