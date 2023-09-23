using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SpawnPoint
{
    Top,
    Left,
    Right,
    Bottom
}

public class ParallaxController : MonoBehaviour
{
    public float minSpeed = 2f;
    public float maxSpeed = 3f;
    public float minRotation = 2f;
    public float maxRotation = 3f;

    public float minScale = 0.5f;
    public float maxScale = 1.5f;

    public SpawnPoint spawnPoint = SpawnPoint.Top;

    Rigidbody2D rb;

    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        rb.velocity = new Vector2(0, (Random.Range(minSpeed, maxSpeed)*(-1)));
        float rotation = 0f;

        rotation = Random.Range(minRotation, maxRotation);

        if(rotation != 0)
        {
            rb.AddTorque(rotation);
        }
        else
        {
            rb.freezeRotation = true;
        }

        if(spawnPoint == SpawnPoint.Bottom)
        {
            rb.rotation = 180;
            rb.velocity = rb.velocity * (-1);
        }
        
        float size = Random.Range(minScale, maxScale);

        transform.localScale = new Vector3(size, size, size);
    }

    // Update is called once per frame
    void OnTriggerExit2D(Collider2D collision)
    {
        switch (collision.gameObject.name)
        {            
            case ("WallBottom"):
                {
                    if (collision.transform.position.y > transform.position.y)
                    {
                        Destroy(gameObject, 2);
                    }
                    break;
                }
            case ("WallTop"):
                {
                    if (collision.transform.position.y < transform.position.y)
                    {
                        Destroy(gameObject, 2);
                    }
                    break;
                }
            case ("WallRight"):
                {
                    if (collision.transform.position.x > transform.position.x)
                    {
                        Destroy(gameObject, 2);
                    }
                    break;
                }
            case ("WallLeft"):
                {
                    if (collision.transform.position.x < transform.position.x)
                    {
                        Destroy(gameObject, 2);
                    }
                    break;
                }
        }
    }
}
