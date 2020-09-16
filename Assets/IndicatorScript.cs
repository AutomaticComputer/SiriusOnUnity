using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndicatorScript : MonoBehaviour
{
    [SerializeField]
    private Material onMaterial, offMaterial;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void on()
    {
        gameObject.GetComponent<Renderer>().material = onMaterial;
    }

    public void off()
    {
        gameObject.GetComponent<Renderer>().material = offMaterial;
    }
}
