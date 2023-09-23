using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Soundtrack
{
    Track1,
    Track2,
    Track3,
    Track4,
    Track5,
    Track6,
    Track7,
    Track8,
    Track9,
    Track10
}

public class ScenarioController : MonoBehaviour
{
    public Material BGMaterial;

    private LevelManager levelManager;
    private GameManager gameManager;

    [Header("Spawn-Einstellungen")]
    public float baseSpawnParallaxWahrscheinlichkeit = 1f;
    public GameObject[] spawnableParallaxPrefabs;
    public float baseSpawnParallaxPassiveWahrscheinlichkeit = 2f;
    public GameObject[] spawnableParallaxPassivePrefabs; 
    public float baseSpawnWahrscheinlichkeit = 0.1f;
    public GameObject[] spawnablePrefabs;
    public float basePowerUpWahrscheinlichkeit = 0.4f;
    public GameObject[] spawnablePowerUps;

    [Header("Allgemeine Level-Eigenschaften")]
    [Tooltip("Maximale Zeit in der keine Gegner spawnen.")]
    public float maxIdleTime = 3f;
    [Tooltip("Maximale Anzahl an Gegnern auf dem Screen.")]
    public int enemiesOnScreenLimit = 5;
    [Tooltip("Maximale Zeit in der kein PowerUps spawnen.")]
    public float maxIdleTimePowerUp = 3f;
    [Tooltip("Maximale Anzahl an PowerUps auf dem Screen.")]
    public int powerUpsOnScreenLimit = 5;
    [Tooltip("Zeit nach der der Boss spawnt.")]
    public float SecondsToBoss = 10;
    public GameObject[] spawnableBossPrefab;
    
    public int EnemiesOnScreenLimit
    {
        get
        {
            int value = Mathf.RoundToInt((gameManager.SchwierigkeitsModifikator * 2) * enemiesOnScreenLimit);

            value += levelManager.GetComponent<ScenarioManager>().durchlaeufe;

            return value;
        }
    }

    public int PowerUpsOnScreenLimit
    {
        get
        {
            int value = Mathf.RoundToInt(powerUpsOnScreenLimit / (gameManager.SchwierigkeitsModifikator * 2));

            value -= levelManager.GetComponent<ScenarioManager>().durchlaeufe;

            if(value < 1)
            {
                value = 1;
            }

            return value;
        }
    }

    public Soundtrack soundtrack;

    // Start is called before the first frame update
    void Awake()
    {
        levelManager = FindObjectOfType<LevelManager>();
        gameManager = FindObjectOfType<GameManager>();

        maxIdleTime = Mathf.RoundToInt(maxIdleTime * gameManager.SchwierigkeitsModifikator);
        maxIdleTimePowerUp = Mathf.RoundToInt(maxIdleTimePowerUp * gameManager.SchwierigkeitsModifikator);

        if(maxIdleTime <= 1)
        {
            maxIdleTime = 1;
        }
        if (maxIdleTimePowerUp <= 1)
        {
            maxIdleTimePowerUp = 1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
