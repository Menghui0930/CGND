using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GrassTilemapManager : MonoBehaviour {
    public static GrassTilemapManager instance;
    public Tilemap tilemap;
    public Tilemap blockTilemap;
    public TileBase grassTile;
    public GameObject[] grassPrefabs;
    [SerializeField] private int spreadWidth = 4;
    [SerializeField] private float spreadDelay = 0.06f;

    [Header("Rare Grass (Skill Unlock)")]
    [SerializeField] private GameObject[] rareGrassPrefabs;   // 拖入3种稀有草
    [SerializeField] private float rareChance = 0.05f;        // 5% 几率
    [HideInInspector] public bool rareGrassUnlocked = false;


    private void Awake() {
        instance = this;
    }

    private void Start() {
        
    }

    public void StartSpread(Vector3 hitPos) {
        StartCoroutine(SpreadGrass(hitPos));
    }

    private IEnumerator SpreadGrass(Vector3 hitPos) {
        Vector3Int centerCell = tilemap.WorldToCell(hitPos);
        TryPlaceGrass(centerCell);

        for (int i = 1; i <= spreadWidth; i++) {
            yield return new WaitForSeconds(spreadDelay);
            TryPlaceGrass(centerCell + new Vector3Int(-i, 0, 0));
            TryPlaceGrass(centerCell + new Vector3Int(i, 0, 0));
        }
    }

    private void TryPlaceGrass(Vector3Int cell) {
        if (blockTilemap.HasTile(cell)) return;

        // if have grass just skip it
        if (tilemap.HasTile(cell)) return;

        tilemap.SetTile(cell, grassTile);

        Vector3 worldPos = tilemap.GetCellCenterWorld(cell);

        // 判断生成普通草还是稀有草
        GameObject selectedGrass;
        if (rareGrassUnlocked && rareGrassPrefabs.Length > 0 && Random.value < rareChance)
            selectedGrass = rareGrassPrefabs[Random.Range(0, rareGrassPrefabs.Length)];
        else
            selectedGrass = grassPrefabs[Random.Range(0, grassPrefabs.Length)];

        GameObject grass = Instantiate(selectedGrass, worldPos, Quaternion.identity);
        grass.GetComponent<GrassObject>().Init(cell);
    }


    //Skill tree
    public void UnlockRareGrass() { rareGrassUnlocked = true; }
    public void LockRareGrass() { rareGrassUnlocked = false; }
}
