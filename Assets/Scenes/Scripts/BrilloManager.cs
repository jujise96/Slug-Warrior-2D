using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;

public class BrilloManager : MonoBehaviour
{
    public GameObject objetoBrillo;
    //public Slider brilloSlide;
    public PostProcessProfile brillo;
    public PostProcessLayer layer;
    private AutoExposure exposure;    
    public static float brightness;
    public const string claveBrillo = "Brillo";
    // Start is called before the first frame update
    void Start()
    {
        //BRILLO
        //BRILLO
        DontDestroyOnLoad(objetoBrillo);
        GameObject objeto1 = GameObject.Find("Brightness");
        brillo.TryGetSettings(out exposure);

        if (PlayerPrefs.HasKey(claveBrillo))
        {
            float nuevoBrillo = PlayerPrefs.GetFloat(claveBrillo);
            AjustaBrillo(nuevoBrillo);
        }
        else
        {
            // Si no hay ningún valor guardado, usar el volumen por defecto
            AjustaBrillo(.05f);
        }
        


        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AjustaBrillo(float valor)
    {
        if(valor != 0)
        {
            exposure.keyValue.value = valor;
        }
        else
        {
            exposure.keyValue.value = .05f;
        }

    }

    
        
}
