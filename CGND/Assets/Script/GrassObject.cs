using UnityEngine;

public class GrassObject : MonoBehaviour
{
    private Vector3Int _cell;

    public void Init(Vector3Int cell) {
        _cell = cell;
    }

    private void OnDestroy() {
        if (GrassTilemapManager.instance != null) {
            GrassTilemapManager.instance.tilemap.SetTile(_cell, null);
        }
    }
}
