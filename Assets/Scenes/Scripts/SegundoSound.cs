using UnityEngine;
using UnityEngine.UI;

public class GestorDeSonido : MonoBehaviour
{
    
    
    public Slider sliderVolumen;
    
    private AudioSource musica;
    private const string claveVolumenMusica = "VolumenMusica";
    private float volumInicial = 0.5f;

    void Start()
    {
        GameObject objeto1 = GameObject.Find("La Guerrera Valiente");
        musica = objeto1.GetComponent<AudioSource>();
        // Cargar el volumen de la música desde PlayerPrefs
        if (PlayerPrefs.HasKey(claveVolumenMusica))
        {
            float volumenGuardado = PlayerPrefs.GetFloat(claveVolumenMusica);
            setVolumen(volumenGuardado);
        }
        else
        {
            // Si no hay ningún valor guardado, usar el volumen por defecto
            setVolumen(volumInicial);
        }

        // Configurar el slider para que refleje el volumen actual
        sliderVolumen.value = musica.volume;
        // Escuchar los cambios en el slider y ajustar el volumen de la música
        sliderVolumen.onValueChanged.AddListener(setVolumen);
    }

    public void setVolumen(float volumen)
    {
        // Ajustar el volumen del AudioSource de la música
        musica.volume = volumen;

        // Guardar el volumen en PlayerPrefs
        PlayerPrefs.SetFloat(claveVolumenMusica, volumen);
        PlayerPrefs.Save(); // Guardar los cambios
    }
}
