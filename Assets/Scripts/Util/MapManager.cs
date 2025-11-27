using UnityEngine;
using System.Collections;

public class MapManager : MonoBehaviour
{
    [Header("Scene References")]
    public SpriteRenderer backgroundRenderer;
    public SpriteRenderer terrainRenderer;

    private GroundController ground;

    private void Awake()
    {
        ground = FindObjectOfType<GroundController>();
        if (ground == null)
            Debug.LogError("❌ MapManager: GroundController not found in scene!");
    }

    private void Start()
    {
        StartCoroutine(ApplyMapDelayed());
    }

    private IEnumerator ApplyMapDelayed()
    {
        yield return null; // wait one frame to ensure GroundController initializes

        string selectedMap = !string.IsNullOrEmpty(Global.SelectedMap) ? Global.SelectedMap : "Junkyard";
        Debug.Log($"🌍 Loading map: {selectedMap}");
        SetMap(selectedMap);
    }

    public void SetMap(string mapName)
    {
        string mapPath = $"Art/Map/MapA/BattleScene_{mapName}";

        // === BACKGROUND ===
        string bgPath = $"{mapPath}_Background";
        Sprite bgSprite = Resources.Load<Sprite>(bgPath);
        if (bgSprite != null && backgroundRenderer != null)
        {
            backgroundRenderer.sprite = bgSprite;
            Debug.Log($"✅ Background loaded: {bgPath}");
        }
        else
        {
            Debug.LogWarning($"⚠️ Background not found at: Resources/{bgPath}");
        }

        // === DESTRUCTIBLE TERRAIN ===
        string terrainPath = $"{mapPath}_DestructableTerrain";
        Texture2D terrainTexture = Resources.Load<Texture2D>(terrainPath);
        if (terrainTexture != null)
        {
            if (ground != null)
            {
                ground.ApplyCustomTexture(terrainTexture);
                BulletController.groundController = ground;

                // Force collider rebuild on new map load
                ground.SendMessage("RebuildCollider", SendMessageOptions.DontRequireReceiver);

                Debug.Log($"✅ Loaded destructible terrain from: Resources/{terrainPath}");
            }
            else
            {
                Debug.LogWarning("⚠️ GroundController not found in scene (terrain skipped)");
            }
        }
        else
        {
            Debug.LogError($"❌ Could not find destructible terrain at Resources/{terrainPath}");
        }

        // === OPTIONAL STATIC TERRAIN OVERLAY ===
        string staticTerrainPath = $"{mapPath}_Terrain";
        Sprite terrainSprite = Resources.Load<Sprite>(staticTerrainPath);
        if (terrainSprite != null && terrainRenderer != null)
        {
            terrainRenderer.sprite = terrainSprite;
            Debug.Log($"✅ Static terrain overlay loaded: {staticTerrainPath}");
        }
    }
}
