using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum Schwierigkeit
{
    Easy,
    Medium,
    Hard
}

public enum GameState
{
    Starting,
    MainMenue,
    Schwierigkeit,
    Level,
    StageCleared,
    Highscore,
    GameOver
}

public class GameManager : MonoBehaviour
{
    private float scoreMultiplier = 1f;

    public float ScoreMultiplier
    {
        get
        {
            return scoreMultiplier;
        }
        set
        {
            if(value >= 10)
            {
                value = 10;
            }
            if(value < 1)
            {
                value = 1;
            }
            scoreMultiplier = value;
        }
    }

    private Schwierigkeit m_schwierigkeit;

    public Schwierigkeit schwierigkeit
    {
        get
        {
            return m_schwierigkeit;
        }
    }
        
    public AudioMixer mixer;

    private string curMusic = "";

    public AudioSource[] Soundtrack;

    public GameObject mainMenuePanel;

    public Canvas UICanvas;

    public GameObject playerPrefab;
    public GameObject playerDummy;

    public GameObject mainMenueStartButton;
    public GameObject mainMenueHighscoreButton;
    public GameObject mainMenueDatenschutzButton;
    public GameObject mainMenueImpressumButton;
    public GameObject debugInfos;
    public GameObject levelReturnUi;

    public Text debugSicherheit;
    public Text debugFluggeschick; 
    public Text debugTechnik;
    public Text debugTechnikZeit;
    public Text debugBeschleunigung;
    public Text debugMaxSpeed;
    public Text debugSchieldTime;

    public Text versionInfo;

    public GameObject[] mainMenueSchwierigkeitsButtons;

    public GameObject[] levelUIElements;

    [HideInInspector]
    public GameObject UILevelBar;

    public GameObject GameOverUi;
    public Text GameOverText;
    public Text GameOverScore;

    public Text scoreText;
    //public Text UIMunition;
    public Text StageClear;

    public GameObject player;
    public LevelManager levelManager;
    public ScenarioManager scenarioManager;

    private Coroutine fader = null;

    public GameState gameState;

    public float SchwierigkeitsModifikator = 1;

    public void PlayAudio(AudioClip sound, float volume = 0.05f)
    {
        GetComponent<AudioSource>().volume = volume;
        GetComponent<AudioSource>().PlayOneShot(sound);
    }

    private void StartMusic(string name)
    {
        foreach(AudioSource track in Soundtrack)
        {
            if(track.name == name)
            {
                track.Play();
                break;
            }
        }
    }

    private void StopMusic(string name)
    {
        foreach (AudioSource track in Soundtrack)
        {
            if (track.name == name)
            {
                track.Stop();
                break;
            }
        }
    }

    public void PlayMusic(string newMusic, float duration, float targetVolume)
    {
        if (curMusic != newMusic)
        {

            if(fader != null)
            {
                StopCoroutine(fader);
            }

            StopMusic(newMusic);
            StartMusic(newMusic);

            fader = StartCoroutine(FadeMixerGroup.StartFade(mixer, curMusic, newMusic, (duration), targetVolume));

            curMusic = newMusic;
        }
    }

    void unityAnalytics()
    {
        UnityEngine.Analytics.Analytics.enabled = false;
        UnityEngine.Analytics.Analytics.deviceStatsEnabled = false;
        UnityEngine.Analytics.Analytics.initializeOnStartup = false;
        UnityEngine.Analytics.Analytics.limitUserTracking = false;
        UnityEngine.Analytics.PerformanceReporting.enabled = false;
    }

    // Start is called before the first frame update
    void Awake()
    {
        unityAnalytics();
        gameState = GameState.Starting;
        versionInfo.text = "Version: " + Application.version;
        UILevelBar = GameObject.Find("LevelUI");
        levelManager = FindObjectOfType<LevelManager>();
        GoToMainMenue();
        player.GetComponent<PlayerController>().highscore = PlayerPrefs.GetInt("Highscore", 0);
        Debug.Log("Pixel width :" + Camera.main.pixelWidth + " Pixel height : " + Camera.main.pixelHeight);        
    }

    private void OnDisable()
    {
        
    }

