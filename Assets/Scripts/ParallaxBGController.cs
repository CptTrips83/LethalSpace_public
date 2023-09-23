using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxBGController : MonoBehaviour
{
    public float scrollSpeed = 0.1f;

    private MeshRenderer meshRenderer;

    private Vector2 savedOffset;

    // Start is called before the first frame update
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();

        savedOffset = meshRenderer.sharedMaterial.GetTextureOffset("_MainTex");
    }

    // Update is called once per frame
    void Update()
    {
        float y = Time.time * scrollSpeed;

        Vector2 offset = new Vector2(0, y);

        meshRenderer.sharedMaterial.SetTextureOffset("_MainTex", offset);
    }

    private void OnDisable()
    {
        meshRenderer.sharedMaterial.SetTextureOffset("_MainTex", savedOffset);
    }
}
