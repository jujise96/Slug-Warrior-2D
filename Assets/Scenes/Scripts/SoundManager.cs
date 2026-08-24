using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SoundManager : MonoBehaviour
{
    public Slider selectSonido;
    public AudioSource fuenteAudio;
    // Start is called before the first frame update
    void Start()
    {
        if(!PlayerPrefs.HasKey("musicVolume"))
        {
            PlayerPrefs.SetFloat("musicVolume", 1);
            Load();
        }else
        {
            Load();
        }

        selectSonido.value = fuenteAudio.volume;
        //Listener para el slider
        selectSonido.onValueChanged.AddListener(delegate { volumen(); });
    }

    // Update is called once per frame
    private void volumen()
    {

        float volum = Mathf.Clamp01(selectSonido.value);
        fuenteAudio.volume = volum;
        Save();
    } 

    private void Load()
    {
        selectSonido.value = PlayerPrefs.GetFloat("musicVolume");
    }
    
    private void Save()
    {
        PlayerPrefs.SetFloat("musicVolume", selectSonido.value);
    }

}