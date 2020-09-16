using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Segment : MonoBehaviour
{
    public Material mat;
    // Start is called before the first frame update
    void Start()
    {
        mat.SetColor("_EmissionColor", new Color(0.4f, 0.2f, 0.1f));
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            mat.SetColor("_EmissionColor", new Color(0.9f, 0.7f, 0.7f));
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            mat.SetColor("_EmissionColor", new Color(0.4f, 0.2f, 0.1f));
        }
    }
}
