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

    public GameObject[] p1Worms;
    public GameObject[] p2Worms;
    private List<GameObject> p1TurnList;
    private List<GameObject> p2TurnList;

    private int p1TurnIndex = 0;
    private int p2TurnIndex = 0;
    //since there is only 2 players, true will be p1's turn while false will be p2's turn
    private bool isP1Turn = true;

    public static event Action onNextTurn;
    public static event Action<bool, Vector2> onTurnIndicatorChange;

    private void Awake()
    {
        p1TurnList = new List<GameObject>();
        p2TurnList = new List<GameObject>();
        FillWormLists();
    }

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
        turnState = TurnState.Preparing;
        
        p1TurnIndex = 0;
        p2TurnIndex = 0;
        
        isP1Turn = true;

        Vector2 indicatorPos = p1TurnList[p1TurnIndex].transform.position;
        onTurnIndicatorChange?.Invoke(isP1Turn, indicatorPos);
    }

    #region Turn List Management
    private void FillWormLists()
    {
        for (int i = 0; i < p1Worms.Length; i++)
        {
            p1TurnList.Add(p1Worms[i]);
            if (!p1Worms[i].TryGetComponent<IIndexable>(out IIndexable indexable))
                return;
            indexable.AssignIndex(i);
        }
        UpdateP1WormsIndex();

        for (int i = 0; i < p2Worms.Length; i++)
        {
            p2TurnList.Add(p2Worms[i]);
            if (!p2Worms[i].TryGetComponent<IIndexable>(out IIndexable indexable))
                return;
            indexable.AssignIndex(i);
        }
        UpdateP2WormsIndex();
    }

    private void UpdateP1WormsIndex()
    {
        for (int i = 0; i < p1TurnList.Count; i++)
        {
            if (!p1TurnList[i].TryGetComponent(out IIndexable indexable))
                return;
            indexable.AssignIndex(i);
        }
    }

    private void UpdateP2WormsIndex()
    {
        for (int i = 0; i < p2TurnList.Count; i++)
        {
            if (!p2TurnList[i].TryGetComponent(out IIndexable indexable))
                return;
            indexable.AssignIndex(i);
        }
    }
    
    public void RemoveObjectFromList(bool isP1, int index)
    {
        if (isP1)
        {
            p1TurnList.Remove(p1TurnList[index]);
            UpdateP1WormsIndex();
        }
        else
        {
            p2TurnList.Remove(p2TurnList[index]);
            UpdateP2WormsIndex();
        }
    }
    #endregion

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
        
        if (isP1Turn)
        {
            p1TurnIndex++;
            if (p1TurnIndex >= p1TurnList.Count)
            {
                p1TurnIndex = 0;
            }
        }
        else
        {
            p2TurnIndex++;
            if (p2TurnIndex >= p2TurnList.Count)
            {
                p2TurnIndex = 0;
            }
        }
        
        onNextTurn?.Invoke();

        onTurnIndicatorChange?.Invoke(isP1Turn, GetIndicatorPos());
        turnState = TurnState.Preparing;
    }

    private Vector2 GetIndicatorPos()
    {
        Vector2 indicatorPos;
        if (isP1Turn)
        {
            p1TurnIndex++;
            if (p1TurnIndex >= p1TurnList.Count)
            {
                p1TurnIndex = 0;
            }
            indicatorPos = p1TurnList[p1TurnIndex].transform.position;
        }
        else
        {
            p2TurnIndex++;
            if (p2TurnIndex >= p2TurnList.Count)
            {
                p2TurnIndex = 0;
            }
            indicatorPos = p2TurnList[p2TurnIndex].transform.position;
        }
        return indicatorPos;
    }
}
