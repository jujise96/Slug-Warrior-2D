using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class OpcionesMenu : MonoBehaviour
{
    
    public TMP_Dropdown menu;
    private string seleccion;
    // Start is called before the first frame update
    void Start()
    {
           
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Volver1()
    {
        SceneManager.LoadScene("MenuPrincipal");
    }

    public void Volver2()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void OnDropdownValueChanged()
    {
        // Obtener el valor seleccionado del Dropdown
        //seleccion = menu.options[menu.value].text;
        
        // Imprimir el valor seleccionado para verificar
        //Debug.Log("Valor vida: " + seleccion);
    }
    

}