    private void Update()
    {
        if (gameState == GameState.Level)
        {
            scoreText.text = "Score: " + player.GetComponent<PlayerController>().score.ToString() + " X " + ScoreMultiplier.ToString("0.0").Replace(",",".");
            //highscoreText.text = "Highscore: " + player.GetComponent<PlayerController>().highscore.ToString();

            debugSicherheit.text = "Treffsicherheit: " + player.GetComponent<ShipController>().Treffsicherheit;
            debugFluggeschick.text = "Fluggeschick: " + player.GetComponent<ShipController>().Fluggeschick;
            debugTechnik.text = "RumpfRepair: " + player.GetComponent<ShipController>().Technik;
            debugTechnikZeit.text = "RumpfRepairTime: " + player.GetComponent<ShipController>().TechnikZeit;
            debugBeschleunigung.text = "Beschleunigung: " + player.GetComponent<ShipController>().Beschleunigung;
            debugMaxSpeed.text = "maxSpeed: " + player.GetComponent<ShipController>().MaximaleGeschwindigkeit;
            debugSchieldTime.text = "ShieldRepairTime: " + player.GetComponent<ShipController>().SchildAufladung;

        }
        if(gameState == GameState.Schwierigkeit)
        {
            if(Input.GetKeyDown(KeyCode.Escape) == true)
            {
                GoToMainMenue();
            }
        }
        else if(gameState == GameState.Level)
        {
            if (Input.GetKeyDown(KeyCode.Escape) == true)
            {
                if (levelReturnUi.activeSelf == false)
                {
                    PauseGame();
                }
                else
                {
                    ResumeGame();
                }
            }
        }
        else if (gameState == GameState.MainMenue)
        {
            if (Input.GetKeyDown(KeyCode.Escape) == true)
            {
                Application.Quit();
            }
        }
    }
    
    public bool PointIsInViewport(Vector2 point)
    {
        bool result = false;

        RectTransform UIRect = UILevelBar.GetComponent<RectTransform>();

        Vector2 UISize = Camera.main.ScreenToWorldPoint(new Vector2(UIRect.sizeDelta.x, UIRect.sizeDelta.y));

        //Debug.Log(UICanvas.scaleFactor);

        Vector2 origin = new Vector2(0, 0 + (UIRect.rect.height * UICanvas.scaleFactor));
        Vector2 size = new Vector2(Screen.width, Screen.height);
        
        Rect rect = new Rect(origin.x, origin.y, size.x, size.y);

        //Debug.Log("Rect: " + rect.ToString() + " Point: " + point.ToString() + " Point(Converted): " + Camera.main.WorldToScreenPoint(point).ToString(), this);

        result = rect.Contains(Camera.main.WorldToScreenPoint(point));

        return result;
    }

    private void switchLevelUIElements(bool value)
    {
        foreach (GameObject obj in levelUIElements)
        {
            obj.SetActive(value);
        }
        //debugInfos.SetActive(value);
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
        switchLevelReturnUI(true);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        switchLevelReturnUI(false);
    }

    public void GoToWWWDatenschutz()
    {
        if (Application.systemLanguage == SystemLanguage.German)
        {
            Application.OpenURL("http://coding.r-c-n.de/datenschutz.html");
        }
        else if(Application.systemLanguage == SystemLanguage.English)
        {
            Application.OpenURL("http://coding.r-c-n.de/en.datenschutz.html");
        }
        else
        {
            Application.OpenURL("http://coding.r-c-n.de/datenschutz.html");
        }
    }

    public void GoToWWWImpressum()
    {        
        if (Application.systemLanguage == SystemLanguage.German)
        {
            Application.OpenURL("http://coding.r-c-n.de/impressum.html");
        }
        else if (Application.systemLanguage == SystemLanguage.English)
        {
            Application.OpenURL("http://coding.r-c-n.de/disclaimer.html");
        }
        else
        {
            Application.OpenURL("http://coding.r-c-n.de/impressum.html");
        }
    }

    public void GoToMainMenue()
    {        
        levelManager.GetComponent<SpawnController>().enabled = false;
        PlayMusic("Track6",1f,1f);        
        ChangeState(GameState.MainMenue);
    }

