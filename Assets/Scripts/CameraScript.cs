using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraScript : MonoBehaviour
{

    public GameObject arrow;
    public GameObject target;
    public Text scoreText;
    public Text powerText;
    private Arrow arrowScript;
    private new Camera camera;

    public int score = 0;

    // Start is called before the first frame update
    void Start()
    {
        camera = GetComponent<Camera>();
        arrowScript = arrow.GetComponent<Arrow>();
        arrowScript.HitTarget.AddListener(OnHitTarget);
        arrowScript.MissTarget.AddListener(OnMissTarget);
    }

    void OnHitTarget()
    {
        score++;
        scoreText.text = "Score: " + score;
        StartCoroutine("ResetArrow");
    }

    void OnMissTarget()
    {
        score--;
        scoreText.text = "Score: " + score;
        StartCoroutine("ResetArrow");
    }

    IEnumerator ResetArrow()
    {
        yield return new WaitForSeconds(1);
        target.transform.position = new Vector3(Random.Range(10f, 100f), target.transform.position.y, target.transform.position.z);
        arrowScript.reset();
    }

    // Update is called once per frame
    void LateUpdate()
    {

        powerText.text = "Power: " + arrowScript.shotStrength + "%";

        /*float center = arrow.transform.position.x / 2;
        float dist = arrow.transform.position.x / 2;
        camera.orthographicSize = dist + 5;
        transform.position = new Vector3(center, 5, -10);*/

    }
}
