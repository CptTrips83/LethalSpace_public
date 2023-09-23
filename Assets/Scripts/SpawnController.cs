using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnController : MonoBehaviour
{
    //private GameObject[] spawnPoints;

    public GameObject spawner;
    public GameObject spawnerRight;
    public GameObject spawnerLeft;
    public GameObject spawnerBottom;

    public GameManager gameManager;
        
    //public float basePowerUpWahrscheinlichkeit = 0.4f;
    //public GameObject[] spawnablePowerUps;


    // Start is called before the first frame update
    void Awake()
    {
        //spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
        //spawner = GameObject.FindGameObjectWithTag("SpawnPoint");
        gameManager = GameObject.FindObjectOfType<GameManager>();
    }

    public Vector2 GetSpawnPoint(string FromSide = "")
    {
        BoxCollider2D collider = spawner.GetComponent<BoxCollider2D>();

        GameObject spawnerActive = spawner;

        if(FromSide == "Right")
        {
            collider = spawnerRight.GetComponent<BoxCollider2D>();
            spawnerActive = spawnerRight;
        }
        else if (FromSide == "Left")
        {
            collider = spawnerLeft.GetComponent<BoxCollider2D>();
            spawnerActive = spawnerLeft;
        }
        else if (FromSide == "Bottom")
        {
            collider = spawnerLeft.GetComponent<BoxCollider2D>();
            spawnerActive = spawnerBottom;
        }
        Vector2 colliderPos = new Vector2 (spawnerActive.transform.position.x, spawnerActive.transform.position.y) + collider.offset;
        float randomPosX = Random.Range(colliderPos.x - spawnerActive.transform.localScale.x / 2, colliderPos.x + spawnerActive.transform.localScale.x / 2);
        float randomPosY = Random.Range(colliderPos.y - spawnerActive.transform.localScale.y / 2, colliderPos.y + spawnerActive.transform.localScale.y / 2);

        // Random position within this transform
        Vector2 rndPosWithin = new Vector2(randomPosX, randomPosY);

        return rndPosWithin;
    }

     
    public Vector2 GetSpawnPointInViewport()
    {
        float spawnY = Random.Range
                (Camera.main.ScreenToWorldPoint(new Vector2(0, 0 + (gameManager.UILevelBar.GetComponent<RectTransform>().sizeDelta.y * gameManager.UICanvas.scaleFactor) +10)).y, Camera.main.ScreenToWorldPoint(new Vector2(0, Screen.height - 10)).y);
        float spawnX = Random.Range
            (Camera.main.ScreenToWorldPoint(new Vector2(10, 0)).x, Camera.main.ScreenToWorldPoint(new Vector2(Screen.width -10, 0)).x);

        Vector2 spawnPosition = new Vector2(spawnX, spawnY);
        return spawnPosition;
    }

    public void SpawnPowerUp(Vector2 spawnPos, GameObject[] powerUpPrefabs)
    {
        //GameObject spawnpoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject prefab = powerUpPrefabs[Random.Range(0, powerUpPrefabs.Length)];

        //GameObject prefab = spawnablePowerUps[Random.Range(6, spawnablePowerUps.Length)];

        GameObject g = Instantiate(prefab, spawnPos, Quaternion.identity);

        Rigidbody2D inst_rb = g.GetComponent<Rigidbody2D>();

        float x = Random.Range(-200, 200);
        float y = Random.Range(-200, 200);

        if (x == 0)
        {
            x = 200;
        }
        if (y == 0)
        {
            y = 200;
        }

        inst_rb.AddForce(new Vector2(x, y));

        inst_rb.AddTorque(Random.Range(-200, 200));

        Destroy(g, Random.Range(10, 20));
    }
    
    // Update is called once per frame
    void FixedUpdate()
    {
        if (gameManager.gameState == GameState.Level)
        {
            /*
            float rnd = Random.Range(0f, 50f);

            if (rnd < (basePowerUpWahrscheinlichkeit / gameManager.SchwierigkeitsModifikator))
            {
                SpawnPowerUp(GetSpawnPointInViewport(), spawnablePowerUps);
            }
            */
        }
    }
}
