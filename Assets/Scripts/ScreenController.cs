using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenController : MonoBehaviour
{
    private GameManager gameManager;

    public float distance = 10;
    public float goDepth = 4;
    Vector3 v3ViewPort;
    Vector3 v3BottomLeft;
    Vector3 v3TopRight;

    public float colDepth = 2f;
    public float zPosition = 0f;
    private Vector2 screenSize;
    public Transform topCollider;
    public Transform bottomCollider;
    public Transform leftCollider;
    public Transform rightCollider;
    public Transform spawner;
    public Transform ParallaxBG;

    private Vector3 cameraPos;
    // Use this for initialization
    void Start()
    {   
        gameManager = FindObjectOfType<GameManager>();

        //Make them the child of whatever object this script is on, preferably on the Camera so the objects move with the camera without extra scripting
        topCollider.parent = transform;
        bottomCollider.parent = transform;
        rightCollider.parent = transform;
        leftCollider.parent = transform;
        spawner.parent = transform;

        RectTransform UIRectTransform = gameManager.UILevelBar.GetComponent<RectTransform>();

        Vector2 UISize = new Vector2((UIRectTransform.sizeDelta.x * gameManager.UICanvas.scaleFactor), (UIRectTransform.sizeDelta.y * gameManager.UICanvas.scaleFactor));

        Vector2 UIWorldSize = Camera.main.ScreenToWorldPoint(UISize);

        //Generate world space point information for position and scale calculations
        cameraPos = Camera.main.transform.position;
        screenSize.x = Vector2.Distance(Camera.main.ScreenToWorldPoint(new Vector2(0, 0)), Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, 0))) * 0.5f;
        screenSize.y = Vector2.Distance(Camera.main.ScreenToWorldPoint(new Vector2(0, 0)), Camera.main.ScreenToWorldPoint(new Vector2(0, Screen.height))) * 0.5f;

        //Change our scale and positions to match the edges of the screen...   
        rightCollider.localScale = new Vector3(colDepth, screenSize.y, colDepth);
        rightCollider.position = new Vector3(cameraPos.x + screenSize.x + (rightCollider.localScale.x * 0.5f), cameraPos.y, zPosition);
        leftCollider.localScale = new Vector3(colDepth, screenSize.y, colDepth);
        leftCollider.position = new Vector3(cameraPos.x - screenSize.x - (leftCollider.localScale.x * 0.5f), cameraPos.y, zPosition);
        topCollider.localScale = new Vector3(screenSize.x, colDepth, colDepth);
        topCollider.position = new Vector3(cameraPos.x, cameraPos.y + screenSize.y + (topCollider.localScale.y * 0.5f), zPosition);
        bottomCollider.localScale = new Vector3(screenSize.x, colDepth, colDepth);
        bottomCollider.position = new Vector3(cameraPos.x, cameraPos.y - (screenSize.y) - (bottomCollider.localScale.y * 0.5f) - (UIWorldSize.y + bottomCollider.localScale.y), zPosition);
        spawner.localScale = new Vector3(screenSize.x * 2, colDepth, colDepth);
        spawner.position = new Vector3(cameraPos.x, cameraPos.y + screenSize.y + (spawner.localScale.y * 5) + (topCollider.localScale.y * 0.01f), zPosition);

        distance -= (goDepth * 0.5f);

        v3ViewPort.Set(0, 0, distance);
        v3BottomLeft = Camera.main.ViewportToWorldPoint(v3ViewPort);
        v3ViewPort.Set(1, 1, distance);
        v3TopRight = Camera.main.ViewportToWorldPoint(v3ViewPort);

        ParallaxBG.localScale = new Vector3(v3BottomLeft.x - v3TopRight.x, v3BottomLeft.y - v3TopRight.y, goDepth);
    }
}
