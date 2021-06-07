using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_Manager : MonoBehaviour
{
    public static UI_Manager instance;
    [Header("Main")]
    [SerializeField] Text jumpText;
    [SerializeField] Text enemyText;
    [SerializeField] Text timeText;


    [Header("Panels")]
    [SerializeField] GameObject pausePanel;

    [SerializeField] GameObject takePanel;
    public GameObject TakeText
    {
        get { return takePanel; }
    }
    
    [SerializeField] GameObject interactPanel;
    public GameObject InteractText
    {
        get { return interactPanel; }
    }

    int jumpCount = 0;
    int enemyCount = 0;
    float timer = 0;

    bool inGame;
    EventsManager.GameState currentState;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        currentState = EventsManager.GameState.Menu;

        EventsManager.instance.onJumpTrigger += UpdateJump;
        EventsManager.instance.onEnemyDestroyTrigger += UpdateEnemyDestroy;
        EventsManager.instance.onChangeStateTrigger += InstanceChangeStateTrigger;
    }

    private void OnEnable()
    {
        EventsManager.instance.onJumpTrigger -= UpdateJump;
        EventsManager.instance.onEnemyDestroyTrigger -= UpdateEnemyDestroy;
        EventsManager.instance.onChangeStateTrigger -= InstanceChangeStateTrigger;
    }

    private void InstanceChangeStateTrigger(EventsManager.GameState state)
    {
        currentState = state;

        switch (state)
        {
            case EventsManager.GameState.Menu:
                inGame = false;
                break;

            case EventsManager.GameState.Play:
                timer = Time.time;
                inGame = true;
                break;

            case EventsManager.GameState.Win:
                inGame = false;
                break;

            default:
                break;
        }
    }

    private void Update()
    {
        if (inGame) timeText.text = (Time.time - timer).ToString();

        if (Input.GetButtonDown("Menu") && currentState != EventsManager.GameState.Menu)
        {
            pausePanel.SetActive(true);
            Time.timeScale = 0;
        }
    }

    private void UpdateJump()
    {
        jumpCount++;
        jumpText.text = jumpCount.ToString();
    }

    private void UpdateEnemyDestroy()
    {
        enemyCount++;
        enemyText.text = enemyCount.ToString();
    }

    public void StartGame()
    {
        EventsManager.instance.ChangeStateTrigger(EventsManager.GameState.Play);
    }

    public void RestartLevel()
    {
        Application.LoadLevel(Application.loadedLevel);
    }

    public void ContinueGame()
    {
        Time.timeScale = 1;
    }
}
