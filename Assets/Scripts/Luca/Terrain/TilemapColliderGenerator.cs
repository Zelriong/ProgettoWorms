using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapColliderGenerator : MonoBehaviour
{
    [SerializeField] private Tilemap m_tilemap;                 //tilemap reference used to create the colliders
    //[SerializeField] TileScriptableObject tileScriptable;       //gets scriptable object for tile component
    private Tile m_tile;                                        //tile reference used to set the collider type for the tilemap
    public Vector3 Center { get; private set; }

    private void Awake()
    {
        m_tile = ScriptableObject.CreateInstance<Tile>();
        //m_tile = tileScriptable.tile;                       //gets the tile from the scriptable object and assigns it to the tile reference
        m_tile.colliderType = Tile.ColliderType.Grid;       //sets the tile collider type to grid, which creates a collider for each tile in the tilemap
    }

    //array of arrays represents if there is a pixel or not where the tilemap collider generator object is placed
    public void PrepareCollider(bool[][] pixelState)
    {
        //runs through every element within the first array
        for (int y = 0; y < pixelState.Length; y++)
        {
            //runs through the inner arrays nested inside the outer arrays
            for (int x = 0; x < pixelState[y].Length; x++)
            {
                //this method prepares the collider, if pixelState returns true, sets the tile, else sets it to null
                m_tilemap.SetTile(new Vector3Int(x, y, 0), pixelState[y][x] ? m_tile : null);
            }
        }
        //gets half of the height and width of tile
        Vector3Int centerTile = new(pixelState[0].Length / 2, pixelState.Length / 2);
        //converts the cell position to world position
        Center = m_tilemap.CellToWorld(centerTile);
    }
    
    //method to destroy tiles within the tilemap
    public void DestroyCollider(Vector2 originWorldSpace, List<Vector2Int> affectedTilesAsOffset)
    {
        //converts origin to tile space
        Vector3Int originCell = m_tilemap.WorldToCell(originWorldSpace);

        //removes the values of each affected tile
        foreach (Vector2Int cell in affectedTilesAsOffset)
        {
            //calculates the position of the affected tile
            Vector3Int tilePosition = originCell + (Vector3Int)cell;

            //if tilemap has a tile at the specified position
            if (m_tilemap.HasTile(tilePosition))
            {
                //removes the tile at the specified position, which also removes the collider for that tile
                m_tilemap.SetTile(tilePosition, null);
            }
        }
    }
}
