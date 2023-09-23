using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SpawnMode
{
    Default,
    FromSide
}

public enum MovementMode
{
    MoveStraightDown,
    MoveHorizontalToLeft,
    MoveHorizontalToRight,
    MoveToPlayer,
    MoveToRandom,
    Station,
    Boss
}

public class EnemyController : MonoBehaviour
{
    public int XpGain = 1;
    public int SpawnMenge = 1;
    public int XPGain
    {
        get
        {
            return (int)((float)XpGain * gameManager.SchwierigkeitsModifikator); 
        }
    }

    

    private ShipController shipController;
    private GameManager gameManager;
    private LevelManager levelManager;
    private ScenarioManager scenarioManager;

    private Rigidbody2D rb;

    public MovementMode BewegungsModus = MovementMode.MoveStraightDown;
    public SpawnMode SpawnModus = SpawnMode.Default;

    public bool isInViewport = false;

    [Range(0, 50)]public float Schwierigkeit=0.1f;

    [Range(0, 100)] public float PowerUpWahrscheinlichkeit = 1;
    public GameObject[] spawnablePowerUps;
    

    // Start is called before the first frame update
    void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        levelManager = FindObjectOfType<LevelManager>();
        shipController = GetComponent<ShipController>();
        scenarioManager = FindObjectOfType<ScenarioManager>();

        rb = GetComponent<Rigidbody2D>();

        if(BewegungsModus == MovementMode.MoveToPlayer)
        {
            Vector3 v3 = (GameObject.FindGameObjectWithTag("Player").transform.position - transform.position).normalized;
            Vector2 v2 = new Vector2(v3.x, v3.y);

            rb.velocity = new Vector2(v2.x,v2.y * shipController.Beschleunigung);
        }
        if(BewegungsModus == MovementMode.MoveToRandom)
        {
            Vector2 v2 = new Vector2(Random.Range(-1f,1f), -1 * shipController.Beschleunigung);
            rb.velocity = v2;
        }
        if (BewegungsModus == MovementMode.Station)
        {
            rb.AddTorque(Random.Range(20, 50));
        }
        if(BewegungsModus == MovementMode.MoveHorizontalToLeft || BewegungsModus == MovementMode.MoveHorizontalToRight )
        {
            rb.velocity = (transform.up * shipController.Beschleunigung * (-1));
        }
        scenarioManager.enemiesOnScreen++;
    }

    private void OnDestroy()
    {
        gameManager.ScoreMultiplier += 0.1f;
        scenarioManager.enemiesOnScreen--;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isInViewport == true)
        {
            GetComponent<ShipController>().Shoot();
        }
        if(BewegungsModus == MovementMode.MoveStraightDown || BewegungsModus == MovementMode.Station)
        {
            float speed = shipController.Beschleunigung;

            rb.velocity = new Vector2(0, speed * (-1));
        }  
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (gameObject.tag != "Player")
        {
            switch (collision.gameObject.name)
            {
                case ("WallTop"):
                    {

                        isInViewport = gameManager.PointIsInViewport(transform.position);

                        break;
                    }
                case ("WallBottom"):
                    {
                        if (gameManager.PointIsInViewport(transform.position) == false)
                        {
                            Destroy(gameObject);
                        }
                        break;
                    }
                case ("WallLeft"):
                    {
                        if (gameManager.PointIsInViewport(transform.position) == false)
                        {
                            Destroy(gameObject);
                        }
                        else
                        {
                            isInViewport = true;
                        }
                        break;
                    }
                case ("WallRight"):
                    {
                        if (gameManager.PointIsInViewport(transform.position) == false)
                        {
                            Destroy(gameObject);
                        }
                        else
                        {
                            isInViewport = true;
                        }
                        break;
                    }
            }
        }
    }
}
