using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TurnState
{
    Preparing,
    Firing,
    Waiting
}

public class TurnManager : MonoBehaviour
{
    public TurnState turnState;

    [SerializeField] private float delayAfterExplosion = 3f;

    [SerializeField] private GameObject[] p1Worms;
    [SerializeField] private GameObject[] p2Worms;
    private Stack<GameObject> p1TurnStack;
    private Stack<GameObject> p2TurnStack;

    private int p1TurnIndex = 0;
    private int p2TurnIndex = 0;
    //since there is only 2 players, true will be p1's turn while false will be p2's turn
    public bool isP1Turn = true;

    public static event Action onNextTurn;

    private void OnEnable()
    {
        //occurs at end of Firing TurnState
        DestructionTest.onMissileExplosion += OnExplosion;

        //occurs when turn timer finishes
        GameManager.onTurnTimerFinished += UpdateTurn;
    }

    private void OnDisable()
    {
        DestructionTest.onMissileExplosion -= OnExplosion;

        GameManager.onTurnTimerFinished -= UpdateTurn;
    }
    
    private void Start()
    {
        p1TurnStack = new Stack<GameObject>();
        p2TurnStack = new Stack<GameObject>();

        FillTurnStack();

        turnState = TurnState.Preparing;
    }

    private void FillTurnStack()
    {
        foreach (GameObject worms in p1Worms)
        {
            p1TurnStack.Push(worms);
        }

        foreach (GameObject worms in p2Worms)
        {
            p2TurnStack.Push(worms);
        }

        Debug.Log(p1TurnStack.Count);
        Debug.Log(p2TurnStack.Count);
    }

    private void OnExplosion()
    {
        turnState = TurnState.Waiting;

        StartCoroutine(WaitAfterExplosion(delayAfterExplosion));
    }

    IEnumerator WaitAfterExplosion(float delay)
    {
        yield return new WaitForSeconds(delay);

        UpdateTurn();
    }

    private void UpdateTurn()
    {
        isP1Turn = !isP1Turn;
        turnState = TurnState.Preparing;
        onNextTurn?.Invoke();
    }
}
