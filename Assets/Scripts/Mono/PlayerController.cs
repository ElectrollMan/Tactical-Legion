using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using TurnBaseUtil;
using UnityEngine;
using static Global;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 1.2f;
    public float jumpForce = 200;
    public Transform headCheckPoint;
    public Transform groundCheckPoint;
    public LayerMask groundCheckLayer;
    private Animator anim;
    private Rigidbody2D rb;
    private float gravityScale;
    private SpriteRenderer sprite;
    private float horizontal_move;
    private bool isJumpButtonDown;
    private bool isGround;
    private bool isHead;
    private bool isJump;
    public float bulletMaxInitialVelocity;
    public float maxTimeShooting;
    public PolygonCollider2D groundBC;
    public GameObject bulletPrefab;
    public GameObject bigBulletPrefab;
    public GameObject multiBulletPrefab;
    public GameObject smallRocketPrefab;

    [Header("Multi-Bazooka Settings")]
    public int multiShotCount = 3;
    public float multiSpreadAngle = 10f;

    private bool shooting;
    private float timeShooting;
    private Vector2 shootDirection;
    public GameObject shootingEffect;
    private Transform weaponTransform;
    private Transform bodyTransform;
    public Transform bulletInitialTransform;
    private bool targetting;
    private bool isFirstInit = true;
    private bool canControl = false;
    private float mouseScrollWheel;
    public float size = 0.95f;
    private PlayerUI ui;
    private TeamPlayer player;
    public float boomForceValue = 150;
    private AudioSource SFX;
    public AudioClip jumpSFX;
    public AudioClip dieSFX;
    public AudioClip chargeSFX;
    public AudioClip shootSFX;
    public bool[] useWeapon = new bool[16];
    public bool IsDead { get; set; }

    private Weapon currentWeapon = Weapon.BAZOOKA;

    public int bazookaAmmo = 5;
    public int bigBazookaAmmo = 3;
    public int multiBazookaAmmo = 3;
    public int smallRocketAmmo = 6;

    private void OnEnable()
    {
        canControl = true;
        Debug.Log(gameObject.name + "OnEnable");
        if (isFirstInit)
        {
            canControl = false;
            isFirstInit = false;
            return;
        }
        GameManager.Instance.vCam.m_Lens.OrthographicSize = 5;
        GameManager.Instance.vCam.Follow = transform;
        ui.SetArrowActive(true);
        UIManager.Instance.SetEndTurnButtonActive(true);
    }

    private void OnDisable()
    {
        canControl = false;
        rb.gravityScale = 1;
        Debug.Log(gameObject.name + "OnDisable");
        targetting = false;
        ui.SetArrowActive(false);
    }

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        ui = GetComponent<PlayerUI>();
        player = GetComponent<TeamPlayer>();
        SFX = GetComponent<AudioSource>();
        weaponTransform = transform.Find("Bazooka").gameObject.transform;
        bodyTransform = transform;
        gravityScale = rb.gravityScale;
    }

    void UpdateCamera()
    {
        mouseScrollWheel = Input.GetAxis("Mouse ScrollWheel");

        if (mouseScrollWheel < 0)
        {
            GameManager.Instance.vCam.m_Lens.OrthographicSize /= size;
        }
        else if (mouseScrollWheel > 0)
        {
            GameManager.Instance.vCam.m_Lens.OrthographicSize *= size;
        }

        GameManager.Instance.vCam.m_Lens.OrthographicSize =
            Mathf.Clamp(GameManager.Instance.vCam.m_Lens.OrthographicSize, 5f, 7.933367f);
    }

    void UpdateShootDetection()
    {
        if (Input.GetMouseButtonDown(0))
        {
            shooting = true;
            shootingEffect.SetActive(true);
            timeShooting = 0f;
            SFX.PlayOneShot(chargeSFX);
            UIManager.Instance.SetEndTurnButtonActive(false);
        }
    }

    void UpdateShooting()
    {
        timeShooting += Time.deltaTime;
        if (Input.GetMouseButtonUp(0))
        {
            shooting = false;
            shootingEffect.SetActive(false);
            Shoot();
            SFX.Stop();
            SFX.PlayOneShot(shootSFX);
        }
        if (timeShooting > maxTimeShooting)
        {
            shooting = false;
            shootingEffect.SetActive(false);
            Shoot();
            SFX.Stop();
            SFX.PlayOneShot(shootSFX);
        }
        canControl = false;
    }

        void Shoot()
    {
        Vector3 mousePosScreen = Input.mousePosition;
        Vector3 mousePosWorld = Camera.main.ScreenToWorldPoint(mousePosScreen);
        Vector2 playerToMouse = new Vector2(mousePosWorld.x - transform.position.x,
                                            mousePosWorld.y - transform.position.y);
        playerToMouse.Normalize();
        shootDirection = playerToMouse;
        Debug.Log("Shoot!");

        // Check ammo
        if ((currentWeapon == Weapon.BAZOOKA && bazookaAmmo <= 0) ||
            (currentWeapon == Weapon.BIG_BAZOOKA && bigBazookaAmmo <= 0) ||
            (currentWeapon == Weapon.MULTI_BAZOOKA && multiBazookaAmmo <= 0) ||
            (currentWeapon == Weapon.SMALL_ROCKET && smallRocketAmmo <= 0))
        {
            shootingEffect.SetActive(false);
            return;
        }

        // Choose prefab
        GameObject prefabToUse = bulletPrefab;
        if (currentWeapon == Weapon.BIG_BAZOOKA && bigBulletPrefab != null)
            prefabToUse = bigBulletPrefab;
        else if (currentWeapon == Weapon.MULTI_BAZOOKA && multiBulletPrefab != null)
            prefabToUse = multiBulletPrefab;
        else if (currentWeapon == Weapon.SMALL_ROCKET && smallRocketPrefab != null)
            prefabToUse = smallRocketPrefab;

        if (currentWeapon == Weapon.MULTI_BAZOOKA)
        {
            StartCoroutine(FireMultiBazooka(prefabToUse, shootDirection));
        }
        else
        {
            GameObject bullet = Instantiate(prefabToUse);
            bullet.transform.position = bulletInitialTransform.position + (Vector3)(shootDirection * 0.6f);
            bullet.GetComponent<Rigidbody2D>().velocity =
                shootDirection * bulletMaxInitialVelocity * (timeShooting / maxTimeShooting);
            Collider2D bulletCol = bullet.GetComponent<Collider2D>();
            Collider2D playerCol = GetComponent<Collider2D>();
            if (bulletCol != null && playerCol != null)
                Physics2D.IgnoreCollision(bulletCol, playerCol);
            else
                Debug.LogWarning("Missing collider in Shoot()");

            bullet.GetComponent<BulletController>().PlayerTransform = transform;
        }


        // Reduce ammo
        switch (currentWeapon)
        {
            case Weapon.BAZOOKA:
                bazookaAmmo = Mathf.Max(0, bazookaAmmo - 1);
                break;
            case Weapon.BIG_BAZOOKA:
                bigBazookaAmmo = Mathf.Max(0, bigBazookaAmmo - 1);
                break;
            case Weapon.MULTI_BAZOOKA:
                multiBazookaAmmo = Mathf.Max(0, multiBazookaAmmo - 1);
                break;
            case Weapon.SMALL_ROCKET:
                smallRocketAmmo = Mathf.Max(0, smallRocketAmmo - 1);
                break;
        }

        // Hide weapon after 1s
        StartCoroutine(GameManager.Instance.DelayFuc(() =>
        {
            weaponTransform.gameObject.SetActive(false);
        }, 1f));
    }

    private IEnumerator FireMultiBazooka(GameObject prefabToUse, Vector2 shootDirection)
    {
        float halfSpread = (multiShotCount - 1) * multiSpreadAngle * 0.5f;

        for (int i = 0; i < multiShotCount; i++)
        {
            // Calculate direction
            float offset = -halfSpread + i * multiSpreadAngle;
            Quaternion rot = Quaternion.Euler(0, 0, offset);
            Vector2 dir = rot * shootDirection.normalized;

            // Offset spawn forward so it's outside the player collider
            Vector3 spawnPos = bulletInitialTransform.position + (Vector3)(dir * 0.6f);

            // Instantiate bullet
            GameObject b = Instantiate(prefabToUse, spawnPos, Quaternion.identity);

            // Apply velocity
            Rigidbody2D brb = b.GetComponent<Rigidbody2D>();
            if (brb != null)
            {
                brb.velocity = dir * bulletMaxInitialVelocity * (timeShooting / maxTimeShooting);
            }

            // Safe collision ignore
            Collider2D bulletCol = b.GetComponent<Collider2D>();
            Collider2D playerCol = GetComponent<Collider2D>();
            if (bulletCol != null && playerCol != null)
            {
                Physics2D.IgnoreCollision(bulletCol, playerCol);
            }
            else
            {
                Debug.LogWarning("Missing collider during FireMultiBazooka()");
            }

            // Link bullet to player
            BulletController bc = b.GetComponent<BulletController>();
            if (bc != null) bc.PlayerTransform = transform;

            // Delay before next bullet (staggered)
            yield return new WaitForSeconds(0.3f);
        }

        // Hide the weapon after all shots
        weaponTransform.gameObject.SetActive(false);

        // Wait a moment for explosions or bullet impacts to finish
        yield return new WaitForSeconds(1.0f);

        // End the turn once all bullets are gone
        if (GameObject.FindObjectsOfType<BulletController>().Length <= 0)
        {
            GameManager.Instance.TurnBaseController.EndTurn();
        }
    }


    void UpdateTargetting()
    {
        Vector3 mousePosScreen = Input.mousePosition;
        Vector3 mousePosWorld = Camera.main.ScreenToWorldPoint(mousePosScreen);
        Vector2 playerToMouse = new Vector2(mousePosWorld.x - transform.position.x,
                                            mousePosWorld.y - transform.position.y);
        playerToMouse.Normalize();
        float angle = Mathf.Asin(playerToMouse.y) * Mathf.Rad2Deg;
        if (playerToMouse.x < 0f)
            angle = 180 - angle;

        if (playerToMouse.x > 0f && sprite.flipX)
            sprite.flipX = false;
        else if (playerToMouse.x < 0f && !sprite.flipX)
            sprite.flipX = true;

        weaponTransform.localEulerAngles = new Vector3(0f, 0f, angle);
    }

    private void ProcessInput()
    {
        horizontal_move = Input.GetAxisRaw("Horizontal");
        isJumpButtonDown = Input.GetButtonDown("Jump");
    }

    private void UpdateCheck()
    {
        isGround = Physics2D.OverlapCircle(groundCheckPoint.position, 0.2f, groundCheckLayer);
        isHead = Physics2D.OverlapCircle(headCheckPoint.position, 0.1f, groundCheckLayer);
    }

    private void UpdateMove()
    {
        rb.velocity = new Vector2(moveSpeed * horizontal_move, rb.velocity.y);

        if (horizontal_move > 0)
            sprite.flipX = false;
        else if (horizontal_move < 0)
            sprite.flipX = true;

        anim.SetBool("isWalk", horizontal_move != 0);
    }

    private void UpdateJump()
    {
        if (isGround)
        {
            isJump = false;
            anim.SetBool("isJump", false);
            if (isJumpButtonDown)
            {
                isJump = true;
                rb.AddForce(new Vector2(0, jumpForce));
                isJumpButtonDown = false;
                SFX.PlayOneShot(jumpSFX);
            }
        }
        else
        {
            if (rb.velocity.y > 2f)
                anim.SetBool("isJump", true);
        }
    }

    public void UseBazooka()
    {
        if (bazookaAmmo <= 0) return;
        currentWeapon = Weapon.BAZOOKA;
        targetting = true;
        weaponTransform.gameObject.SetActive(true);
        //UITool.FindUIGameObject("BagPanel").transform.DOLocalMoveX(600, 1f);
        //UIManager.Instance.isOpenedBag = false;
        UIManager.Instance.CloseBagAndLock();
    }

    public void UseBigBazooka()
    {
        if (bigBazookaAmmo <= 0) return;
        currentWeapon = Weapon.BIG_BAZOOKA;
        targetting = true;
        weaponTransform.gameObject.SetActive(true);
        //UITool.FindUIGameObject("BagPanel").transform.DOLocalMoveX(600, 1f);
        //UIManager.Instance.isOpenedBag = false;
        UIManager.Instance.CloseBagAndLock();
    }

    public void UseMultiBazooka()
    {
        if (multiBazookaAmmo <= 0) return;
        currentWeapon = Weapon.MULTI_BAZOOKA;
        targetting = true;
        weaponTransform.gameObject.SetActive(true);
        //UITool.FindUIGameObject("BagPanel").transform.DOLocalMoveX(600, 1f);
        //UIManager.Instance.isOpenedBag = false;
        UIManager.Instance.CloseBagAndLock();
    }

    public void UseSmallRocket()
    {
        if (smallRocketAmmo <= 0) return;
        currentWeapon = Weapon.SMALL_ROCKET;
        targetting = true;
        weaponTransform.gameObject.SetActive(true);
        //UITool.FindUIGameObject("BagPanel").transform.DOLocalMoveX(600, 1f);
        //UIManager.Instance.isOpenedBag = false;
        UIManager.Instance.CloseBagAndLock();
    }

    void Update()
    {
        UpdateCamera();
        if (IsDead)
            return;
        if (canControl)
        {
            ProcessInput();
            UpdateCheck();

            if (useWeapon[(int)Weapon.BAZOOKA])
                UseBazooka();
            else if (useWeapon[(int)Weapon.BIG_BAZOOKA])
                UseBigBazooka();
            else if (useWeapon[(int)Weapon.MULTI_BAZOOKA])
                UseMultiBazooka();
            else if (useWeapon[(int)Weapon.SMALL_ROCKET])
                UseSmallRocket();

            ClearWeaponBoolean();
        }
        else
        {
            anim.SetBool("isWalk", false);
            anim.SetBool("isJump", false);
        }

        if (targetting)
        {
            UpdateTargetting();
            if (canControl)
            {
                UpdateShootDetection();
            }
            if (shooting)
                UpdateShooting();
        }
    }

    void UpdateHurt(int damage)
    {
        anim.SetBool("isHurt", true);
        player.DoHurt(damage);
        StartCoroutine(GameManager.Instance.DelayFuc(() => { anim.SetBool("isHurt", false); }, 1f));
    }

    private void FixedUpdate()
    {
        UpdateMove();
        UpdateJump();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (IsDead)
            return;
        if (collision.gameObject.CompareTag("Ground") && enabled)
        {
            if (!isHead)
                rb.gravityScale = 0;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (IsDead)
            return;
        if (collision.gameObject.CompareTag("Ground") && enabled)
        {
            if (!isHead)
                rb.gravityScale = gravityScale;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsDead)
            return;
        if (collision.gameObject.CompareTag("Explosion"))
        {
            Debug.Log("ExplosionHurt");
            Vector2 vec = transform.position - collision.gameObject.transform.position;
            int damage = (int)(15f / vec.magnitude);
            UpdateHurt(damage);
            player.belongsTo.UpdateHP();
            rb.gravityScale = gravityScale;
            rb.AddForce(vec.normalized * boomForceValue / vec.normalized.magnitude);
        }
        else if (collision.gameObject.CompareTag("BulletCollider"))
        {
            rb.gravityScale = gravityScale;
        }
    }

    public void ClearWeaponBoolean()
    {
        useWeapon = new bool[16];
    }
}
