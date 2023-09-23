using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMovementController : MonoBehaviour
{
    GameManager gameManager;
    GameObject player;
    Rigidbody2D rb;


    // Start is called before the first frame update
    void Awake()
    {
        gameManager = GameObject.FindObjectOfType<GameManager>();
        player = GameObject.FindGameObjectWithTag("Player");
        rb = player.GetComponent<Rigidbody2D>();
    }

    private void Movement(Vector2 target)
    {
        Vector2 mouse = Camera.main.ScreenToWorldPoint(target);

        Vector2 playerPos = player.transform.position;

        Vector2 v2 = (mouse - playerPos).normalized;
        Vector2 velo = rb.velocity;

        ShipController shipController = player.GetComponent<ShipController>();

        float x;
        float y;

        if (velo.x < 0 && v2.x > 0)
        {
            x = v2.x + shipController.Fluggeschick;
        }
        else if (velo.x > 0 && v2.x < 0)
        {
            x = v2.x + shipController.Fluggeschick * (-1);
        }
        else
        {
            x = v2.x;
        }

        if (velo.y < 0 && v2.y > 0)
        {
            y = v2.y + shipController.Fluggeschick;
        }
        else if (velo.y > 0 && v2.y < 0)
        {
            y = v2.y + shipController.Fluggeschick * (-1);
        }
        else
        {
            y = v2.y;
        }




        Vector2 direction = new Vector2(x, y);



        float speed = shipController.Beschleunigung;

        player.GetComponent<Rigidbody2D>().AddForce(direction * speed, ForceMode2D.Force);
    }

    

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (gameManager.gameState == GameState.Level)
        {
            bool isPressed = false;
            Vector2 inputPosition = new Vector2(0, 0);
            if (Input.touchSupported)
            {
                if (Input.touches.Length > 0)
                {
                    inputPosition = Input.touches[0].position;
                    isPressed = true;
                }
            }
            else
            {
                if (Input.GetMouseButton(0) == true)
                {
                    inputPosition = Input.mousePosition;
                    isPressed = true;
                }
            }

            //Debug.Log("Position: " + inputPosition.ToString());
            //Debug.Log("Position Screen: " + Camera.main.WorldToScreenPoint(inputPosition).ToString());
            //Debug.Log("Position From Viewport: " + Camera.main.ViewportToWorldPoint(inputPosition).ToString());

            if (isPressed == true && gameManager.PointIsInViewport(Camera.main.ScreenToWorldPoint(inputPosition)))
            {
                Movement(inputPosition);
            }
            else
            {
                rb.velocity = new Vector2(rb.velocity.x * 0.9f, rb.velocity.y * 0.9f);
            }
        }
    }
}
