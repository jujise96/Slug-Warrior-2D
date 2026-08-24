using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemigo : MonoBehaviour
{
    public float vida;
    private Animator animator;
    private bool Block = false;
    private float distance;
    private Vector2 direction;
    public int dano;
    public float velocidad;

    private GameObject playerObject;
    private PlayerMovement PM;

    public float tiempoSiguienteAtaque;
    public float tiempoEntreAtaques;
    public float radioGolpe;

    private CapsuleCollider2D capsuleCollider;
    private Rigidbody2D rb;
    //Transform playerTransform;

    private void Start()
    {
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        rb = GetComponent<Rigidbody2D>();

        velocidad = 3;
        animator = GetComponent<Animator>();
        playerObject = GameObject.FindWithTag("Player");
        PM = playerObject.GetComponent<PlayerMovement>();
        tiempoEntreAtaques = 5;
        radioGolpe = 2f;

        switch (PM.getMaxLife())
        {
            case 1:
                vida = 5;
                break;
            case 2:
            case 3:
                vida = 10;
                break;
            default:
                vida = 15;
                break;
        }



        dano = PM.getDano();
    }

    private void Update()
    {
            distance = Vector2.Distance(PM.transformJugador().position, transform.position);
            Debug.Log("La distancia del jugador es: " + distance);

            direction = PM.transformJugador().position - transform.position;
            if (direction.x > 0 && vida > 0)
            {
                transform.localScale = new Vector3(2, 2, 1);
            }
            else if (direction.x < 0 && vida > 0)
            {
                transform.localScale = new Vector3(-2, 2, 1);
            }


            if (distance < 2f && PM.Health > 0 && vida > 0)
            {
                animator.SetBool("Running", false);
                Attacking();
            }
            else if (distance < 5f && PM.Health > 0 && vida > 0)
            {
                animator.SetBool("Running", false);
                Blocking();
            }
            else if (distance < 15f && PM.Health > 0 && vida > 0)
            {
                animator.SetBool("Running", true);
                Perseguir();
            }

    }

    public void Perseguir()
    {
        animator.SetBool("IdleBlock", false);
        transform.position = Vector2.MoveTowards(transform.position, PM.transformJugador().position, velocidad * Time.deltaTime);
    }


    public void TomarDano(float dano)
    {
        dano = PM.getDano();
        if (Block)
        {
            vida -= dano;
            animator.SetTrigger("Block");
            animator.SetBool("IdleBlock", false);

        }
        else if (!Block)
        {
            vida -= (dano + 1);
            animator.SetTrigger("Hurt");
        }
        Debug.Log("Enemigo sufre dano");


        if (vida <= 0)
        {
            Muerte();
        }
    }

    private void Blocking()
    {
        animator.SetBool("IdleBlock", true);

    }

    private void Attacking()
    {
        animator.SetBool("IdleBlock", false);
        if (tiempoSiguienteAtaque > 0)
        {
            tiempoSiguienteAtaque -= Time.deltaTime;
        }

        if (tiempoSiguienteAtaque <= 00)
        {
            animator.SetTrigger("Attack1");

            Collider2D[] objetos = Physics2D.OverlapCircleAll(transform.position, radioGolpe);

            foreach (Collider2D colisionador in objetos)
            {
                if (colisionador.CompareTag("Player"))
                {
                    colisionador.transform.GetComponent<PlayerMovement>().Hit();
                }
            }

            tiempoSiguienteAtaque = tiempoEntreAtaques;
        }
    }



    private void Muerte()
    {
        animator.SetTrigger("Death");
        animator.SetBool("IdleBlock", false);
        Debug.Log("Enemigo Muere");
        Invoke("DesactivarColliderYGravedadDespuesDeUnSegundo", 1f);
        
    }

    void DesactivarColliderYGravedadDespuesDeUnSegundo()
    {
        rb.gravityScale = 0f;
        capsuleCollider.enabled = false;
    }
}
