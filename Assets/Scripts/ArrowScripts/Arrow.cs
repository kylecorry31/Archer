using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using MLAgents;
using UnityEngine.UI;

public class Arrow : Agent
{

    public GameObject area;
    private ArcherArea myArea;

    public Text text;

    public float minShotForce = 0.0f;
    public float maxShotForce = 1000.0f;
    public bool shot = false;
    public bool drawn = false;
    public int shotStrength = 0;
    public float shootingAngleIncrement = 0.5f;
    public float shootingAngle = 0;
    public float maxShootingAngle = 90;
    public float minShootingAngle = -90;

    private float startTime;

    public GameObject target;

    private new Rigidbody2D rigidbody2D;

    private Vector3 position;
    private Quaternion rotation;

    // Start is called before the first frame update
    public override void InitializeAgent()
    {
        base.InitializeAgent();
        myArea = area.GetComponent<ArcherArea>();
        rigidbody2D = GetComponent<Rigidbody2D>();
        rigidbody2D.gravityScale = 0;
        position = transform.position;
        rotation = transform.rotation;
    }

    public override void CollectObservations()
    {
        AddVectorObs(myArea.GetTargetDistance() / 100.0f);
        AddVectorObs(shootingAngle / 90.0f);
        AddVectorObs(shotStrength / 100.0f);
        AddVectorObs(shot);
        AddVectorObs(shotStrength == 100);
        AddVectorObs(drawn);
        text.text = string.Format("Strength: {0}%\nAngle: {1}\nReward: {2:0.000}", shotStrength, shootingAngle, GetCumulativeReward());
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        if (transform.position.y < 0)
        {
            AddReward(-0.1f);
            Done();
            return;
        }

        if (Time.fixedTime - startTime > 10.0f)
        {
            AddReward(-0.1f);
            Done();
            return;
        }

        if (!shot)
        {
            transform.rotation = Quaternion.Euler(0, 0, shootingAngle);
            AddReward(-1 / 1000.0f);
        }

        if (vectorAction[0] == 1 && shootingAngle < maxShootingAngle && !shot)
        {
            shootingAngle += shootingAngleIncrement;
        } else if (vectorAction[0] == 2 && shootingAngle > minShootingAngle && !shot)
        {
            shootingAngle -= shootingAngleIncrement;
        }        

        float velocity = Mathf.Abs(rigidbody2D.velocity.x);

        // Build up strength while the mouse button is held
        if (vectorAction[1] == 1 && !shot)
        {
            if (shotStrength < 100)
            {
                shotStrength++;
                AddReward(1 / 1000.0f);
            } else
            {
                AddReward(-1 / 1000.0f);
            }
            drawn = true;
        }

        // When the button is released, fire the arrow at the given strength
        if (!shot && vectorAction[1] == 2)
        {
            AddReward(shotStrength / 400.0f);
            rigidbody2D.gravityScale = 1;
            float force = shotStrength / 100.0f * maxShotForce;
            float forceX = force * Mathf.Cos(shootingAngle * Mathf.Deg2Rad);
            float forceY = force * Mathf.Sin(shootingAngle * Mathf.Deg2Rad);
            rigidbody2D.AddForce(new Vector2(forceX, forceY));
            shot = true;
            drawn = false;
        }

        // Match rotation of arrow to direction of velocity
        if (rigidbody2D.velocity.y != 0)
        {
            float angle = Mathf.Rad2Deg * Mathf.Atan2(rigidbody2D.velocity.y, rigidbody2D.velocity.x); // Mathf.Rad2Deg * Mathf.Atan(rigidbody2D.velocity.y);
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }


    public override void AgentReset()
    {
        shot = false;
        transform.position = position;
        transform.rotation = rotation;
        rigidbody2D.velocity = Vector2.zero;
        rigidbody2D.angularVelocity = 0;
        rigidbody2D.gravityScale = 0;
        shotStrength = 0;
        shootingAngle = 0;
        myArea.MoveTarget();
        startTime = Time.fixedTime;
    }

    public override void AgentOnDone()
    {

    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject == target)
        {
            AddReward(1.0f);
            Done();
        } else
        {
            AddReward(-0.1f);
            Done();
        }
    }
}
