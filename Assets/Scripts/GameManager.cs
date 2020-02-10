using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public static GameManager instance = null;
    public int playerHealth = 100;

    [SerializeField] private GameObject inGameMenu;
    [SerializeField] private bool isPaused;
    private int currentSceneIndex;

    void Awake()
    {
        if (instance == null)  //singleton pattern
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }

    void InitGame(Scene scene, LoadSceneMode mode)
    {
        isPaused = false;
        Time.timeScale = 1f;
        currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        if (currentSceneIndex == 0) //if main menu
            return;
        inGameMenu = GameObject.Find("In-Game Menu");
        inGameMenu.SetActive(false);
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += InitGame; //subscribing to event
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= InitGame;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && currentSceneIndex!=0)
            PauseGame();
    }

    public void PauseGame()
    {
        if(!isPaused)
        {
            isPaused = true;
            Time.timeScale = 0f;
            inGameMenu.SetActive(true);
        }
        else
        {
            isPaused = false;
            Time.timeScale = 1f;
            inGameMenu.SetActive(false);
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Level1");
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

}
