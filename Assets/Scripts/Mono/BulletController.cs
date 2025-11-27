using UnityEngine;
using System.Collections;
using Cinemachine;
using TurnBaseUtil;

public class BulletController : MonoBehaviour
{
    private Rigidbody2D rb;
    public Transform bulletSpriteTransform;
    public Transform PlayerTransform { get; set; }
    private bool updateAngle = true;

    public GameObject bulletSmoke;
    public CircleCollider2D destructionCircle;
    public static GroundController groundController;

    private SpriteRenderer sprite;
    private int currentPlayerCount;

    public GameObject explosionEffectPrefab;
    private bool isDestroyed;
    private GameObject explosionEffectObject;

    private AudioSource SFX;
    public AudioClip boomSFX;

    void Start()
    {
        currentPlayerCount = GameManager.Instance.TurnBaseController.GetAllTeamPlayerCount();
        GameManager.Instance.vCam.Follow = transform;
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        SFX = GetComponent<AudioSource>();
        rb.AddForce(GameManager.Instance.TurnBaseController.TurnProperties.WindForce);
    }

    void Update()
    {
        if (updateAngle)
        {
            Vector2 dir = rb.velocity;
            dir.Normalize();
            float angle = Mathf.Asin(dir.y) * Mathf.Rad2Deg;
            if (dir.x < 0f)
                angle = 180 - angle;
            bulletSpriteTransform.localEulerAngles = new Vector3(0f, 0f, angle + 45f);
        }

        if (transform.position.y < -9f || transform.position.x < -18f || transform.position.x > 18f)
        {
            if (!isDestroyed)
                DestroySelf(false, 0.5f);
        }
    }

    private void DestroySelf(bool isDelay, float duration = 2f)
    {
        isDestroyed = true;
        if (isDelay)
        {
            sprite.enabled = false;
            rb.velocity = Vector2.zero;
            rb.isKinematic = true;
        }
        StartCoroutine(GameManager.Instance.DelayFuc(() =>
        {
            Destroy(gameObject);
            // Check if this is the last active bullet
            if (GameObject.FindObjectsOfType<BulletController>().Length <= 1)
            {
                // Only end the turn once all bullets are gone
                GameManager.Instance.TurnBaseController.EndTurn();
                GameManager.Instance.TurnBaseController.StartTurn();
            }

        }, duration));
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        // Handle collision with Ground
        if (coll.collider.CompareTag("Ground") && !isDestroyed)
        {
            TriggerExplosion(coll.contacts[0].point);
        }

        // Handle collision with BombBracket
        else if (coll.collider.CompareTag("BombBracket") && !isDestroyed)
        {
            TriggerExplosion(coll.contacts[0].point);
        }

        // Handle collision with Player
        else if (coll.collider.CompareTag("Player") && !isDestroyed)
        {
            TriggerExplosion(coll.contacts[0].point);
        }
    }

    void TriggerExplosion(Vector2 hitPoint)
    {
        updateAngle = false;
        bulletSmoke.SetActive(false);

        if (groundController != null && destructionCircle != null)
            groundController.DestroyGround(destructionCircle);

        explosionEffectObject = Instantiate(explosionEffectPrefab, hitPoint, Quaternion.identity);

        float r = destructionCircle != null ? destructionCircle.radius : 1f;
        explosionEffectObject.transform.localScale = Vector3.one * Mathf.Clamp(r * 2f, 1f, 5f);

        Invoke(nameof(RemoveEffectTrigger), 0.2f);
        DestroySelf(true);
        SFX.PlayOneShot(boomSFX);
    }

    void RemoveEffectTrigger()
    {
        if (explosionEffectObject != null)
            explosionEffectObject.GetComponent<CircleCollider2D>().enabled = false;
    }
}
