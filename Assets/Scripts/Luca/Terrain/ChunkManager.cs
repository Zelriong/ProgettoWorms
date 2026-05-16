using System.Collections.Generic;
using UnityEngine;

//optimizing for collider formation
public class ChunkManager : MonoBehaviour
{
    //stores all references to collider chunks created
    private List<TilemapColliderGenerator> m_chunks = new();

    //adds references to chunks list
    public void AddChunk(TilemapColliderGenerator chunk)
    {
        m_chunks.Add(chunk);
    }
    
    //creates a method to update the colliders of the chunks within a certain radius of the explosion
    public List<TilemapColliderGenerator> GetClosestChunks(Vector2 worldPosition)
    {
        //list to store chunks and their distances from explosion
        List<(float distance, TilemapColliderGenerator chunk)> closestChunks = new();
        //loops through collider chunk references
        foreach (TilemapColliderGenerator chunk in m_chunks)
        {
            //gets the distance between tilemap center and explosion position
            float distance = Vector3.Distance(chunk.Center, worldPosition);
            closestChunks.Add((distance, chunk));
        }
        
        //sorts through chunks to have the closest distances first, which allows the colliders to be updated in order of proximity to explosion
        closestChunks.Sort((a,b) => a.distance.CompareTo(b.distance));
        
        //creates a list of chunks to remove on explosion
        List<TilemapColliderGenerator> closestChunksToRemove = new();
        //gets the 4 closest chunks to explosion, which is the maximum number of chunks that can be affected by an explosion at once, and adds them to the list of chunks to remove
        int count = Mathf.Min(4, closestChunks.Count);
        //gets the closest chunks from the closest chunks list and adds it to the list of chunks to remove
        for (int i = 0; i < count; i++)
        {
            closestChunksToRemove.Add(closestChunks[i].chunk);
        }
        return closestChunksToRemove;
    }

    public void DrawGizmos(Vector3 chunkSize, Vector3 halfSize)
    {
        if (m_chunks.Count > 0)
        {
            foreach (TilemapColliderGenerator chunk in m_chunks)
            {
                Gizmos.DrawWireCube(chunk.transform.position + halfSize, chunkSize);
                Gizmos.DrawSphere(chunk.transform.position, 0.1f);
            }
        }
    }
}
