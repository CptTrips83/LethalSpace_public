using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BossState
{
    normal,
    sauer
}

public class BossController : MonoBehaviour
{
    public Soundtrack soundtrack = Soundtrack.Track9;

    ShipController shipController;
    LevelManager levelManager;
    Rigidbody2D rb;
    PlayerController playerController;
    GameManager gameManager;
    ScenarioManager scenarioManager;

    public bool isActive = false;
    public bool isInViewport = false;

    public BossState bossState = BossState.normal;

    // Start is called before the first frame update
    void Awake()
    {
        shipController = gameObject.GetComponent<ShipController>();
        levelManager = FindObjectOfType<LevelManager>();
        rb = gameObject.GetComponent<Rigidbody2D>();
        playerController = FindObjectOfType<PlayerController>();
        gameManager = FindObjectOfType<GameManager>();
        scenarioManager = FindObjectOfType<ScenarioManager>();

        if(gameObject.name == "Boss 2(Clone)")
        {
            rb.AddTorque(30);
        }
    }

    public WeaponController[] GetWeaponController()
    {
        List<WeaponController> list = new List<WeaponController>();

        foreach(WeaponController g in GetComponentsInChildren<WeaponController>())
        {
            list.Add(g);
        }

        return list.ToArray();
    }

    public void changeState(BossState state)
    {
        if(bossState != state)
        {
            bossState = BossState.sauer;
            if(state == BossState.sauer)
            {
                foreach(WeaponController weapon in GetWeaponController())
                {
                    weapon.waffenModus = WeaponMode.Normal;
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.name == "WallTop")
        {
            isInViewport = true;
        }
    }

    private void OnDisable()
    {
        if (levelManager != null && shipController.curRumpf <= 0 && isActive == true)
        {
            levelManager.GetComponent<ScenarioManager>().BossBesiegt();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {        
        if(gameObject.name == "Boss 2(Clone)" || gameObject.name == "Boss 4(Clone)")
        {
            rb.freezeRotation = false;
        }

        if(shipController.curRumpf < (shipController.Rumpf / 3))
        {
            changeState(BossState.sauer);
        }

        if (isInViewport == true && shipController.shipState == ShipState.Active)
        {
            shipController.Shoot();
        }
        if (gameObject.name != "Boss 3(Clone)" && gameObject.name != "Boss 4(Clone)")
        {
            if (isInViewport == false || transform.position.y > 3)
            {
                rb.velocity = new Vector2(0, -1);
            }
            else if (isInViewport == true && transform.position.y <= 3)
            {
                if (isActive == false)
                {
                    int rnd = Random.Range(1, 2);

                    float x = shipController.MaximaleGeschwindigkeit;

                    if (rnd == 2)
                    {
                        x *= (-1);
                    }

                    rb.velocity = new Vector2(x, 0);
                    isActive = true;
                }
            }
        }
        else if (gameObject.name == "Boss 3(Clone)" || gameObject.name == "Boss 4(Clone)")
        {
            if (isInViewport == false || transform.position.y > 4)
            {
                rb.velocity = new Vector2(0, -1);
            }
            else if (isInViewport == true && transform.position.y <= 4)
            {
                if (isActive == false)
                {
                    rb.velocity = new Vector2(0, 0);
                    isActive = true;
                }
            }
        }
        Vector3 v3 = new Vector3(transform.position.x, transform.position.y, 0);

        Vector2 v2 = Camera.main.WorldToScreenPoint(v3);

        if (gameObject.name != "Boss 3(Clone)" && gameObject.name != "Boss 4(Clone)")
        {
            if (isInViewport == true && v2.x <= (GetComponent<SpriteRenderer>().sprite.rect.width / 2))
            {
                rb.velocity = new Vector2(shipController.MaximaleGeschwindigkeit, 0);
            }
            if (isInViewport == true && v2.x >= Camera.main.pixelWidth - (GetComponent<SpriteRenderer>().sprite.rect.width / 2))
            {
                rb.velocity = new Vector2(shipController.MaximaleGeschwindigkeit * (-1), 0);
            }
            if (shipController.shipState == ShipState.Destroyed)
            {
                rb.velocity = new Vector2(0, 0);
            }
        }
        else if (gameObject.name == "Boss 3(Clone)")
        {

        }

        
    }
}
