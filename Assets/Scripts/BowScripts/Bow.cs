using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow : MonoBehaviour
{

    public GameObject arrow;
    public Sprite bowDrawn;
    public Sprite bowRest;

    private Arrow arrowScript;
    private new SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        arrowScript = arrow.GetComponent<Arrow>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {

        if (!arrowScript.shot)
        {
            transform.rotation = arrow.transform.rotation;
            spriteRenderer.sprite = bowDrawn;
        } else
        {
            spriteRenderer.sprite = bowRest;
        }
    }
}
