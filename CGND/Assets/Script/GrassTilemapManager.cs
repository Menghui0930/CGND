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

        GameObject selectedGrass = grassPrefabs[Random.Range(0, grassPrefabs.Length)];
        GameObject grass = Instantiate(selectedGrass, worldPos,Quaternion.identity);
    }
}
