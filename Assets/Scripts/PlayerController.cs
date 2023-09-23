using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int highscore = 1;
    public int score = 0;

    public TextMesh powerUpText;
    public float textSichtbar = 1f;
    private Coroutine powerUpTextCoroutine;

    [HideInInspector]
    public bool isHoldFire = false;
    
    private ShipController shipController;
    private GameManager gameManager;
    private LevelManager levelManager;

    private Rigidbody2D rb;

    public void showPowerUpText(string value)
    {
        if(powerUpText != null)
        {
            if(powerUpTextCoroutine != null)
            {
                StopCoroutine(powerUpTextCoroutine);
            }

            powerUpText.text = value;
            powerUpText.GetComponent<MeshRenderer>().enabled = true;

            powerUpTextCoroutine = StartCoroutine(hidePowerUpText());
        }
    }

    public IEnumerator hidePowerUpText()
    {
        yield return new WaitForSeconds(textSichtbar);

        powerUpText.GetComponent<MeshRenderer>().enabled = false;
    }

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        levelManager = FindObjectOfType<LevelManager>();
        shipController = GetComponent<ShipController>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    void Start()
    {     
        shipController.switchWeapon("Weapon2", false); 
        shipController.switchWeapon("Weapon3", false);
        shipController.switchWeapon("Weapon4", false);
        powerUpText.GetComponent<MeshRenderer>().enabled = false;
    }

    private void OnDisable()
    {
        PlayerPrefs.SetInt("Highscore", highscore);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(score > highscore)
        {
            highscore = score;
        }

        if (isHoldFire == false)
        {
            shipController.Shoot();
        }

        // Tastatur-Eingabe

        Vector2 v2 = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        Vector2 velo = rb.velocity;

        float x;
        float y;

        if(velo.x < 0 && v2.x > 0)
        {
            x = v2.x + shipController.Fluggeschick;
        }
        else if(velo.x > 0 && v2.x < 0)
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

        Vector2 velocity = new Vector2(x * shipController.Beschleunigung, y * shipController.Beschleunigung);

        rb.AddForce(velocity);


        if (Input.GetKeyDown(KeyCode.Space))
        {
            GetComponent<ShipController>().Shoot();
        }

        // Ende Tastatur-Eingabe 
    }
}
