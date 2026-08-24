using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class BatController : MonoBehaviour
{

    public Transform target;

    public float speed;
    public float nextwapointdistance = 3f;
    public bool reachedendofpath = false;
    public Transform enemyGTX;

    Path path;
    int currentWaypoint = 0;
    private Animator animator;

    Seeker seeker;
    Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetTrigger("Despegar");
        animator.SetBool("Volando", true);
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        InvokeRepeating("UpdatePath", 0f, 0.5f);
        seeker.drawGizmos = false;
    }

    void UpdatePath()
    {
        if(seeker.IsDone())
        seeker.StartPath(rb.position, target.position, OnPathComplete);
    }

    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (path == null)
        {
            return;
        }

        if(currentWaypoint >= path.vectorPath.Count)
        {
            reachedendofpath = true;
            return;
        }else
        {
            reachedendofpath = false;
        }

        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        Vector2 force = direction * speed * Time.deltaTime;

        rb.AddForce(force);

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

        if(distance < nextwapointdistance)
        {
            currentWaypoint++;
        }

        if(force.x >= 0.01f)
        {
            transform.localScale = new Vector3(-2f, 2f, 2f);
        } else if (force.x <= -0.01f)
        {
            transform.localScale = new Vector3(2f, 2f, 2f);
        }
    }
}
