using UnityEngine;

public class ModifiableTexture
{
    private Texture2D m_texture;            //stores the texture for the duplicate of the sprite
    private Sprite m_sprite;                //used to generate a sprite from the texture
    //pivot and pixels per unit define the sprite properties
    private Vector2 m_pivot;                
    private float m_pixelsPerUnit;
    //creates a public Texture2D and Sprite that return the relative private variables
    public Texture2D Texture => m_texture;
    public Sprite Sprite => m_sprite;

    public static ModifiableTexture CreateFromSprite(Sprite sprite)
    {
        Rect rect = sprite.rect;        //rect gives a reference to width and height of sprite passed through
        Texture2D texture = new((int)rect.width,        //gets width from sprite rect
            (int)rect.height,                           //gets height from sprite rect
            TextureFormat.RGBA32,                       //defines texture type
            false);                             //does not generate a mipmap
        //defines how the texture is viewed visually
        texture.filterMode = FilterMode.Point;          
        
        //gets all the pixel colors
        Color32[] pixels = sprite.texture.GetPixels32();
        //sets the pixels colors to texture
        texture.SetPixels32(pixels);
        //sends texture data from CPU to GPU
        texture.Apply();

        //normalizes sprite from pixels to values between 0 and 1
        Vector2 normalizedPivot = new(sprite.pivot.x / rect.width, sprite.pivot.y / rect.height);

        return new ModifiableTexture(texture, normalizedPivot, sprite.pixelsPerUnit);
    }

    //creates a constructor that passes through texture properties
    private ModifiableTexture(Texture2D texture, Vector2 pivot, float pixelsPerUnit)
    {
        //and assigns them to the class variables
        this.m_texture = texture;
        this.m_pivot = pivot;
        this.m_pixelsPerUnit = pixelsPerUnit;

        //calls the method to recrate sprite
        RecreateSprite();
    }

    //recreates
    private void RecreateSprite()
    {
        m_sprite = Sprite.Create(m_texture,                         //passes texture to create
            new Rect(0, 0, m_texture.width, m_texture.height),      //defines the sides of texture
            //uses the pivot and pixels per unit from the private variables to create the sprite
            m_pivot,                                                
            m_pixelsPerUnit, 
            //defines any modifications to the sprite
            0, 
            SpriteMeshType.FullRect,
            Vector4.zero, 
            false);         //decides if we create a collisions shape (false = no generation)
    }

    public Vector2Int WorldToTexturePosition(Vector2 worldPosition, Transform transform)
    {
        //converts from world position to local position
        Vector2 localPosition = transform.InverseTransformPoint(worldPosition);
        //calculates the position relative to the texture
        int x = Mathf.FloorToInt(localPosition.x * m_pixelsPerUnit + m_sprite.pivot.x);
        int y = Mathf.FloorToInt(localPosition.y * m_pixelsPerUnit + m_sprite.pivot.y);
        //returns the positions calculated
        return new Vector2Int(x, y);
    }

    public bool IsValidTexturePosition(Vector2Int texturePosition)
    {
        //returns bool based on if the position clicked is inside the texture or not
        return texturePosition.x >= 0 && texturePosition.y >= 0
                                    && texturePosition.x < m_texture.width 
                                    && texturePosition.y < m_texture.height;
    }

    public bool SetPixel(Vector2Int texturePosition, Color color)
    {
        //if bool check of texture position returns false
        if (!IsValidTexturePosition(texturePosition))
            //this too returns false
            return false;
        
        //else modifies the color of pixel at that position
        m_texture.SetPixel(texturePosition.x, texturePosition.y, color);
        //and returns true after successfully modifying pixel value
        return true;
    }

    public void ApplyChanges()
    {
        //sends texture data from CPU to GPU
        m_texture.Apply();
    }
}
