using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    [SerializeField] private Rigidbody rb;
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject[] tile;
    [SerializeField] private Transform SpawnerTile;
    [SerializeField] private float spawnOffset = 0.55f;
    [SerializeField] private float maxHealth;
    [SerializeField] private float cycleMaxHealth;
    [SerializeField] private float currentHealth;
    [SerializeField] private Transform bombThrow;
    [SerializeField] private GameObject Wings;

    [SerializeField] private GameObject granadePrefab;
    [SerializeField] private int countGranade = 3;


    [SerializeField] private float jumpForce;
    [SerializeField] private bool isGrounded;
    [SerializeField] private LayerMask groundLayer;

    private Animator WingsAnimator;

    private float speed = 5f;
    private float rotation = 5f;
    private float shootDelay = 0.5f; // Задержка между выстрелами
    private float timeSinceLastShot; private Vector3 velocity;


    [SerializeField] private int countGainMax = 3;
    [SerializeField] private int cycleCountGain;
    [SerializeField]private int countGain;
    private int gainHealth = 100;
    private int gainDamage = 25;
    private float timeGain = 5f;
    private float reloadGain = 12f;
    [SerializeField] private bool canUseGain;
    [SerializeField]private bool isHaveGain;
    [SerializeField]private float countdownGain;
    [SerializeField]private float countdownReloadGain;
    [SerializeField] private int maxCountGranade = 8;

    [SerializeField]private bool isFlying = false;
    [SerializeField] private float maxlevitationSpeed;
    [SerializeField] private float maxLevitationHeight;
    private float levitationSpeed;
    private PlayerProjectTile playerProjectTile;
    private int single_double_tile_damage = 1;

    private void Awake()
    {
        if (!instance)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        cycleMaxHealth = maxHealth;
        cycleCountGain = countGainMax;
        countGain = cycleCountGain;
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        currentHealth = cycleMaxHealth;
        WingsAnimator = Wings.GetComponent<Animator>();
        countdownGain = timeGain;
        countdownReloadGain = reloadGain;
        tile[0].TryGetComponent<PlayerProjectTile>(out playerProjectTile);
        playerProjectTile.ResetLevelDamage();
        isHaveGain = false;

        UIController.instance.healthSlider.maxValue = cycleMaxHealth;
        UIController.instance.healthSlider.value = currentHealth;
        UIController.instance.healthText.text= currentHealth.ToString() + "/" + cycleMaxHealth;

        UIController.instance.gainSlider.maxValue = cycleCountGain;
        UIController.instance.gainSlider.value = countGain;
        UIController.instance.gainText.text = countGain.ToString() + "/" + cycleCountGain;
        UIController.instance.VisualGain(isHaveGain);

        UIController.instance.bombCountText.text = countGranade.ToString();
        UIController.instance.attackText.text = (playerProjectTile.GetAttack()* single_double_tile_damage).ToString();
    }

    private void Update()
    {
        HandleShooting();

        Vector3 inputDir = new Vector3(0f, 0f, 0f);

        if (Input.GetKey(KeyCode.W))
        {
            inputDir.x = -1f;
        }
        if (Input.GetKey(KeyCode.S))
        {
            inputDir.x = 1f;
        }

        if (Input.GetKey(KeyCode.A))
        {
            inputDir.z = -1f;
        }
        if (Input.GetKey(KeyCode.D))
        {
            inputDir.z = 1f;
        }

        if(Input.GetKeyDown(KeyCode.Q))
        {
            if(countGranade>0)
            {
                ThrowGranade();
            }
        }
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            if(isHaveGain && countGain > 0 && canUseGain)
            {
                isFlying = !isFlying;
                activateGain();
                TimeGain();
            }
        }

        inputDir = inputDir.normalized;
        Vector3 targeVelocity = inputDir * speed;

        velocity = Vector3.Lerp(velocity, targeVelocity, 0.1f);
        if (inputDir != Vector3.zero)
        {
            if(!isFlying)
                animator.SetBool("IsMoving",true);
            else
                animator.SetBool("IsMoving", false);
            Quaternion toRotation = Quaternion.LookRotation(inputDir, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, Time.deltaTime * rotation);
        }
        else
        {
            velocity = Vector3.Lerp(velocity,Vector3.zero,0.2f*Time.deltaTime);
            animator.SetBool("IsMoving", false);
        }

        if (isFlying)
        {
            levitationSpeed = maxlevitationSpeed;
            Levitate();
            Wings.SetActive(isFlying);
            WingsAnimator.SetBool("isFly", isFlying);
        }
        else
        {
            levitationSpeed = maxlevitationSpeed;
            Wings.SetActive(isFlying);
            WingsAnimator.SetBool("isFly", isFlying);
        }
        if(!isFlying)
            HandleJump();
        PlayerDie();
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
    }

    private void HandleShooting()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            TryShoot(Vector3.back);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            TryShoot(Vector3.forward);
        }
        else if (Input.GetKey(KeyCode.UpArrow))
        {
            TryShoot(Vector3.left);
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            TryShoot(Vector3.right);
        }
    }

    private void TryShoot(Vector3 direction)
    {
        if (Time.time - timeSinceLastShot > shootDelay)
        {
            Vector3 spawner = SpawnerTile.position + direction * spawnOffset;
            Shoot(spawner, direction);
            timeSinceLastShot = Time.time;
        }
    }

    private void Shoot(Vector3 spawner,Vector3 direction)
    {
        if (playerProjectTile != null && spawner != null)
        {
            Instantiate(playerProjectTile, spawner, Quaternion.LookRotation(direction));
        }
    }


    private void HandleJump()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 0.1f, groundLayer);

        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            animator.SetBool("IsJump", true);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
        }
        animator.SetBool("IsJump", false);
    }

    private void Levitate()
    {
        // Ограничение по высоте
        if (transform.position.y >= maxLevitationHeight)
        {
            rb.velocity = Vector3.zero;
            return;
        }

        rb.AddForce(Vector3.up * levitationSpeed, ForceMode.Acceleration);
    }


    public void GetDamage(int damage)
    {
        animator.SetBool("isHit", true);
        currentHealth -= damage;
        animator.SetBool("isHit", false);
        UIController.instance.healthSlider.value = currentHealth;
        UIController.instance.healthText.text = currentHealth.ToString() + "/" + cycleMaxHealth;
    }

    private void PlayerDie()
    {
        if(currentHealth <= 0)
        {
            instance = null;
            animator.SetBool("isDie", true);
            Destroy(gameObject);
        }
    }

    private void ThrowGranade()
    {
        GameObject granade = Instantiate(granadePrefab, bombThrow.position, bombThrow.rotation);
        countGranade--;
        UIController.instance.bombCountText.text = countGranade.ToString();
    }

    public void Healing(int heal)
    {
        if (currentHealth < cycleMaxHealth)
            currentHealth =Mathf.Min(currentHealth+=heal,cycleMaxHealth);
        UIController.instance.healthSlider.value = currentHealth;
        UIController.instance.healthText.text = currentHealth.ToString() + "/" + cycleMaxHealth;
    }

    public void AddBomb()
    {
        if(countGranade<maxCountGranade)
            countGranade++;
        UIController.instance.bombCountText.text = countGranade.ToString();
    }

    public void AddGain(int heal)
    {
        if(currentHealth< cycleMaxHealth)
            currentHealth = Mathf.Min(currentHealth += heal, cycleMaxHealth);
        if(countGain<cycleCountGain)
            countGain++;

        UIController.instance.healthSlider.value = currentHealth;
        UIController.instance.healthText.text = currentHealth.ToString() + "/" + cycleMaxHealth;

        UIController.instance.gainSlider.value = countGain;
        UIController.instance.gainText.text = countGain.ToString() + "/" + cycleCountGain;
    }

    public void GetGain()
    {
        if (!isHaveGain)
        {
            isHaveGain = true;
            canUseGain = true;
        }
        else
        {
            UpgradeGain();
        }
        UIController.instance.VisualGain(isHaveGain);
    }

    public void UpgradeHealth(float upgradeHealth)
    {
        cycleMaxHealth += upgradeHealth;
        UIController.instance.healthSlider.maxValue = cycleMaxHealth;
        UIController.instance.healthText.text = currentHealth.ToString() + "/" + cycleMaxHealth;
    }

    public void UpgradeGain()
    {
        if(cycleCountGain<5)
            cycleCountGain++;
        UIController.instance.gainSlider.maxValue = cycleCountGain;
        UIController.instance.gainText.text = countGain.ToString() + "/" + cycleCountGain;
    }

    public void UpgradeDamage()
    {
        playerProjectTile.UpdateLevelDamage();
        UIController.instance.attackText.text = (playerProjectTile.GetAttack() * single_double_tile_damage).ToString();
    }

    public void MaxHealing()
    {
        currentHealth = cycleMaxHealth;
        UIController.instance.healthSlider.value = currentHealth;
        UIController.instance.healthText.text = currentHealth.ToString() + "/" + cycleMaxHealth;
    }

    public void changeTile(GameObject newTile)
    {
        int levelDamageNow = playerProjectTile.GetLevelDamage();

        newTile.TryGetComponent<tileItem>(out tileItem itemTile);
        if (newTile.CompareTag("Single"))
        {
            playerProjectTile.GetTileView().TryGetComponent<tileItem>(out tileItem oldTileView); 
            if(itemTile!=null && oldTileView!=null)
            {
                oldTileView.setLevel(levelDamageNow);
                levelDamageNow = itemTile.GetLevel();
            }
            Vector3 spawner = new Vector3((bombThrow.position + spawnOffset * transform.forward).x, spawnOffset, bombThrow.position.z);
            Instantiate(oldTileView, spawner, Quaternion.identity);
            tile[0].TryGetComponent<PlayerProjectTile>(out playerProjectTile);
            single_double_tile_damage = 1;
        }
        if(newTile.CompareTag("Double")) 
        {
            playerProjectTile.GetTileView().TryGetComponent<tileItem>(out tileItem oldTileView);
            if (itemTile != null && oldTileView != null)
            {
                oldTileView.setLevel(levelDamageNow);
                levelDamageNow = itemTile.GetLevel();
            }
            Vector3 spawner = new Vector3((bombThrow.position + spawnOffset * transform.forward).x, spawnOffset, bombThrow.position.z);
            Instantiate(oldTileView, spawner, Quaternion.identity);
            tile[1].TryGetComponent<PlayerProjectTile>(out playerProjectTile);
            single_double_tile_damage = 2;
        }
        playerProjectTile.SetLevelDamage(levelDamageNow);
        UIController.instance.attackText.text = (playerProjectTile.GetAttack() * single_double_tile_damage).ToString() ;
    }

    private void activateGain()
    {
        canUseGain = false;
        countGain--;
        currentHealth += gainHealth;
        playerProjectTile.GainDamage(gainDamage);
        UIController.instance.gainSlider.value = countGain;
        UIController.instance.gainText.text = countGain.ToString() + "/" + cycleCountGain;
        UIController.instance.healthSlider.value = currentHealth;
        UIController.instance.healthText.text = currentHealth.ToString() + "/" + cycleMaxHealth;
        UIController.instance.attackText.text = (playerProjectTile.GetAttack() * single_double_tile_damage).ToString();
    }


    private void TimeGain()
    {
        countdownGain = timeGain; // Сбрасываем таймер при активации усиления
        StartCoroutine(GainTimerCoroutine());
    }
    private IEnumerator GainTimerCoroutine()
    {
        while (countdownGain > 0f)
        {
            countdownGain -= Time.deltaTime;
            yield return null;
        }

        DeactivateGain();
    }

    private void DeactivateGain()
    {
        currentHealth -= gainHealth;
        playerProjectTile.DeactivateGainDamage();
        ReloadGain();
        isFlying = !isFlying;
        UIController.instance.healthSlider.value = currentHealth;
        UIController.instance.healthText.text = currentHealth.ToString() + "/" + cycleMaxHealth;
        UIController.instance.attackText.text = (playerProjectTile.GetAttack() * single_double_tile_damage).ToString();
    }

    private void ReloadGain()
    {
        if (!canUseGain)
        {
            countdownReloadGain = reloadGain; // Сбрасываем таймер при начале перезарядки
            StartCoroutine(ReloadTimerCoroutine());
        }
    }

    private IEnumerator ReloadTimerCoroutine()
    {
        while (countdownReloadGain > 0f)
        {
            countdownReloadGain -= Time.deltaTime;
            yield return null;
        }
        canUseGain = true;
    }

    private void OnDestroy()
    {
        DontDestroyOnLoad(this);
    }
}
