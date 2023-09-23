using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GameObjectType
{
    Text,
    Sprite
}

public class BlinkController : MonoBehaviour
{
    public GameObjectType type = GameObjectType.Text;
    public float BlinkTime = 1f;

    private float blinkTimer = 0f;
    private bool IsActiv = false;

    // Start is called before the first frame update
    void Start()
    {
            
    }

    public void SwitchComponent(bool stop = false)
    {
        switch (type)
        {
            case GameObjectType.Sprite:
                {
                    bool value = false;

                    SpriteRenderer renderer = GetComponent<SpriteRenderer>();
                    Animator animator = GetComponent<Animator>();

                    if (renderer != null)
                    {
                        if (renderer.enabled == true)
                        {
                            value = false;
                        }
                        else
                        {
                            value = true;
                        }

                        if(stop == true)
                        {
                            value = false;
                        }

                        renderer.enabled = value;

                        if (animator != null)
                        {
                            animator.enabled = value;
                        }
                    }
                    break;
                }
            case GameObjectType.Text:
                {
                    bool value = false;

                    Text text = GetComponent<Text>();
                    Outline outline = GetComponent<Outline>();

                    if (text != null)
                    {
                        if (text.enabled == true)
                        {
                            value = false;
                        }
                        else
                        {
                            value = true;
                        }

                        if (stop == true)
                        {
                            value = false;
                        }

                        text.enabled = value;

                        if (outline != null)
                        {
                            outline.enabled = value;
                        }
                    }
                    break;
                }
        }

    }

    public void StartBlinking()
    {
        if (IsActiv == false)
        {
            blinkTimer = 0f;
            IsActiv = true;
        }
    }

    public void StopBlinking()
    {
        if (IsActiv == true)
        {
            blinkTimer = 0f;
            IsActiv = false;
            SwitchComponent(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(IsActiv == true)
        {
            blinkTimer += Time.deltaTime;

            if(blinkTimer >= BlinkTime)
            {
                SwitchComponent();
                blinkTimer = 0f;
            }
        }
    }
}
