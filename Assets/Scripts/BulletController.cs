using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    private GameManager gameManager;

    [Header("Schuss Eigenschaften")]
    [Tooltip("Die Geschwindigkeit der Kugel")]
    public Vector2 Geschwindigkeit;
    [Tooltip("Lebensdauer der Kugel in Sekunden")]
    [Range(0.1f, 10f)]public float Lebensdauer;

    public bool isImmortal = false;

    public int Schaden;
    [HideInInspector]
    public TargetMode targetMode = TargetMode.Normal;
    [HideInInspector]
    public BulletMode schussMode = BulletMode.Normal;

    public Vector2[] shrapnelDirections;

    public BulletController prefabSchrapnel = null;

    private float curLebensdauer = 0f;

    private bool isMakingDamage = true;

    [Tooltip("Interval in dem Schaden verursacht wird. Nur bei isImmortal = true")]
    public float hitTime = 1f;

    private float hitTimer = 0f;


    public bool IsMakingdamage
    {
        get
        {
            return isMakingDamage;
        }
        set
        {
            isMakingDamage = value;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        if (isImmortal == false)
        {   
            Vector2 G;

            if (gameObject.tag == "PlayerBullet")
            {
                float y = Geschwindigkeit.y;
                if (y < 0)
                {
                    y *= (-1);
                }
                G = new Vector2(Geschwindigkeit.x, y);
            }
            else if (gameObject.tag == "EnemyBullet")
            {
                float y = Geschwindigkeit.y;

                if (y > 0 && targetMode == TargetMode.Normal)
                {
                    y *= (-1);
                }

                G = new Vector2(Geschwindigkeit.x, y);
            }
            else
            {
                G = new Vector2(Geschwindigkeit.x, Geschwindigkeit.y);
            }

            if (schussMode != BulletMode.ShrapnelBullets)
                GetComponent<Rigidbody2D>().AddForce(G);


            Destroy(gameObject, Lebensdauer);
        }
        else
        {
            Schaden = Mathf.RoundToInt((float)Schaden * gameManager.SchwierigkeitsModifikator);

            if(Schaden < 1)
            {
                Schaden = 1;
            }

        }
    }

    private void SpawnShrapnel()
    {
        if (prefabSchrapnel != null)
        {
            if (gameManager.PointIsInViewport(transform.position) == true && schussMode == BulletMode.Shrapnel)
            {
                List<Vector2> directions = new List<Vector2>();

                directions.Add(new Vector2(1, -1));
                directions.Add(new Vector2(-1, -1));
                directions.Add(new Vector2(0, -1));
                directions.Add(new Vector2(1, 0));
                directions.Add(new Vector2(-1, 0));
                directions.Add(new Vector2(1, 1));
                directions.Add(new Vector2(-1, 1));
                directions.Add(new Vector2(0, 1));

                for (int a = 0; a < 8; a++)
                {
                    GameObject g = Instantiate(prefabSchrapnel.gameObject, transform.position, Quaternion.identity);

                    g.GetComponent<Rigidbody2D>().freezeRotation = false;

                    g.GetComponent<Rigidbody2D>().freezeRotation = true;

                    g.tag = "EnemyBullet";

                    g.SetActive(true);

                    BulletController bullet = g.GetComponent<BulletController>();

                    float tempSpeed = 1;

                    if (Geschwindigkeit.x < 0)
                    {
                        Geschwindigkeit.x *= -1;
                    }
                    if (Geschwindigkeit.y < 0)
                    {
                        Geschwindigkeit.y *= -1;
                    }

                    if (Geschwindigkeit.y > Geschwindigkeit.x)
                    {
                        tempSpeed = Geschwindigkeit.y;
                    }
                    else
                    {
                        tempSpeed = Geschwindigkeit.x;
                    }

                    Vector2 v2 = directions[a];

                    g.name += "(shrapnel)";

                    int MinSchaden = 1;

                    switch(gameManager.schwierigkeit)
                    {
                        case Schwierigkeit.Easy:
                            {
                                MinSchaden = 1;
                                break;
                            }
                        case Schwierigkeit.Medium:
                            {
                                MinSchaden = 2;
                                break;
                            }
                        case Schwierigkeit.Hard:
                            {
                                MinSchaden = 3;
                                break;
                            }
                    }

                    bullet.schussMode = BulletMode.ShrapnelBullets;
                    bullet.Schaden = (Schaden / 8);
                    if (bullet.Schaden <= MinSchaden)
                    {
                        bullet.Schaden = MinSchaden;
                    }

                    if (v2.x == v2.y || v2.x == v2.y * (-1))
                    {
                        tempSpeed /= 2;
                    }

                    bullet.Lebensdauer = (Lebensdauer * 3);
                    bullet.Geschwindigkeit = (v2 * tempSpeed) / 2;
                    bullet.transform.localScale *= 0.8f;

                    Rigidbody2D rb = g.GetComponent<Rigidbody2D>();

                    rb.AddForce(bullet.Geschwindigkeit);

                    Destroy(g, bullet.Lebensdauer);
                }
            }
        }
    }

    private void OnDisable()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(isImmortal == true)
        {
            if (hitTimer < hitTime)
            {
                if (isMakingDamage == false)
                {
                    hitTimer += Time.deltaTime;
                }
            }
            else
            {
                hitTimer = 0f;
                isMakingDamage = true;
            }
        }

        if (curLebensdauer <= Lebensdauer - 0.1f)
        {
            curLebensdauer += Time.deltaTime;
        }
        else if(curLebensdauer > Lebensdauer - 0.1f)
        {
            curLebensdauer = 0;
            SpawnShrapnel();
        }
    }
}
