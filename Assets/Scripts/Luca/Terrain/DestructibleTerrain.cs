using System.Collections.Generic;
using UnityEngine;

public class DestructibleTerrain : MonoBehaviour
{
    [SerializeField] private SpriteRenderer m_spriteRenderer;
    private ModifiableTexture m_modifiableTexture;

    private void Start()
    {
        //sprite renderer null check
        if (m_spriteRenderer == null)
            return;
        //passes serialized sprite in modifiable texture to create a duplicate of the sprite
        m_modifiableTexture = ModifiableTexture.CreateFromSprite(m_spriteRenderer.sprite);
        //sets serialized sprite to sprite generated in modifiable texture
        m_spriteRenderer.sprite = m_modifiableTexture.Sprite;
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
