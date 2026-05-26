using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    private GameManager gm;
    private TurnManager tm;

    [SerializeField] private TMP_Text gameTimer;
    [SerializeField] private TMP_Text turnTimer;
    [SerializeField] private Image p1HealthBar;
    [SerializeField] private Image p2HealthBar;

    [Header("Turn Indicator")]
    [SerializeField] private GameObject turnIndicator;
    [SerializeField] private Image turnIndicatorImage;
    [SerializeField] private float offsetFromPlayer = 2f;
    [SerializeField] private float bounceAmount = 1f;
    [SerializeField] private float bounceDuration = 1f;

    private void Awake()
    {
        gm = FindAnyObjectByType<GameManager>();
        tm = FindAnyObjectByType<TurnManager>();
    }

    private void OnEnable()
    {
        GameManager.onGameTimerChange += UpdateGameTimer;
        GameManager.onTurnTimerChange += UpdateTurnTimer;
        GameManager.onP1HealthUpdated += UpdateP1HealthBar;
        GameManager.onP2HealthUpdated += UpdateP2HealthBar;

        //occurs at beginning of Firing TurnState
        DestructionTest.onMissileExplosion += DeactivateTurnTimer;      //to be changed to when missile is launched

        //occurs at end of Waiting TurnState
        TurnManager.onTurnIndicatorChange += UpdateTurnIndicator;
    }


    private void OnDisable()
    {
        GameManager.onGameTimerChange -= UpdateGameTimer;
        GameManager.onTurnTimerChange -= UpdateTurnTimer;
        GameManager.onP1HealthUpdated -= UpdateP1HealthBar;
        GameManager.onP2HealthUpdated -= UpdateP2HealthBar;

        //occurs at start of Firing TurnState
        DestructionTest.onMissileExplosion -= DeactivateTurnTimer;      //to be changed to when missile is launched

        //occurs at end of Waiting TurnState
        TurnManager.onTurnIndicatorChange -= UpdateTurnIndicator;
    }

    private void Start()
    {
        p1HealthBar.fillAmount = 1f;
        p2HealthBar.fillAmount = 1f;
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

    private void UpdateTurnIndicator(bool isP1Turn, Vector2 position)
    {
        if (isP1Turn)
        {
            turnIndicator.transform.position = new Vector2(position.x, position.y + offsetFromPlayer);
            turnIndicatorImage.color = Color.red;
        }
        else
        {
            turnIndicator.transform.position = new Vector2(position.x, position.y + offsetFromPlayer);
            turnIndicatorImage.color = Color.blue;
        }

        turnTimer.gameObject.SetActive(true);
    }

    private void UpdateP1HealthBar(float currentHealth, float maxHealth)
    {
        p1HealthBar.fillAmount = currentHealth / maxHealth;
    }

    private void UpdateP2HealthBar(float currentHealth, float maxHealth)
    {
        p2HealthBar.fillAmount = currentHealth / maxHealth;
    }
}
