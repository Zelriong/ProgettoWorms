using System.Collections.Generic;
using UnityEngine;

public class ExplosionPooler : MonoBehaviour
{
    [SerializeField] private GameObject explosionVFX;

    private List<GameObject> pooledExplosions;

    private void Awake()
    {
        pooledExplosions = new List<GameObject>();
    }

    public GameObject GetExplosion(Vector2 position, Quaternion rotation)
    {
        return SpawnExplosionVFX(explosionVFX, position, rotation);
    }
    
    private GameObject SpawnExplosionVFX(GameObject explosion, Vector2 position, Quaternion rotation)
    {
        for (int i = 0; i < pooledExplosions.Count; i++)
        {
            if (!pooledExplosions[i].activeInHierarchy)
            {
                pooledExplosions[i].transform.position = position;
                pooledExplosions[i].transform.rotation = rotation;
                pooledExplosions[i].SetActive(true);
                return pooledExplosions[i];
            }
        }
        
        //generates an explosion effect at clicked position
        GameObject newExplosion = Instantiate(explosion, position, rotation, gameObject.transform);
        pooledExplosions.Add(newExplosion);
        return newExplosion;
    }
}
