using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CheckBox : MonoBehaviour
{
    public int vida;
    // Start is called before the first frame update
    
    void start()
    {
        vida = 3;
        PlayerPrefs.SetInt("Vida", 0);
    }

    void update ()
    {
        Debug.Log("Hola");
    }

    public void HandleInputData(int val)
    {
        if(val == 0) {
             
             Debug.Log("Numero de vidas igual a 3");
             vida = 3;
             PlayerPrefs.SetInt("Vida", 3);
        }else if(val == 1) {
             
             Debug.Log("Numero de vidas igual a 5");
             vida = 5;
             PlayerPrefs.SetInt("Vida", 5);
        }else if(val == 2) {
             
             Debug.Log("Numero de vidas igual a 7");
             vida = 7;
             PlayerPrefs.SetInt("Vida", 7);
        }
    }
}
