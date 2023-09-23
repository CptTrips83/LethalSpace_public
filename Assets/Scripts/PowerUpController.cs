using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PowerUpType
{
    Buff,
    Weapon2,
    Weapon3,
    Weapon4
}

public enum PowerUpModifier
{
    none,
    pilotTreffsicherheit,
    pilotWahrnehmung,
    pilotFluggeschick,
    pilotTechnik,
    pilotTechnikZeit,
    shipBeschleunigung,
    shipMaximaleGeschwindigkeit,
    shipSchildEnergie,
    shipSchildAufladung,
    shipRumpf,
    weaponFeuerrate,
    weaponSchaden
}

public class Modifier
{
    public string powerupText;
    public PowerUpType powerupType;
    public PowerUpModifier wert;
    public float Wirkung;
    public float MaxWirkung;
    public float Dauer;
    public bool IsDauerhaft;

    public float timeRunning;

    public float originalWert;

    public Modifier()
    {
        timeRunning = 0;
    }

    public void Update()
    {
        if (IsDauerhaft == false)
        {
            timeRunning += Time.deltaTime;
        }
    }
}

public class PowerUpController : MonoBehaviour
{
    public PowerUpType powerupType = PowerUpType.Buff;

    private Rigidbody2D rb;
    private ScenarioManager scenarioManager;
    private GameManager gameManager;

    [Tooltip("Anzeigetext des PowerUps")]
    public string powerUpText;
    [Tooltip("Dauer des PowerUps. Bei 0 ist die Wirkung dauerhaft")]
    public float Dauer = 10;
    [Tooltip("Die Wirkung auf den modifizierten Wert")]
    [Range(0,500)]public float Wirkung = 2;
    [Tooltip("Die Wirkung auf den modifizierten Wert")]
    [Range(0, 1000)] public float MaxWirkung = 2;
    [Tooltip("Der zu modifizierende Wert")]
    public PowerUpModifier Wert;

    // Start is called before the first frame update
    void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        rb = GetComponent<Rigidbody2D>();
        scenarioManager = FindObjectOfType<ScenarioManager>();
        scenarioManager.powerUpsOnScreen++;
    }

    private Modifier GenerateModifier()
    {
        Modifier m = new Modifier();

        m.powerupText = powerUpText;
        m.powerupType = powerupType;
        m.wert = Wert;
        m.Wirkung = Wirkung;
        m.MaxWirkung = MaxWirkung;
        m.Dauer = Dauer;

        if(Dauer <= 0)
        {
            m.IsDauerhaft = true;
        }
        else
        {
            m.IsDauerhaft = false;
        }


        return m;
    }

    private void OnDestroy()
    {
        scenarioManager.powerUpsOnScreen--;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Wall")
        {
            Vector2 v2 = rb.velocity;

            if(collision.name == "WallLeft" || collision.name == "WallRight")
            {
                v2 = new Vector2(v2.x * (-1), v2.y);
            }
            if (collision.name == "WallTop" || collision.name == "WallBottom")
            {
                v2 = new Vector2(v2.x, v2.y * (-1));
            }

            rb.velocity = v2;
        }
        if (collision.tag == "Player")
        {
            if (gameManager.gameState == GameState.Level)
            {
                Modifier mod = GenerateModifier();

                collision.GetComponent<ShipController>().Modify(mod);

                Destroy(gameObject);
            }
        }
    }
}
