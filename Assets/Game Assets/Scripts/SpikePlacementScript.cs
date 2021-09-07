using UnityEngine;
using UnityEngine.Tilemaps;

// Tile that displays a Sprite when it is alone and a different Sprite when it is orthogonally adjacent to the same NeighourTile
[CreateAssetMenu]
public class SpikePlacementScript : TileBase
{
    public Sprite BasicSpike;


    public override void RefreshTile(Vector3Int position, ITilemap tilemap)
    {
        for (int yd = -1; yd <= 1; yd++)
        {
            Vector3Int location = new Vector3Int(position.x, position.y + yd, position.z);
            if (IsNeighbour(location, tilemap))
                tilemap.RefreshTile(location);
        }
    }

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        tileData.sprite = BasicSpike;
        tileData.colliderType = Tile.ColliderType.Sprite;
        // https://docs.unity3d.com/Manual/Tilemap-ScriptableTiles-Example.html
        var m = tileData.transform;
        m.SetTRS(Vector3.zero, Quaternion.Euler(0f, 0f, 90f), Vector3.one);
        tileData.flags = TileFlags.LockTransform;
        tileData.transform = m;

    }


    private bool IsNeighbour(Vector3Int position, ITilemap tilemap)
    {
        TileBase tile = tilemap.GetTile(position);
        return (tile != null && tile == this);
    }
}
