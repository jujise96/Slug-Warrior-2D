using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PausaEscena : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            reanudar1();
        }
        
    }

    public void salir()
    {
        SceneManager.LoadScene("MenuPrincipal");
    }

    public void Continuar()
    {
        AsyncOperation AasyncLoad = SceneManager.LoadSceneAsync("SampleScene");

    }

    public void opciones()
    {
        SceneManager.LoadScene("MenuOpciones");
    }

    
    public void reanudar1()
    {
        SceneManager.LoadScene("SampleScene");
    }


    
}
