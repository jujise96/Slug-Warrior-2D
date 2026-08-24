using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
public class TimerRecord : MonoBehaviour
{
    public TMP_Text tiempo;
    private float inicio;
    private bool empiezaCount;
    private float tPasado;
    private int minutos;
    private int segundos;
    private string minutosString;
    private string segundosString;
    // Start is called before the first frame update
    void Start()
    {
        float[] myFloatArray = new float[] {};
        empiezaCount = true;
        inicio = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if(empiezaCount)
        {
            tPasado = Time.time - inicio;

            minutos = (int)(tPasado / 60);
            segundos = (int)(tPasado % 60);

            minutosString = minutos.ToString("00");
            segundosString = segundos.ToString("00");

            tiempo.text = minutosString + ":" + segundosString;
        }
        
    }

    
    public void Finaliza()
    {
        empiezaCount = false;
        SaveTime();
    }

    private void SaveTime()
    {
        float tPasado = Time.time - inicio;
        PlayerPrefs.SetFloat("UltimoTiempo", tPasado);
        Debug.Log("Ultimo Tiempo:" + tPasado);
        estasMuerto();
    }



    private void estasMuerto()
    {
       Invoke("Endgame", 3f);
       
        
    }

    private void Endgame()
    {
        SceneManager.LoadScene("Records");
    }

    

    

    
}
