using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallCollider : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();


            if (gameObject.name == "WallRight")
            {
                if (rb.velocity.x > 0)
                {
                    Vector2 v2 = new Vector2(0, rb.velocity.y);
                    rb.velocity = v2;
                }
            }
            if (gameObject.name == "WallLeft")
            {
                if (rb.velocity.x < 0)
                {
                    Vector2 v2 = new Vector2(0, rb.velocity.y);
                    rb.velocity = v2;
                }

            }
            if (gameObject.name == "WallTop")
            {
                if (rb.velocity.y > 0)
                {
                    Vector2 v2 = new Vector2(rb.velocity.x, 0);
                    rb.velocity = v2;
                }

            }
            if (gameObject.name == "WallBottom")
            {
                if (rb.velocity.y < 0)
                {
                    Vector2 v2 = new Vector2(rb.velocity.x, 0);
                    rb.velocity = v2;
                }

            }
        }
    }
}
