using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum BossExtensionState
{
    Open,
    Closed
}

public class BossExtensionController : MonoBehaviour
{
    public BossController boss;

    private GameManager gameManager;
    private LevelManager levelManager;
    private ScenarioManager scenarioManager;

    private Animator animator; 
    private Rigidbody2D rb;

    public GameObject[] laserBarriers;
    
    public float rotationSpeed = 1f;

    private BossExtensionState state = BossExtensionState.Closed;

    private bool isInTransition = false;

    private float TransitionTimer = 0f;
    private float TransitionTime = 1f;

    private float ChangeStateTimer = 0f;
    public float ChangeStateTime = 5f;

    private bool isActive = false;    

    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        levelManager = FindObjectOfType<LevelManager>();
        scenarioManager = FindObjectOfType<ScenarioManager>();

        rb = GetComponent<Rigidbody2D>();

        animator = GameObject.Find("Boss4ExtensionAnimation").GetComponent<Animator>();
        animator.enabled = false;
                
        rb.AddTorque(rotationSpeed * gameManager.SchwierigkeitsModifikator * rb.mass);

        foreach (GameObject g in laserBarriers)
        {
            g.GetComponent<BulletController>().Schaden = (int)((float)g.GetComponent<BulletController>().Schaden * gameManager.SchwierigkeitsModifikator);
            g.transform.RotateAround(transform.position, new Vector3(0, 1, 0), rotationSpeed * Time.deltaTime);
        }
    }


    private void switchChildren(GameObject g, bool value)
    {
        g.GetComponent<SpriteRenderer>().enabled = value;
        g.GetComponent<Collider2D>().enabled = value;
        g.GetComponent<Animator>().enabled = value;
    }

    private void FixedUpdate()
    {
        if (transform.position.y > -0.75f)
        {
            rb.velocity = new Vector2(0, -1f);
        }
        else if (transform.position.y <= -0.75f)
        {
            rb.velocity = new Vector2(0, 0);            

            isActive = true;
            animator.enabled = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        
    }

    void Update()
    {
        if (isActive == true)
        {
            ChangeStateTimer += Time.deltaTime;

            if (ChangeStateTimer >= ChangeStateTime)
            {
                switch (state)
                {
                    case BossExtensionState.Closed:
                        {

                            animator.SetBool("Open", true);
                            animator.SetBool("Close", false);
                            state = BossExtensionState.Open;
                            isInTransition = true;

                            break;
                        }
                    case BossExtensionState.Open:
                        {
                            if (boss.bossState == BossState.normal)
                            {
                                animator.SetBool("Open", false);
                                animator.SetBool("Close", true);
                                state = BossExtensionState.Closed;
                                isInTransition = true;
                            }
                            break;
                        }
                }
                ChangeStateTimer = 0f;
            }



            if (isInTransition == true)
            {
                TransitionTimer += Time.deltaTime;

                if (TransitionTimer >= TransitionTime)
                {
                    TransitionTimer = 0f;
                    isInTransition = false;
                }

            }

            foreach (GameObject g in laserBarriers)
            {
                if (state == BossExtensionState.Open && (isInTransition == false))
                {
                    switchChildren(g, true);
                }
                else if (state == BossExtensionState.Closed)
                {
                    switchChildren(g, false);
                }
            }
        }
    }
}
