using System.Collections.Generic;
using UnityEngine;

public class DestructibleTerrain : MonoBehaviour
{
    [SerializeField] private SpriteRenderer m_spriteRenderer;
    private ModifiableTexture m_modifiableTexture;

    [SerializeField] private TilemapColliderGenerator m_colliderGenerator;      //this will be a prefab
    private TilemapColliderGenerator m_nonChunkedCollider;              //assign this component to an instance of collider generator

    [SerializeField] private Grid m_grid;               //reference to grid used for tilemap collider generator
    
    [SerializeField] private ChunkManager m_chunkManager;     //reference to chunk manager that manages the collider chunks
    [SerializeField] private Vector2Int m_chunkSize = new(300, 300);        //maximum size for tilemap chunks
    
    private void Start()
    {
        //sprite renderer null check
        if (m_spriteRenderer == null)
            return;
        //passes serialized sprite in modifiable texture to create a duplicate of the sprite
        m_modifiableTexture = ModifiableTexture.CreateFromSprite(m_spriteRenderer.sprite);
        //sets serialized sprite to sprite generated in modifiable texture
        m_spriteRenderer.sprite = m_modifiableTexture.Sprite;
        
        //this process makes the cell size of grid have the same size as the pixels of the sprite, which allows the tilemap collider generator to create colliders that match the pixels of the sprite
        float pixelSize = 1 / m_modifiableTexture.Sprite.pixelsPerUnit;
        m_grid.cellSize = new Vector2(pixelSize, pixelSize);
        
            ////gets the size of the sprite
            //Vector2 size = m_modifiableTexture.Sprite.bounds.size;
        //calculates the pixel position of the bottom left position of the texture
        Vector2 bottomLeftPixel = -m_modifiableTexture.Sprite.pivot;
        
        //splits colliders in collider chunks
        Vector2Int chunkGridSize = SplitTextureIntoChunks(m_modifiableTexture.Texture.width, m_modifiableTexture.Texture.height, m_chunkSize);

        //needs this method to fill in the tilemaps
        bool[][] pixels = m_modifiableTexture.GetPixelsState();

        //generates the chunk colliders
        PrepareColliderChunks(chunkGridSize, m_chunkSize, bottomLeftPixel, pixels);

        ////converts local position to world position
        // Vector2 bottomLeftWorld = m_spriteRenderer.transform.TransformPoint(bottomLeftLocal);
        // //spawns the collider generator prefab in the bottom left corner of texture and as a child of the grid
        // m_nonChunkedCollider = Instantiate(m_colliderGenerator, bottomLeftWorld, Quaternion.identity, m_grid.transform);
        //
        // m_nonChunkedCollider.PrepareCollider(m_modifiableTexture.GetPixelsState());
    }

    private void PrepareColliderChunks(Vector2Int chunkGridSize, Vector2Int mChunkSize, Vector2 bottomLeftLocal, bool[][] pixels)
    {
        //for loops cycles through each grid generated on a 2D scale
        for (int x = 0; x < chunkGridSize.x; x++)
        {
            for (int y = 0; y < chunkGridSize.y; y++)
            {
                //calculates the grid size in world space
                Vector3Int offset = new Vector3Int(x * mChunkSize.x, y * mChunkSize.y, 0);
                //bottom left corner texture/tilemap space
                Vector3 bottomLeftCorner = (Vector3)bottomLeftLocal + offset;
                //converts to local space
                bottomLeftCorner.Scale(m_grid.cellSize);
                //for safe measures, converts to world space by getting position and rotation of sprite renderer
                bottomLeftCorner = m_spriteRenderer.transform.TransformPoint(bottomLeftCorner);

                //after getting the position of the chunk
                //creates the chunk
                TilemapColliderGenerator colliderGenerator = Instantiate(m_colliderGenerator, bottomLeftCorner,
                    Quaternion.identity, m_grid.transform);
                colliderGenerator.gameObject.name = $"Chunk_{x}_{y}";
                //adds chunk to manager list
                m_chunkManager.AddChunk(colliderGenerator);
                bool[][] chunkPixels = SliceArray(pixels, offset.y, offset.x, mChunkSize.y, mChunkSize.x);
                colliderGenerator.PrepareCollider(chunkPixels);
            }
        }
    }

