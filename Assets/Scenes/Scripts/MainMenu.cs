using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    
    private GameObject check;

    
    // Start is called before the first frame update
    void Start()
    {
        
        
        
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void opciones()
    {
        SceneManager.LoadScene("MenuOpciones");
    }

    public void records()

    {
        SceneManager.LoadScene("Records");
    }    

    public void QuitGame()

    {
        Application.Quit();
    }

    
}
