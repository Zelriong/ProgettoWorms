using UnityEngine;

[CreateAssetMenu(fileName = "Worm_", menuName = "ScriptableObjects/Worms", order = 0)]
public class Player : ScriptableObject

{
    [SerializeField] string wormName;
    [SerializeField] int hp;



}
