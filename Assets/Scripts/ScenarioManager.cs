using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScenarioManager : MonoBehaviour
{
    private int curLevel = 0;

    public int durchlaeufe = 0;

    public int enemiesOnScreen = 0;
    public int powerUpsOnScreen = 0;

    GameManager gameManager;
    LevelManager levelManager;
    PlayerController playerController;
    
    public ParallaxBGController BGController;
    public ScenarioController[] prefabsScenarios;
    private ScenarioController[] Scenarios;
    public ScenarioController activeScenario;

    public Text BossAlert;

    private float SchwierigkeitdurchZeit = 0f;
    private float TimeToBoss = 0f;    
    
    public bool IsBossActive = false;
    
    private int BosseFinished = 0;

    private float idleTime = 0f;
    private float idleTimePowerUp = 0f;


    IEnumerator StageCleared()
    {                 
        yield return new WaitForSecondsRealtime(10);

        NextLevel();
    }
    
    private void NextLevel()
    {
        StopCoroutine("StageCleared");

        curLevel++;

        if (curLevel >= Scenarios.Length)
        {
            gameManager.GoToVictory();
        }
        else
        {
            loadScenario(curLevel);
            gameManager.ChangeState(GameState.Level);
        }
    }

    public void BossBesiegt()
    {
        BosseFinished++;

        IsBossActive = false;

        if(BosseFinished >= activeScenario.spawnableBossPrefab.Length)
        {
            gameManager.ChangeState(GameState.StageCleared);
            StartCoroutine(StageCleared());
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        levelManager = FindObjectOfType<LevelManager>();
        playerController = FindObjectOfType<PlayerController>();

        List<ScenarioController> s = new List<ScenarioController>();

        for(int a = 0; a < prefabsScenarios.Length; a++)
        {
            s.Add(Instantiate(prefabsScenarios[a]));
        }

        Scenarios = s.ToArray();
    }

    private GameObject[] GetEnemyPrefabsBySchwierigkeit(float Schwierigkeit)
    {
        List<GameObject> list = new List<GameObject>();

        foreach (GameObject obj in activeScenario.spawnablePrefabs)
        {
            if (obj.GetComponent<EnemyController>().Schwierigkeit < Schwierigkeit)
            {
                list.Add(obj);
            }
        }

        GameObject[] result = list.ToArray();

        return result;
    }

    public void loadScenario(int index = -1)
    {        
        if (index == -1)
        {
            activeScenario = Scenarios[Random.Range(0, Scenarios.Length)];
        }
        else
        {
            activeScenario = Scenarios[index];
        }

        foreach(ParallaxController parallax in FindObjectsOfType<ParallaxController>())
        {
            if(parallax.gameObject.name.Contains("(Clone)") == true)
            {
                Destroy(parallax.gameObject);
            }
        }

        BGController.GetComponent<MeshRenderer>().material = activeScenario.BGMaterial;

        gameManager.PlayMusic(activeScenario.soundtrack.ToString(), 1, 1f);

        SchwierigkeitdurchZeit = (curLevel + durchlaeufe) * 50f;
        BosseFinished = 0;
    }

    private void SpawnUpdate()
    {
        float rnd = Random.Range(0f, 50f);

        SchwierigkeitdurchZeit += Time.deltaTime;

        if (IsBossActive == false)
            TimeToBoss += Time.deltaTime;

        float Wahrscheinlichkeit = (activeScenario.baseSpawnWahrscheinlichkeit * gameManager.SchwierigkeitsModifikator) + (SchwierigkeitdurchZeit / 1000);

        float Schwierigkeit = (activeScenario.baseSpawnWahrscheinlichkeit * gameManager.SchwierigkeitsModifikator) + (SchwierigkeitdurchZeit / 50);

        if (TimeToBoss > activeScenario.SecondsToBoss)
        {          
            GameObject prefab = activeScenario.spawnableBossPrefab[Random.Range(0, activeScenario.spawnableBossPrefab.Length)];

            gameManager.PlayMusic(prefab.GetComponent<BossController>().soundtrack.ToString(), 2, 1);

            GameObject g;

            // Alle Bosse spawnen auf x = 0
            //if (prefab.name != "Boss 3" && prefab.name != "Boss 4")
            //{
                //g = Instantiate(prefab, levelManager.GetComponent<SpawnController>().GetSpawnPoint(), Quaternion.identity);
            //}
           // else if (prefab.name == "Boss 3" || prefab.name == "Boss 4")
            //{
                Vector2 spawnpoint = levelManager.GetComponent<SpawnController>().GetSpawnPoint();

                spawnpoint.x = 0;

                g = Instantiate(prefab, spawnpoint, Quaternion.identity);

            //}

            IsBossActive = true;

            TimeToBoss = 0;
        }

        if (((rnd < (Wahrscheinlichkeit) || idleTime > activeScenario.maxIdleTime) && IsBossActive == false) && enemiesOnScreen <= (activeScenario.EnemiesOnScreenLimit))
        {
            idleTime = 0f;

            GameObject[] prefabs = GetEnemyPrefabsBySchwierigkeit(Schwierigkeit);

            //GameObject spawnpoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            GameObject prefab = prefabs[Random.Range(0, prefabs.Length)];

            EnemyController enemyController = prefab.GetComponent<EnemyController>();

            Vector2 spawnPos = levelManager.GetComponent<SpawnController>().GetSpawnPoint();

            string spawnname = "Top";

            if (enemyController.SpawnModus == SpawnMode.FromSide)
            {
                int dir = Random.Range(0, 100);
                
                if (dir < 50)
                {
                    spawnname = "Left";
                    enemyController.BewegungsModus = MovementMode.MoveHorizontalToRight;
                }
                else
                {
                    spawnname = "Right";
                    enemyController.BewegungsModus = MovementMode.MoveHorizontalToLeft;

                }

                spawnPos = levelManager.GetComponent<SpawnController>().GetSpawnPoint(spawnname);
            }
            else if(enemyController.SpawnModus == SpawnMode.Default)
            {
                if(spawnPos.x > 0)
                {
                    spawnPos.x -= 0.5f;
                }
                if (spawnPos.x < 0)
                {
                    spawnPos.x += 0.5f;
                }
            }
            
            GameObject g = Instantiate(prefab, spawnPos, Quaternion.identity);            

            g.GetComponent<SpriteRenderer>().sortingLayerName = "Enemy";

            if (enemyController.SpawnMenge > 1)
            {
                Vector2 dubSpawnPos = spawnPos;
                for (int a = 1; a < enemyController.SpawnMenge; a++)
                {
                    
                    switch(spawnname)
                    {
                        case "Top":
                            {
                                dubSpawnPos = new Vector2(dubSpawnPos.x, dubSpawnPos.y + 1);
                                break;
                            }
                        case "Left":
                            {
                                dubSpawnPos = new Vector2(dubSpawnPos.x - 1, dubSpawnPos.y);
                                break;
                            }
                        case "Right":
                            {
                                dubSpawnPos = new Vector2(dubSpawnPos.x + 1, dubSpawnPos.y);
                                break;
                            }
                        default:
                            {
                                dubSpawnPos = new Vector2(dubSpawnPos.x, dubSpawnPos.y + 1);
                                break;
                            }
                    }

                    GameObject obj = Instantiate(prefab, dubSpawnPos, Quaternion.identity);

                    obj.GetComponent<SpriteRenderer>().sortingLayerName = "Enemy";
                }
            }
        }
        else
        {
            idleTime += Time.deltaTime;
        }
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        if (gameManager.gameState == GameState.Level)
        {
            if(IsBossActive == true)
            {
                BossAlert.GetComponent<BlinkController>().StartBlinking();
            }
            else
            {
                BossAlert.GetComponent<BlinkController>().StopBlinking();
            }

            SpawnUpdate();

            float rnd = Random.Range(0f, 50f);

            ScenarioController scenario = activeScenario;

            if (rnd < scenario.baseSpawnParallaxWahrscheinlichkeit)
            {
                //GameObject spawnpoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
                GameObject prefab = scenario.spawnableParallaxPrefabs[Random.Range(0, scenario.spawnableParallaxPrefabs.Length)];

                string spawnName = prefab.GetComponent<ParallaxController>().spawnPoint.ToString();

                GameObject g = Instantiate(prefab, levelManager.GetComponent<SpawnController>().GetSpawnPoint(spawnName), Quaternion.identity);

                if(g.GetComponent<DestructionController>() != null)
                {
                    if(g.GetComponent<ParallaxController>().spawnPoint == SpawnPoint.Bottom)
                    {
                        int rndDestruction = Random.Range(0, 100);
                        if(rndDestruction < 50)
                        {
                            g.GetComponent<DestructionController>().enabled = false;
                        }
                    }
                }

                string sortingLayer = "ParallaxBack";

                g.GetComponent<SpriteRenderer>().sortingLayerName = sortingLayer;
            }
            if (rnd < scenario.baseSpawnParallaxPassiveWahrscheinlichkeit)
            {
                //GameObject spawnpoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
                GameObject prefab = scenario.spawnableParallaxPassivePrefabs[Random.Range(0, scenario.spawnableParallaxPassivePrefabs.Length)];

                string spawnName = prefab.GetComponent<ParallaxController>().spawnPoint.ToString();

                GameObject g = Instantiate(prefab, levelManager.GetComponent<SpawnController>().GetSpawnPoint(spawnName), Quaternion.identity);

                int parallaxrnd = Random.Range(1, 2);

                string sortingLayer = "ParallaxBack";

                g.GetComponent<SpriteRenderer>().sortingLayerName = sortingLayer;
            }
            if ((rnd < (scenario.basePowerUpWahrscheinlichkeit / gameManager.SchwierigkeitsModifikator) ||  idleTimePowerUp > activeScenario.maxIdleTimePowerUp) && powerUpsOnScreen <= (activeScenario.PowerUpsOnScreenLimit))
            {
                idleTimePowerUp = 0f;
                levelManager.GetComponent<SpawnController>().SpawnPowerUp(levelManager.GetComponent<SpawnController>().GetSpawnPointInViewport(), scenario.spawnablePowerUps);
            }
            else
            {
                if (powerUpsOnScreen >= activeScenario.PowerUpsOnScreenLimit)
                {
                    idleTimePowerUp = 0f;
                }
                else
                {
                    idleTimePowerUp += Time.deltaTime;
                }
            }
        }
    }
}
