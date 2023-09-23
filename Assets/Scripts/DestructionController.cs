using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructionController : MonoBehaviour
{
    public GameObject[] prefabExplosion;
    public GameObject[] prefabSchutt;

    public AudioClip[] sounds; 

    private ShipController shipController;
    private GameManager gameManager;
    private ScenarioManager scenarioManager;

    public float MaxDauerDestruction = 2f;
    [Range(0.1f, 3)]public float Staerke = 1f;

    private float DauerDestruction = 0f;
    private bool IsSchutt = false;

    public bool IsParallax = false;

    // Start is called before the first frame update
    void Awake()
    {
        if (IsParallax == false)
        {
            shipController = GetComponent<ShipController>();
        }
        gameManager = FindObjectOfType<GameManager>();
        scenarioManager = FindObjectOfType<ScenarioManager>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (IsParallax != true)
        {
            if (shipController.shipState == ShipState.Destroyed)
            {
                DauerDestruction += Time.deltaTime;

                if (DauerDestruction < MaxDauerDestruction)
                {
                    // Explosionen
                    if (prefabExplosion.Length > 0)
                    {
                        float rnd = Random.Range(0.1f, 5);

                        if (rnd < Staerke)
                        {
                            Vector3 v3 = GetSpawnPoint(-1);

                            GameObject p = prefabExplosion[Random.Range(0, prefabExplosion.Length)];

                            GameObject exp = Instantiate(p, v3, Quaternion.identity);
                                                        
                            AudioClip sound = sounds[Random.Range(0, sounds.Length)];

                            gameManager.PlayAudio(sound);
                        }
                    }
                    // Schutt
                    if (prefabSchutt.Length > 0)
                    {
                        if (DauerDestruction > (MaxDauerDestruction / 2))
                        {
                            if (IsSchutt == false)
                            {
                                foreach (SpriteRenderer spriteRenderer in shipController.GetComponentsInChildren<SpriteRenderer>())
                                {
                                    spriteRenderer.enabled = false;
                                }
                                shipController.GetComponent<SpriteRenderer>().enabled = false;

                                for (int a = 0; a < (Staerke * 5); a++)
                                {
                                    Vector2 v2 = GetSpawnPoint();

                                    GameObject p = prefabSchutt[Random.Range(0, prefabSchutt.Length)];

                                    GameObject g = Instantiate(p, gameObject.transform.position, new Quaternion(0, 0, Random.Range(0, 180), 0));

                                    g.GetComponent<SpriteRenderer>().sortingOrder = 0;

                                    g.GetComponent<Rigidbody2D>().AddTorque(Random.Range(-100, 100));

                                    g.GetComponent<Rigidbody2D>().velocity = (new Vector2(Random.Range(-1, 1), Random.Range(-1, 1)));

                                    Destroy(g, (MaxDauerDestruction / 2));
                                }
                                IsSchutt = true;
                            }
                        }
                    }
                }
                else
                {
                    Destroy(gameObject);
                }
            }
        }
        else if (IsParallax == true)
        {
            float rnd = Random.Range(0.1f, 5);

            if (rnd < Staerke)
            {
                Vector3 v3 = GetSpawnPoint(-1);

                GameObject p = prefabExplosion[Random.Range(0, prefabExplosion.Length)];

                GameObject g = Instantiate(p, v3, Quaternion.identity);

                foreach (SpriteRenderer renderer in g.GetComponentsInChildren<SpriteRenderer>())
                {
                    renderer.sortingLayerName = "ParallaxBack";
                    renderer.sortingOrder = 0;
                }

                AudioClip sound = sounds[Random.Range(0, sounds.Length)];

                float minScale = GetComponent<ParallaxController>().minScale;
                float maxScale = GetComponent<ParallaxController>().maxScale;

                float size = Random.Range(minScale, maxScale) / 10;

                g.transform.localScale = new Vector3(size, size, size);

                //gameManager.PlayAudio(sound, 0.01f);
            }
        }
    }

    private Vector3 GetSpawnPoint(float z = 0)
    {
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        Vector2 colliderPos = new Vector2(transform.position.x, transform.position.y) + collider.offset;
        float randomPosX = Random.Range(colliderPos.x - transform.localScale.x / 2, colliderPos.x + transform.localScale.x / 2);
        float randomPosY = Random.Range(colliderPos.y - transform.localScale.y / 2, colliderPos.y + transform.localScale.y / 2);

        // Random position within this transform
        Vector3 rndPosWithin = new Vector3(randomPosX, randomPosY,z);

        return rndPosWithin;
    }
}
