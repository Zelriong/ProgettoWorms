using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    [SerializeField] private GameObject[] p1Worms;
    [SerializeField] private GameObject[] p2Worms;
    private Stack<GameObject> turnStack;

    private int currentTurn = 0;

    private void OnEnable()
    {
        DestructionTest.onMissileLaunched += UpdateTurn;
    }

    private void OnDisable()
    {
        DestructionTest.onMissileLaunched -= UpdateTurn;
    }
    
    private void Start()
    {
        turnStack = new Stack<GameObject>();

        FillTurnStack();
    }

    private void FillTurnStack()
    {
        if (p1Worms.Length < p2Worms.Length)
        {
            for (int i = 0; i < p2Worms.Length; i++)
            {
                if (p1Worms.Length > i)
                    turnStack.Push(p1Worms[i]);
                
                turnStack.Push(p2Worms[i]);
            }
        }
        else if (p1Worms.Length > p2Worms.Length)
        {
            for (int i = 0; i < p1Worms.Length; i++)
            {
                turnStack.Push(p1Worms[i]);

                if (p2Worms.Length > i)
                    turnStack.Push(p2Worms[i]);
            }
        }
        else
        {
            for (int i = 0; i < p1Worms.Length; i++)
            {
                turnStack.Push(p1Worms[i]);
                turnStack.Push(p2Worms[i]);
            }
        }
        
        Debug.Log(turnStack.Count);
    }

    private void UpdateTurn()
    {
        
    }
}
