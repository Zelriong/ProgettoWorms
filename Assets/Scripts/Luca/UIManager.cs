using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UIManager : MonoBehaviour
{
    private GameManager gm;
    private TurnManager tm;

    [SerializeField] private TMP_Text gameTimer;
    [SerializeField] private TMP_Text turnTimer;
    [SerializeField] private Image p1Icon;
    [SerializeField] private Image p2Icon;

    private void Awake()
    {
        gm = FindAnyObjectByType<GameManager>();
        tm = FindAnyObjectByType<TurnManager>();
    }

    private void OnEnable()
    {
        GameManager.onGameTimerChange += UpdateGameTimer;
        GameManager.onTurnTimerChange += UpdateTurnTimer;

        //occurs at beginning of Firing TurnState
        DestructionTest.onMissileExplosion += DeactivateTurnTimer;      //to be changed to when missile is launched

        //occurs at end of Waiting TurnState
        TurnManager.onNextTurn += SwitchToNextPlayer;
    }


    private void OnDisable()
    {
        GameManager.onGameTimerChange -= UpdateGameTimer;
        GameManager.onTurnTimerChange -= UpdateTurnTimer;

        //occurs at start of Firing TurnState
        DestructionTest.onMissileExplosion -= DeactivateTurnTimer;      //to be changed to when missile is launched

        //occurs at end of Waiting TurnState
        TurnManager.onNextTurn -= SwitchToNextPlayer;
    }

    private void Start()
    {
        SwitchToNextPlayer();
    }

    private void UpdateGameTimer(float timer)
    {
        int timerRounded = Mathf.RoundToInt(timer);

        int minutes = timerRounded / 60;
        int seconds = timerRounded - (minutes * 60);

        gameTimer.text = string.Format("{00:00}:{01:00}", minutes, seconds);
    }

    private void UpdateTurnTimer(float timer)
    {
        int timerRounded = Mathf.RoundToInt(timer);

        turnTimer.text = timerRounded.ToString();
    }
    
    private void DeactivateTurnTimer() => turnTimer.gameObject.SetActive(false);

    private void SwitchToNextPlayer()
    {
        if (tm.isP1Turn)
        {
            p1Icon.gameObject.SetActive(true);
            p2Icon.gameObject.SetActive(false);
        }
        else
        {
            p1Icon.gameObject.SetActive(false);
            p2Icon.gameObject.SetActive(true);
        }

        turnTimer.gameObject.SetActive(true);
    }
}
