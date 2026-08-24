using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;

public class SliderBrillo : MonoBehaviour
{
    public PostProcessProfile profbrillo;
    public PostProcessLayer layerBrillo;
    public Slider brilloSlider;
    public GameObject objetoBrillo;
    AutoExposure exposure;
    public const string claveBrillo = "Brillo";
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(objetoBrillo);
        profbrillo.TryGetSettings(out exposure);
        
        if (PlayerPrefs.HasKey(claveBrillo))
        {
            float nuevoBrillo = PlayerPrefs.GetFloat(claveBrillo);
            Debug.Log("Brillo guardado:" + nuevoBrillo );
            //brilloSlider.value = nuevoBrillo;
            //AjustaBrillo(nuevoBrillo);
        }
        else
        {
            Debug.Log("No guardó brillo");
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
            PlayerPrefs.SetFloat(claveBrillo, valor);
            PlayerPrefs.Save();
        }
        else
        {
            exposure.keyValue.value = .05f;
            PlayerPrefs.SetFloat(claveBrillo, valor);
            PlayerPrefs.Save();
        }

    }
}
