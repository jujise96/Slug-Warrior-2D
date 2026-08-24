using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class RecordsScript : MonoBehaviour
{
    public TMP_Text tiempo1;

     public TMP_Text tiempo2;

      public TMP_Text tiempo3;

      private float puesto1;
      private float puesto2;
      private float puesto3;


    
    public float timer;
    private Transform entryContainer;
    private Transform entryTemplate;

    
    
    // Start is called before the first frame update
    void Start()
    {
        
        //entryContainer = transform.Find("TemplateContainer");
        //entryTemplate = transform.Find("TemplateRecord");
        
        //entryTemplate.gameObject.SetActive(false);
        //float altura = 236.1638F;
        //for (int i = 0; i < 10; i++)
        //{
            //Transform entryTransform = Instantiate(entryTemplate, entryContainer);
            //RectTransform entryRectTransform = entryTransform.GetComponent<RectTransform>();
            //entryRectTransform.anchoredPosition = new Vector3(528.0266F, -altura * i, 21.42527F);
            //entryTransform.gameObject.SetActive(true);
        }
    

    // Update is called once per frame
    void Update()
    {
        timer = PlayerPrefs.GetFloat("UltimoTiempo");
        tiempo1.text = timer.ToString();
        
        if(float.TryParse(tiempo1.text, out puesto1))
        {
            if (timer>puesto1)
            {
                tiempo1.text = timer.ToString();
            }
        
        }

        if(float.TryParse(tiempo2.text, out puesto2))
        {
            if (timer>puesto2)
            {
                tiempo2.text = timer.ToString();
            }
        
        }

        if(float.TryParse(tiempo3.text, out puesto3))
        {
            if (timer>puesto3)
            {
                tiempo3.text = timer.ToString();
            }
        
        }
        
        
    }

    

    public void Volver()
    {
        SceneManager.LoadScene("MenuPrincipal");
    }
    
}
