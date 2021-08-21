// the script works with raycasts.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private GameObject player;
    public bool patrol = true, gaurd = false, clockwise=false;
    public bool moving = true;
    public bool pursuingPlayer = false, goingToLastLoc=false;
    Vector3 target;
    Rigidbody2D rb;
    public Vector3 playerLastPos;
    RaycastHit2D hit;
    float moveSpeed = 2f; // changed bullets to be kinematic
    int layerMask = 1<<8;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("player");
        playerLastPos = this.transform.position;

        rb = this.GetComponent<Rigidbody2D>();
        layerMask = ~layerMask;

    }

    void Update()
    {
        movement();
        playerDetect();
    }

    void movement()
    {
        float dist = Vector3.Distance(player.transform.position, this.transform.position);
        Vector3 dir = player.transform.position - transform.position;
        hit = Physics2D.Raycast(new Vector2(this.transform.position.x, this.transform.position.x), new Vector2(dir.x, dir.y), dist, layerMask);
        Debug.DrawRay(transform.position, dir, Color.red);

        Vector3 fwt = this.transform.TransformDirection(Vector3.right);

        RaycastHit2D hit2 = Physics2D.Raycast(new Vector2(this.transform.position.x, this.transform.position.x), new Vector2(fwt.x, fwt.y), 1f, layerMask);

        Debug.DrawRay (new Vector2(this.transform.position.x, this.transform.position.y), new Vector2(fwt.x, fwt.y), Color.cyan);

        if(moving)
        {
            transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);
        }

        if(patrol)
        {
            Debug.Log("Patrolling normally");
            moveSpeed = 2f;

            if(hit.collider != null)
            {
                if (hit2.collider.gameObject.tag == "walls")
                {
                    if(clockwise == false)
                    {
                        transform.Rotate(0, 0, 90);
                    }
                    else
                    {
                        transform.Rotate(0, 0, -90);
                    }
                }
            }
        }

        if(pursuingPlayer)
        {
            Debug.Log("Pursuing player");
            moveSpeed=3.5f;
            rb.transform.eulerAngles = new Vector3(0,0, Mathf.Atan2((playerLastPos.y - transform.position.y), (playerLastPos.x - transform.position.x)) * Mathf.Rad2Deg);
            // The Euler angles are mainly used to locate a moving Rm x3y3z3

            if(hit.collider.gameObject.tag == "player")
            {
                playerLastPos = player.transform.position;
            }
        }

        if(goingToLastLoc)
        {
            Debug.Log("pursuing player");
            moveSpeed = 3.5f;

            rb.transform.eulerAngles = new Vector3(0,0, Mathf.Atan2((playerLastPos.y - transform.position.y), (playerLastPos.x - transform.position.x)) * Mathf.Rad2Deg);
            if(Vector3.Distance(this.transform.position, playerLastPos) < 1.5f)
            {
                patrol = true;
                goingToLastLoc = false;
            }
        }
    }

    void playerDetect()
    {
        Vector3 pos = this.transform.InverseTransformPoint(player.transform.position);

        if(hit.collider != null)
        {
            if(hit.collider.gameObject.tag == "player" && pos.x > 1.2f && Vector3.Distance(this.transform.position, player.transform.position) < 9)
            {
                patrol = false;
                pursuingPlayer = true;
            }
            else
            {
                if(pursuingPlayer)
                {
                    goingToLastLoc = true;
                    pursuingPlayer = false;
                }
            }
        }
    }
}
