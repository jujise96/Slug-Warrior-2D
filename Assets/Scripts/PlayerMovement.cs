using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerMovement : MonoBehaviour
{
    public float Speed;
    public float JumpForce;
    public GameObject BulletPrefab;

    private Rigidbody2D Rigidbody2D;
    private Animator Animator;
    private float Horizontal;
    private bool Grounded;
    private bool Sliding;
    private int maxLife = 3;
    public int Health;

    public GameObject corazon1;
    public GameObject corazon2;
    public GameObject corazon3;
    public GameObject corazon4;
    public GameObject corazon5;
    public GameObject corazon6;
    public GameObject corazon7;


    //private Transform controladorGolpe;
    public float radioGolpe;
    public int dano;
    public float tiempoEntreAtaques;
    public float tiempoSiguienteAtaque;
    private bool Atacando = false;

    public int getMaxLife()
    {
        return maxLife;
    }


    void Start()
    {
        corazon1.SetActive(false);
        corazon2.SetActive(false);
        corazon3.SetActive(false);
        corazon4.SetActive(false);
        corazon5.SetActive(false);
        corazon6.SetActive(false);
        corazon7.SetActive(false);


        maxLife = PlayerPrefs.GetInt("Vida");
        Health = maxLife;
        Rigidbody2D = GetComponent<Rigidbody2D>();
        Animator = GetComponent<Animator>();
        Speed = 5;
        JumpForce = 300;
        tiempoEntreAtaques = 1;
        radioGolpe = 2f;
    }

    public int getDano()
    {
        return dano;
    }

    public void setDano(int dano)
    {
        this.dano = dano;
    }

    public Transform transformJugador()
    {
        return transform;
    } 

    // Update is called once per frame
    void Update()
    {
            Debug.Log("Vida Jugador: "+Health);
        controlvida();
        // Movimiento
        if (Health > 0)
            {
            Horizontal = Input.GetAxisRaw("Horizontal");            
            if (Horizontal < 0.0f) transform.localScale = new Vector3(-6.0f, 6.0f, 6.0f);
            else if (Horizontal > 0.0f) transform.localScale = new Vector3(6.0f, 6.0f, 6.0f);

            Animator.SetBool("running", Horizontal != 0.0f);
            }
            Animator.SetBool("grounded", Grounded);
            
            // Detectar Suelo
            // Debug.DrawRay(transform.position, Vector3.down * 0.1f, Color.red);
            if (Physics2D.Raycast(transform.position, Vector3.down, 1f))
            {
                Grounded = true;
            }
            else
            {
                Grounded = false;
            }

            // deslizado
            if ((Health>0 && (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))) && Grounded)
            {
                Slide();
            }

            // Salto
            if ((Health > 0 && (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Space))) && Grounded && !Atacando)
            {
                Jump();
            }

            //ATAQUE
            if (tiempoSiguienteAtaque > 0)
            {
                tiempoSiguienteAtaque -= Time.deltaTime;
            }

            if (Health > 0 && Input.GetButtonDown("Fire1") && tiempoSiguienteAtaque <= 00 && Grounded)
            {
                Golpe();
                tiempoSiguienteAtaque = tiempoEntreAtaques;
            }
    }

    private void FixedUpdate()
    {
        Rigidbody2D.velocity = new Vector2(Horizontal * Speed, Rigidbody2D.velocity.y);
    }

    private void Jump()
    {

        Rigidbody2D.AddForce(Vector2.up * JumpForce);
    }

    private void Slide()
    {

        if (Grounded)
        {
            Animator.SetTrigger("Slide");
        }
    }

    public void FinishAttack()
    {
        Atacando = false;
    }

    private void Golpe()
    {
        
        if (Atacando == false && Grounded==true)
        {
            Atacando = true;
            Animator.SetTrigger("Golpe");
        }
        

        Collider2D[] objetos = Physics2D.OverlapCircleAll(transform.position, radioGolpe);

        foreach (Collider2D colisionador in objetos)
        {
            if (colisionador.CompareTag("Enemy_1"))
            {
                colisionador.transform.GetComponent<Enemigo>().TomarDano(dano);
            }
        }
    }

    public void Hit()
    {
        Animator.SetTrigger("Hurting");
        Health -= 1;
        if (Health <= 0)
        {
            Animator.SetBool("isDead", true);
            Animator.SetTrigger("Dead");
            Debug.Log("Jugador Muere");
        }
    }

    public void heal(int type)
    {
        if (Health < maxLife && type == 1)
        {
            Health += 1;
        }

        if (Health < maxLife && type == 2)
        {
            Health = maxLife;
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        string tagItem = collision.gameObject.tag;
        Debug.Log("Collision con item" + " " + collision.gameObject.tag);

        if (tagItem == "Trampa")
        {
            Hit();
            Debug.Log("Collision con" + " " + collision.gameObject.tag + " " + Health);
        }

        if (tagItem == "Vida_1")
        {
            heal(1);
            Debug.Log("Collision con" + " " + collision.gameObject.tag + " " + Health);
            Destroy(collision.gameObject);

        }

        if (tagItem == "Vida_2")
        {
            heal(2);
            Debug.Log("Collision con" + " " + collision.gameObject.tag + " " + Health);
            Destroy(collision.gameObject);
        }


    }


    private void OnDrawGizmos()
    {
        //Gizmos.color = Color.red;
        //Gizmos.DrawWireSphere(transform.position, radioGolpe);
    }

    public void controlvida()
    {
        if (Health == 0)
        {
            corazon1.SetActive(false);
            corazon2.SetActive(false);
            corazon3.SetActive(false);
            corazon4.SetActive(false);
            corazon5.SetActive(false);
            corazon6.SetActive(false);
            corazon7.SetActive(false);
        }
        else if (Health == 1)
        {
            corazon1.SetActive(true);
            corazon2.SetActive(false);
            corazon3.SetActive(false);
            corazon4.SetActive(false);
            corazon5.SetActive(false);
            corazon6.SetActive(false);
            corazon7.SetActive(false);
        }
        else if (Health == 2)
        {
            corazon1.SetActive(true);
            corazon2.SetActive(true);
            corazon3.SetActive(false);
            corazon4.SetActive(false);
            corazon5.SetActive(false);
            corazon6.SetActive(false);
            corazon7.SetActive(false);
        }
        else if (Health == 3)
        {
            corazon1.SetActive(true);
            corazon2.SetActive(true);
            corazon3.SetActive(true);
            corazon4.SetActive(false);
            corazon5.SetActive(false);
            corazon6.SetActive(false);
            corazon7.SetActive(false);
        }
        else if (Health == 4)
        {
            corazon1.SetActive(true);
            corazon2.SetActive(true);
            corazon3.SetActive(true);
            corazon4.SetActive(true);
            corazon5.SetActive(false);
            corazon6.SetActive(false);
            corazon7.SetActive(false);
        }
        else if (Health == 5)
        {
            corazon1.SetActive(true);
            corazon2.SetActive(true);
            corazon3.SetActive(true);
            corazon4.SetActive(true);
            corazon5.SetActive(true);
            corazon6.SetActive(false);
            corazon7.SetActive(false);
        }
        else if (Health == 6)
        {
            corazon1.SetActive(true);
            corazon2.SetActive(true);
            corazon3.SetActive(true);
            corazon4.SetActive(true);
            corazon5.SetActive(true);
            corazon6.SetActive(true);
            corazon7.SetActive(false);
        }
        else if (Health == 7)
        {
            corazon1.SetActive(true);
            corazon2.SetActive(true);
            corazon3.SetActive(true);
            corazon4.SetActive(true);
            corazon5.SetActive(true);
            corazon6.SetActive(true);
            corazon7.SetActive(true);
        }
    }

}