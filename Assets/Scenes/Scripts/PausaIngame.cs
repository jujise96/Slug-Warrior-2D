using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PausaIngame : MonoBehaviour
{
    PlayerMovement vidaPlayer;
    TimerRecord sc;
    private GameObject scObject;
    public int vidas;
    private GameObject playerObject;
    private bool pausa;
    public string nombreDeLaNuevaEscena;
    
    // Start is called before the first frame update
    void Start()
    {
        playerObject = GameObject.FindWithTag("Player");
        vidaPlayer = playerObject.GetComponent<PlayerMovement>();
        scObject = GameObject.Find("Escenario");
        sc = scObject.GetComponent<TimerRecord>();

        
        pausa = false;
    }

    // Update is called once per frame
    void Update()
    {
        vidas = vidaPlayer.Health;

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(pausa)
            {
                continuar();    
            }else
            {
                pausar();
            }
        }

        if(vidas <= 0)
        {
            sc.Finaliza();
        }
    }

    void pausar()
    {
        
        pausa = true;
        Debug.Log("Juego pausado");
        AsyncOperation AasyncLoad = SceneManager.LoadSceneAsync("Pausa");
        
    }

    void continuar()
    {
        pausa = false;
        Debug.Log("Juego reanudado");
        SceneManager.LoadScene("SampleScene");
        
    }


    


    
    

    

    

    

    


}
