using UnityEngine;
using UnityEngine.Tilemaps;

// Tile that displays a Sprite when it is alone and a different Sprite when it is orthogonally adjacent to the same NeighourTile
[CreateAssetMenu]
public class LadderPlacementScript : TileBase
{
    public Sprite LadderTop;
    public Sprite LadderMiddle;
    public Sprite LadderBottom;


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
        tileData.sprite = LadderMiddle;
        tileData.colliderType = Tile.ColliderType.Sprite;

        Vector3Int upperSide = new Vector3Int(position.x, position.y + 1, position.z);
        Vector3Int downSide = new Vector3Int(position.x, position.y - 1, position.z);

        if (IsNeighbour(upperSide, tilemap) && IsNeighbour(downSide, tilemap))
        {
            tileData.sprite = LadderMiddle;
            return;
        }
        if (IsNeighbour(upperSide, tilemap) && !IsNeighbour(downSide, tilemap))
        {
            tileData.sprite = LadderBottom;
            return;
        }
        if (IsNeighbour(downSide, tilemap) && !IsNeighbour(upperSide, tilemap))
        {
            tileData.sprite = LadderTop;
            return;
        }
    }

    private bool IsNeighbour(Vector3Int position, ITilemap tilemap)
    {
        TileBase tile = tilemap.GetTile(position);
        return (tile != null && tile == this);
    }
}
