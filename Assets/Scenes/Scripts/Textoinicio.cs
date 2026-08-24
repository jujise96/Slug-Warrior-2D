using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Textoinicio : MonoBehaviour
{
    public int vida;
    public GameObject musica;
    public GameObject brillo;
    //VOLUMEN
    private const string claveVolumenMusica = "VolumenMusica";
    private float volumenPorDefecto = 0.5f;
    private AudioSource musicaAudioSource;
    // Start is called before the first frame update
    void Start()
    {
        vida = 5;
        PlayerPrefs.SetInt("Vida", vida);
        //VOLUMEN
        musicaAudioSource = musica.GetComponent<AudioSource>();
        DontDestroyOnLoad(musica);
        if (PlayerPrefs.HasKey(claveVolumenMusica))
        {
            float volumenGuardado = PlayerPrefs.GetFloat(claveVolumenMusica);
            SetVolumenMusica(volumenGuardado);
        }
        else
        {
            // Si no hay ningún valor guardado, usar el volumen por defecto
            SetVolumenMusica(volumenPorDefecto);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
       if(Input.GetKeyDown(KeyCode.Return))
       {
            SceneManager.LoadScene("BrilloSelect");
       } 
    }

    public void SetVolumenMusica(float volumen)
    {
        // Ajustar el volumen del AudioSource de la música
        musicaAudioSource.volume = volumen;

        // Guardar el volumen en PlayerPrefs
        PlayerPrefs.SetFloat(claveVolumenMusica, volumen);
        PlayerPrefs.Save(); // Guardar los cambios
    }

    
}
