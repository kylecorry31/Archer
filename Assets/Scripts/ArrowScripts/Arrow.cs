using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Arrow : MonoBehaviour
{

    public float minShotForce = 0.0f;
    public float maxShotForce = 10000.0f;
    public bool shot = false;
    private bool hit = false;
    public int shotStrength = 0;
    public float shootingAngleIncrement = 0.5f;
    public float shootingAngle = 0;
    public float maxShootingAngle = 90;
    public float minShootingAngle = -90;

    public GameObject target;

    private new Rigidbody2D rigidbody2D;

    public UnityEvent HitTarget;
    public UnityEvent MissTarget;


    private Vector3 position;
    private Quaternion rotation;

    // Start is called before the first frame update
    void Start()
    {
        HitTarget = new UnityEvent();
        MissTarget = new UnityEvent();
        rigidbody2D = GetComponent<Rigidbody2D>();
        rigidbody2D.gravityScale = 0;
        position = transform.position;
        rotation = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (hit)
        {
            rigidbody2D.velocity = Vector2.zero;
            rigidbody2D.angularVelocity = 0;
            rigidbody2D.gravityScale = 0;
            return;
        }

        if (transform.position.y < 0)
        {
            hit = true;
            MissTarget.Invoke();
        }

        if (!shot)
        {
            transform.rotation = Quaternion.Euler(0, 0, shootingAngle);
        }

        if ((Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)) && shootingAngle < maxShootingAngle)
        {
            shootingAngle += shootingAngleIncrement;
        } else if ((Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)) && shootingAngle > minShootingAngle)
        {
            shootingAngle -= shootingAngleIncrement;
        }        

        float velocity = Mathf.Abs(rigidbody2D.velocity.x);

        // Build up strength while the mouse button is held
        if (Input.GetMouseButton(0))
        {
            if (shotStrength < 100)
            {
                shotStrength++;
                // TODO: Display this
                //Debug.Log(shotStrength);
            }
        }

        // When the button is released, fire the arrow at the given strength
        if (!shot && Input.GetMouseButtonUp(0))
        {
            rigidbody2D.gravityScale = 1;
            float force = shotStrength / 100.0f * maxShotForce;
            float forceX = force * Mathf.Cos(shootingAngle * Mathf.Deg2Rad);
            float forceY = force * Mathf.Sin(shootingAngle * Mathf.Deg2Rad);
            rigidbody2D.AddForce(new Vector2(forceX, forceY));
            shot = true;
        }

        // Match rotation of arrow to direction of velocity
        if (rigidbody2D.velocity.y != 0 && !hit)
        {
            float angle = Mathf.Rad2Deg * Mathf.Atan2(rigidbody2D.velocity.y, rigidbody2D.velocity.x); // Mathf.Rad2Deg * Mathf.Atan(rigidbody2D.velocity.y);
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        
    }


    public void reset()
    {
        hit = false;
        shot = false;
        transform.position = position;
        transform.rotation = rotation;
        rigidbody2D.velocity = Vector2.zero;
        rigidbody2D.angularVelocity = 0;
        rigidbody2D.gravityScale = 0;
        shotStrength = 0;
        shootingAngle = 0;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        hit = true;
        if (collision.gameObject == target)
        {
            HitTarget.Invoke();
        } else
        {
            MissTarget.Invoke();
        }
    }
}
