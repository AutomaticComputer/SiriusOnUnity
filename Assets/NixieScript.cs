using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class NixieScript : MonoBehaviour
{

    [SerializeField]
    private Renderer rend0, rend1, rend2, rend3, rend4, rend5, rend6, rend7, rend8, rend9;
//    [SerializeField]
//    private Renderer rend0w, rend1w, rend2w, rend3w, rend4w, rend5w, rend6w, rend7w, rend8w, rend9w;
    [SerializeField]
    private Material matOn; 
 
    private Renderer[] renderers;

    // Start is called before the first frame update
    void Start()
    {

        /*
        rend0w.enabled = false; // FIXME: performance?
        rend1w.enabled = false;
        rend2w.enabled = false;
        rend3w.enabled = false;
        rend4w.enabled = false;
        rend5w.enabled = false;
        rend6w.enabled = false;
        rend7w.enabled = false;
        rend8w.enabled = false;
        rend9w.enabled = false;
        */

        renderers = new Renderer[10];
        renderers[0] = rend0; renderers[1] = rend1; renderers[2] = rend2; renderers[3] = rend3;
        renderers[4] = rend4; renderers[5] = rend5; renderers[6] = rend6; renderers[7] = rend7;
        renderers[8] = rend8; renderers[9] = rend9;

        for (int i = 0; i < 10; i++) { 
            renderers[i].enabled = false;
            renderers[i].material = matOn;
        }
        setNumber(0);
    }

    // Update is called once per frame
    void Update()
    {
    }

    private int currentNumber = 0;
    public void setNumber(int i)
    {
        if (currentNumber >= 0)
        {
            renderers[currentNumber].enabled = false;
        }
        if (i >= 0 && i < 10)
        {
            renderers[i].enabled = true; 
            currentNumber = i;
        } else
        {
            currentNumber = -1;
        }
    }
}
