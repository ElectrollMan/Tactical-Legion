using UnityEngine;
using System.Collections;

public class GroundController : MonoBehaviour
{
    private SpriteRenderer sr;
    private float widthWorld, heightWorld;
    private int widthPixel, heightPixel;
    private Color transp;

    [HideInInspector] public Texture2D customTerrainTexture;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr == null)
        {
            Debug.LogError("❌ GroundController: Missing SpriteRenderer!");
            return;
        }

        // Load default or assigned custom texture
        if (customTerrainTexture != null)
        {
            ApplyCustomTexture(customTerrainTexture);
        }
        else
        {
            Texture2D tex = Resources.Load<Texture2D>("Art/Map/MapA/BattleScene_Junkyard_DestructableTerrain");
            if (tex != null)
            {
                ApplyCustomTexture(tex);
            }
            else
            {
                Debug.LogError("❌ Default Junkyard texture missing!");
            }
        }

        transp = new Color(1f, 1f, 1f, 0f);
        InitSpriteDimensions();
        BulletController.groundController = this;
    }

    public void ApplyCustomTexture(Texture2D customTexture)
    {
        if (customTexture == null)
        {
            Debug.LogError("❌ GroundController.ApplyCustomTexture called with null texture!");
            return;
        }

        if (sr == null)
        {
            sr = GetComponent<SpriteRenderer>();
            if (sr == null)
            {
                Debug.LogError("❌ GroundController: Missing SpriteRenderer during ApplyCustomTexture!");
                return;
            }
        }

        // Make a readable clone of the texture
        Texture2D texClone = Instantiate(customTexture);
        texClone.Apply();

        sr.sprite = Sprite.Create(
            texClone,
            new Rect(0f, 0f, texClone.width, texClone.height),
            new Vector2(0.5f, 0.5f),
            100f
        );

        InitSpriteDimensions();
        RebuildCollider();
        Debug.Log($"✅ Applied destructible terrain texture: {customTexture.name}");
    }

    private void InitSpriteDimensions()
    {
        if (sr == null || sr.sprite == null) return;

        widthWorld = sr.bounds.size.x;
        heightWorld = sr.bounds.size.y;
        widthPixel = sr.sprite.texture.width;
        heightPixel = sr.sprite.texture.height;
    }

    public void DestroyGround(CircleCollider2D cc)
    {
        if (sr == null || sr.sprite == null || sr.sprite.texture == null) return;

        Texture2D tex = sr.sprite.texture;

        Vector2Int c = World2Pixel(cc.bounds.center.x, cc.bounds.center.y);
        int r = Mathf.RoundToInt(cc.bounds.size.x * widthPixel / widthWorld / Mathf.RoundToInt(transform.localScale.x));

        for (int x = 0; x <= r; x++)
        {
            int d = Mathf.RoundToInt(Mathf.Sqrt(r * r - x * x));
            for (int y = 0; y <= d; y++)
            {
                int px = c.x + x;
                int nx = c.x - x;
                int py = c.y + y;
                int ny = c.y - y;

                if (px >= 0 && px < widthPixel && py >= 0 && py < heightPixel)
                    tex.SetPixel(px, py, transp);
                if (nx >= 0 && nx < widthPixel && py >= 0 && py < heightPixel)
                    tex.SetPixel(nx, py, transp);
                if (px >= 0 && px < widthPixel && ny >= 0 && ny < heightPixel)
                    tex.SetPixel(px, ny, transp);
                if (nx >= 0 && nx < widthPixel && ny >= 0 && ny < heightPixel)
                    tex.SetPixel(nx, ny, transp);
            }
        }

        tex.Apply();

        // Wait a frame to let Unity finish applying texture changes
        StartCoroutine(DelayedColliderRebuild());
    }

    private IEnumerator DelayedColliderRebuild()
    {
        yield return new WaitForEndOfFrame(); // ensure texture updates are applied
        RebuildCollider();
    }

    private void RebuildCollider()
    {
        PolygonCollider2D poly = GetComponent<PolygonCollider2D>();
        if (poly != null)
            DestroyImmediate(poly);

        gameObject.AddComponent<PolygonCollider2D>();
        Debug.Log("Terrain collider rebuilt (destruction or map reload)");
    }

    private Vector2Int World2Pixel(float x, float y)
    {
        Vector2Int v = new Vector2Int();
        float dx = x - transform.position.x;
        v.x = Mathf.RoundToInt(0.5f * widthPixel + dx * widthPixel / widthWorld);
        float dy = y - transform.position.y;
        v.y = Mathf.RoundToInt(0.5f * heightPixel + dy * heightPixel / heightWorld);
        return v;
    }
}