    private bool[][] SliceArray(bool[][] pixels, int startRow, int startColumn, int numRows, int numColumns)
    {
        //calculates the dimensions of the source array in pixels
        int sourceWidth = pixels.Length;
        int sourceHeight = pixels[0].Length;
        
        //calculates the actual dimensions of the chunk to be sliced, which can be smaller than the specified chunk size if the chunk is at the edge of the source array
        int actualWidth = Mathf.Min(numRows, sourceWidth - startRow);
        int actualHeight = Mathf.Min(numColumns, sourceHeight - startColumn);

        //makes sure the actual dimensions of the chunk are not negative, which can happen if the chunk size exceeds the remaining pixels in the source array
        actualWidth = Mathf.Max(0, actualWidth);
        actualHeight = Mathf.Max(0, actualHeight);

        //initiates the bool array of arrays
        bool[][] result = new bool[actualWidth][];
        //cycles through each row of chunks
        for (int row = 0; row < actualWidth; row++)
        {
            //sets the result array index within row array index to a new bool array of the size of the actual height of the chunk
            //which is determined by the remaining pixels in the source array
            result[row] = new bool [actualHeight];
            //cycles through each column of chunks
            for (int col = 0; col < actualHeight; col++)
            {
                //sets the result of the array index within row array index to the value of the source array at the position of the chunk being sliced
                //which is determined by the starting row and column plus the current row and column being cycled through
                result[row][col] = pixels[startRow + row][startColumn + col];
            }
        }

        return result;
    }

    private Vector2Int SplitTextureIntoChunks(int width, int height, Vector2Int mChunkSize)
    {
        //calculates how many chunks to generate on the right
        int chunkCountRight = Mathf.CeilToInt((float)width / mChunkSize.x);
        //calculates how many chunks to generate up
        int chunkCountUp = Mathf.CeilToInt((float)height / mChunkSize.y);
        return new (chunkCountRight, chunkCountUp);
    }

    public void DestroyTerrainAt(Vector2 worldPosition, float radius)
    {
        //calculates how many pixels should be affect from explosion radius
        float pixelSize = 1 / m_modifiableTexture.Sprite.pixelsPerUnit;
        int radiusInPixels = Mathf.RoundToInt(radius / pixelSize);
        //gets all pixels within explosion radius
        List<Vector2Int> affectedPixelsAsOffset = GetCircleOffset(radiusInPixels);

        //gets center point clicked from mouse input in texture local position
        Vector2Int circleCenterInPixelSpace = 
            m_modifiableTexture.WorldToTexturePosition(worldPosition, m_spriteRenderer.transform);
        
        //changes affected pixels from explosion to clear (Alpha 0)
        ModifyTextureAt(circleCenterInPixelSpace, Color.clear, affectedPixelsAsOffset);

        // //gets the affected pixels and changes their tiles
        // m_nonChunkedCollider.DestroyCollider(worldPosition, affectedPixelsAsOffset);

        //gets the closest chunks to position of explosion
        List<TilemapColliderGenerator> chunksToModify = m_chunkManager.GetClosestChunks(worldPosition);
        foreach (var chunk in chunksToModify)
        {
            //only affects chunks close to the position of the explosion
            chunk.DestroyCollider(worldPosition, affectedPixelsAsOffset);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (Application.isPlaying)
        {
            if (m_chunkManager != null)
            {
                Vector3 chunkSize = new(m_chunkSize.x * m_grid.cellSize.x, m_chunkSize.y * m_grid.cellSize.y);
                Vector3 halfSize = chunkSize / 2;
                m_chunkManager.DrawGizmos(chunkSize, halfSize);
            }
        }
    }

    private void ModifyTextureAt(Vector2Int circleCenterInPixelSpace, Color color, List<Vector2Int> affectedPixelsAsOffset)
    {
        //changes each pixel in list to color passed in function parameter
        foreach (Vector2Int offset in affectedPixelsAsOffset)
        {
            Vector2Int pos = circleCenterInPixelSpace + offset;
            m_modifiableTexture.SetPixel(pos, color);
        }
        //applies changes to texture and updates sprite
        m_modifiableTexture.ApplyChanges();
    }

    private List<Vector2Int> GetCircleOffset(int radiusInPixels)
    {
        //creates affected pixel list
        List<Vector2Int> affectedPixelsAsOffset = new List<Vector2Int>();
        
        //for loop to go through all pixels within circle as x values
        for (int x = -radiusInPixels; x <= radiusInPixels; x++)
        {
            //within the for loop, also runs though y values
            for (int y = -radiusInPixels; y <= radiusInPixels; y++)
            {
                //if pixel values from both for loops resides inside explosion radius
                if (x * x + y * y <= radiusInPixels * radiusInPixels)
                {
                    //adds said pixel to affected pixels list
                    affectedPixelsAsOffset.Add(new Vector2Int(x, y));
                }
            }
        }
        //at the end of the for loops, returns all affected pixels in list
        return affectedPixelsAsOffset;
    }
}