    public void GoToSchwierigkeitsAuswahl()
    {
        levelManager.GetComponent<SpawnController>().enabled = false;
        ChangeState(GameState.Schwierigkeit);
    }

    // Nur für Beta-Zwecke
    public void GoToLevelSuperEasy()
    {
        m_schwierigkeit = Schwierigkeit.Easy;
        GoToLevel(0.5f);
    }

    public void GoToLevelEasy()
    {
        m_schwierigkeit = Schwierigkeit.Easy;
        GoToLevel(0.75f);
    }

    public void GoToLevelMedium()
    {
        m_schwierigkeit = Schwierigkeit.Medium;
        GoToLevel(1f);
    }
    public void GoToLevelHard()
    {
        m_schwierigkeit = Schwierigkeit.Hard;
        GoToLevel(1.25f);
    }

    public void GoToLevel(float Schwierigkeit = 1f)
    {
        levelManager.GetComponent<SpawnController>().enabled = true;
        SchwierigkeitsModifikator = Schwierigkeit;
        scenarioManager.loadScenario(0);
        ChangeState(GameState.Level);
    }
    public void GoToGameOver()
    {
        levelManager.GetComponent<SpawnController>().enabled = false;
        ChangeState(GameState.GameOver);
    }
    public void GoToVictory()
    {
        levelManager.GetComponent<SpawnController>().enabled = false;
        ChangeState(GameState.GameOver);

        GameOverText.text = "Victory";


        string HighscoreID = "";

        switch (schwierigkeit)
        {
            case Schwierigkeit.Easy:
                {
                    HighscoreID = GPGSIds.leaderboard_highscore_easy;
                    break;
                }
            case Schwierigkeit.Medium:
                {
                    HighscoreID = GPGSIds.leaderboard_highscore_medium;
                    break;
                }
            case Schwierigkeit.Hard:
                {
                    HighscoreID = GPGSIds.leaderboard_highscore_hard;
                    break;
                }
        }

        if (HighscoreID != "")
        {
            GetComponent<ServiceManager>().SendScore(player.GetComponent<PlayerController>().score, HighscoreID);
        }

        GetComponent<ServiceManager>().SendScore(player.GetComponent<PlayerController>().score);
    }

    public void SwitchPlayerObject(bool value)
    {
        foreach (PlayerController child in player.GetComponents<PlayerController>())
        {
            child.enabled = value;
        }
        foreach (SpriteRenderer child in player.GetComponentsInChildren<SpriteRenderer>())
        {
            child.enabled = value;
        }
        foreach (Animator child in player.GetComponentsInChildren<Animator>())
        {
            child.enabled = value;
        }
        foreach (ShipController child in player.GetComponents<ShipController>())
        {
            child.enabled = value;
        }
        foreach (WeaponController child in player.GetComponentsInChildren<WeaponController>())
        {
            child.enabled = value;
        }
        if(value == false)
        {
            player.GetComponent<PlayerController>().powerUpText.GetComponent<MeshRenderer>().enabled = value;
        }
    }

    private void switchSchwierigkeitsAuswahl(bool value)
    {
        foreach (GameObject schwierigkeit in mainMenueSchwierigkeitsButtons)
        {
            schwierigkeit.SetActive(value);
        }
    }

    private void switchStageClear(bool value)
    {        
        StageClear.gameObject.SetActive(value);        
    }

    private void switchGameOver(bool value)
    {
        GameOverUi.gameObject.SetActive(value);
    }

    private void switchLevelReturnUI(bool value)
    {
        levelReturnUi.SetActive(value);
    }

