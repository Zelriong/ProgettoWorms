using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[DefaultExecutionOrder(-99)]
public class UIManager : MonoBehaviour
{
    private GameManager gm;
    private TurnManager tm;

    [Header("Menus")]
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject optionsMenu;
    [SerializeField] GameObject finalBluePanel;
    [SerializeField] GameObject finalRedPanel;
    [SerializeField] AudioClip buttonSFX;

    [SerializeField] private TMP_Text gameTimer;
    [SerializeField] private TMP_Text turnTimer;
    [SerializeField] private Image p1HealthBar;
    [SerializeField] private Image p2HealthBar;

    [Header("Turn Indicator")]
    [SerializeField] private GameObject turnIndicator;
    [SerializeField] private Image turnIndicatorImage;
    [SerializeField] private float offsetFromPlayer = 2f;

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
        InputManager.OnPause += OpenPauseMenu;    //perdoname luca por mi vida loca

        //occurs at beginning of Firing TurnState
        Bullet.onMissileExplosion += DeactivateTurnTimer;      //to be changed to when missile is launched

        //occurs at end of Waiting TurnState
        TurnManager.onTurnIndicatorChange += UpdateTurnIndicator;
        PlayerMovement.onPlayerMove += DeactivateTurnIndicator;

        //occurs at the end of the game
        GameManager.onRedTeamWin += RedWin;
        GameManager.onBlueTeamWin += BlueWin;
    }


    private void OnDisable()
    {
        GameManager.onGameTimerChange -= UpdateGameTimer;
        GameManager.onTurnTimerChange -= UpdateTurnTimer;
        GameManager.onP1HealthUpdated -= UpdateP1HealthBar;
        GameManager.onP2HealthUpdated -= UpdateP2HealthBar;
        InputManager.OnPause += OpenPauseMenu;

        //occurs at start of Firing TurnState
        Bullet.onMissileExplosion -= DeactivateTurnTimer;      //to be changed to when missile is launched

        //occurs at end of Waiting TurnState
        TurnManager.onTurnIndicatorChange -= UpdateTurnIndicator;
        PlayerMovement.onPlayerMove -= DeactivateTurnIndicator;

        //occurs at the end of the game
        GameManager.onRedTeamWin -= RedWin;
        GameManager.onBlueTeamWin -= BlueWin;
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
        turnIndicator.gameObject.SetActive(true);
        
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
    
    private void DeactivateTurnIndicator() => turnIndicator.gameObject.SetActive(false);

    private void UpdateP1HealthBar(float currentHealth, float maxHealth)
    {
        p1HealthBar.fillAmount = currentHealth / maxHealth;
    }

    private void UpdateP2HealthBar(float currentHealth, float maxHealth)
    {
        p2HealthBar.fillAmount = currentHealth / maxHealth;
    }

    public void OpenPauseMenu()
    {
        SoundFXManager.instance.PlaySoundFXClip(buttonSFX, transform, 1f);
        gm.timeStatus = TimeStatus.Stopped;
        optionsMenu.SetActive(false);
        pauseMenu.SetActive(true);
        return;
    }

    public void ClosePauseMenu()
    {
        SoundFXManager.instance.PlaySoundFXClip(buttonSFX, transform, 1f);
        gm.timeStatus = TimeStatus.Running;
        pauseMenu.SetActive(false);
        return;
    }

    public void OpenOptionsMenu()
    {
        SoundFXManager.instance.PlaySoundFXClip(buttonSFX, transform, 1f);
        pauseMenu.SetActive(false);
        optionsMenu.SetActive(true);
        return;
    }

    public void BlueWin()
    {
        gm.timeStatus = TimeStatus.Stopped;
        finalBluePanel.SetActive(true);

    }

    public void RedWin()
    {
        gm.timeStatus = TimeStatus.Stopped;
        finalRedPanel.SetActive(true);
    }
}
