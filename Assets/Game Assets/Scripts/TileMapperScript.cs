using UnityEngine;
using UnityEngine.Tilemaps;

// Tile that displays a Sprite when it is alone and a different Sprite when it is orthogonally adjacent to the same NeighourTile
[CreateAssetMenu]
public class NeighbourTile : TileBase
{
    public Sprite LeftTile;
    public Sprite MidTile;
    public Sprite RightTile;

    public Sprite LoneTile;

    public Sprite GroundBLTile;
    public Sprite GroundBMTile;
    public Sprite GroundBRTile;

    public Sprite GroundMiddleTile;



    public override void RefreshTile(Vector3Int position, ITilemap tilemap)
    {
        for (int yd = -1; yd <= 1; yd++)
        {
            Vector3Int location = new Vector3Int(position.x, position.y + yd, position.z);
           if (IsNeighbour(location, tilemap))
                 tilemap.RefreshTile(location);
        }
        for (int x = -1; x <= 1; x++)
        {
            Vector3Int location = new Vector3Int(position.x + x, position.y, position.z);
            if (IsNeighbour(location, tilemap))
                tilemap.RefreshTile(location);
        }
    }

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        tileData.sprite = LoneTile;
        tileData.colliderType = Tile.ColliderType.Sprite;

        Vector3Int leftSide = new Vector3Int(position.x - 1, position.y, position.z);
        Vector3Int rightSide = new Vector3Int(position.x + 1, position.y, position.z);
        Vector3Int upperSide = new Vector3Int(position.x, position.y + 1, position.z);
        Vector3Int downSide = new Vector3Int(position.x, position.y - 1, position.z);

        if (IsNeighbour(upperSide, tilemap) && IsNeighbour(downSide, tilemap))
        {
            tileData.sprite = GroundMiddleTile;
            return;
        }
        if (IsNeighbour(leftSide, tilemap) && IsNeighbour(rightSide, tilemap) && !IsNeighbour(upperSide, tilemap))
        {
            tileData.sprite = MidTile;
            return;
        }
        else if(IsNeighbour(leftSide, tilemap) && IsNeighbour(rightSide, tilemap) && IsNeighbour(upperSide, tilemap))
        {
            tileData.sprite = GroundBMTile;
            return;
        }
        if (IsNeighbour(leftSide, tilemap) && !IsNeighbour(upperSide, tilemap))
        {
            tileData.sprite = RightTile;
            return;
        }
        else if (IsNeighbour(leftSide, tilemap) && IsNeighbour(upperSide, tilemap))
        {
            tileData.sprite = GroundBRTile;
            return;
        }
        if (IsNeighbour(rightSide, tilemap) && !IsNeighbour(upperSide, tilemap))
        {
            tileData.sprite = LeftTile;
            return;
        }
        else if (IsNeighbour(rightSide, tilemap) && IsNeighbour(upperSide, tilemap))
        {
            tileData.sprite = GroundBLTile;
            return;
        }
    }

    private bool IsNeighbour(Vector3Int position, ITilemap tilemap)
    {
        TileBase tile = tilemap.GetTile(position);
        return (tile != null && tile == this);
    }
}