    public void ChangeState(GameState state)
    {
        switch (state)
        {
            case GameState.MainMenue:
                {

                    if (gameState == GameState.Level || gameState == GameState.StageCleared || gameState == GameState.GameOver)
                    {
                        SceneManager.LoadScene("Level");
                    }

                    gameState = GameState.MainMenue;

                    Time.timeScale = 1;
                    mainMenuePanel.SetActive(true);
                    mainMenueStartButton.SetActive(true);
                    mainMenueHighscoreButton.SetActive(true);
                    mainMenueDatenschutzButton.SetActive(true);
                    mainMenueImpressumButton.SetActive(true);
                    switchLevelUIElements(false);
                    switchSchwierigkeitsAuswahl(false);
                    SwitchPlayerObject(false);
                    switchStageClear(false);
                    switchGameOver(false);
                    switchLevelReturnUI(false);

                    break;
                }
            case GameState.Schwierigkeit:
                {
                    gameState = GameState.Schwierigkeit;

                    mainMenueStartButton.SetActive(false);
                    mainMenueHighscoreButton.SetActive(false);
                    switchSchwierigkeitsAuswahl(true);
                    switchStageClear(false);
                    switchGameOver(false);
                    switchLevelReturnUI(false);

                    

                    break;
                }
            case GameState.Level:
                {
                    gameState = GameState.Level;

                    //if (player != null)
                    //Destroy(player);

                    //player = Instantiate(playerPrefab, new Vector2(0, -2), Quaternion.identity);
                    mainMenuePanel.SetActive(false);
                    mainMenueDatenschutzButton.SetActive(false);
                    mainMenueImpressumButton.SetActive(false);
                    switchSchwierigkeitsAuswahl(false);
                    switchLevelUIElements(true);
                    SwitchPlayerObject(true);
                    switchStageClear(false);
                    switchGameOver(false);
                    switchLevelReturnUI(false);

                    player.transform.position = new Vector2(0, -2);
                    player.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);

                    Time.timeScale = 1;                    

                    break;
                }
            case GameState.GameOver:
                {
                    gameState = GameState.GameOver;

                    GameOverScore.text = player.GetComponent<PlayerController>().score.ToString();
                    
                    mainMenuePanel.SetActive(false);
                    mainMenueDatenschutzButton.SetActive(false);
                    mainMenueImpressumButton.SetActive(false);
                    switchSchwierigkeitsAuswahl(false);
                    switchLevelUIElements(false);
                    SwitchPlayerObject(false);
                    switchStageClear(false);
                    switchGameOver(true);
                    switchLevelReturnUI(false);

                    GameOverText.text = "Game Over";
                    /*

                    foreach(EnemyController g in FindObjectsOfType<EnemyController>())
                    {
                        if(g.name.Contains("(Clone)"))
                        {
                            Destroy(g);
                        }
                    }

                    foreach (BossController g in FindObjectsOfType<BossController>())
                    {
                        if (g.name.Contains("(Clone)"))
                        {
                            Destroy(g);
                        }
                    }

                    foreach (BulletController g in FindObjectsOfType<BulletController>())
                    {
                        if (g.name.Contains("(Clone)"))
                        {
                            Destroy(g);
                        }
                    }

                    */

                    Time.timeScale = 0;                    
                                        
                    break;
                }
                           

            case GameState.StageCleared:
                {
                    gameState = GameState.StageCleared;

                    mainMenuePanel.SetActive(false);
                    mainMenueDatenschutzButton.SetActive(false);
                    mainMenueImpressumButton.SetActive(false);
                    switchSchwierigkeitsAuswahl(false);
                    switchLevelUIElements(false);
                    SwitchPlayerObject(false);
                    switchStageClear(true);
                    switchLevelReturnUI(false);

                    foreach(PowerUpController powerup in FindObjectsOfType<PowerUpController>())
                    {
                        if(powerup.name.Contains("(Clone)"))
                        {
                            Destroy(powerup);
                        }
                    }

                    Time.timeScale = 1;                    

                    Vector2 playerPos = player.transform.position;

                    GameObject dummy = Instantiate(playerDummy, playerPos, Quaternion.identity);

                    dummy.transform.Rotate(new Vector3(0, 0, 180));

                    dummy.GetComponent<Rigidbody2D>().velocity = (new Vector2(0, 3));

                    Destroy(dummy, 10);

                    ShipController playerShipController = player.GetComponent<ShipController>();

                    playerShipController.curRumpf = playerShipController.Rumpf;
                    playerShipController.curSchildEnergie = playerShipController.SchildEnergie;

                    break;
                }
        }
    }

}
