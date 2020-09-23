using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelScript : MonoBehaviour
{
    private int value;
    private const int valueMax = 10, defaultValue = 5;
    private Vector3 savedEulerAngles;
    // Start is called before the first frame update
    void Start()
    {
        savedEulerAngles = gameObject.transform.localEulerAngles;

        value = defaultValue;
    }
    
    // Update is called once per frame
    void Update()
    {

    }

    private void OnMouseDown()
    {
        int i, j;
        Vector3 screenPoint = Camera.main.WorldToScreenPoint(transform.position);
        Vector3 p = 
//            Quaternion.Inverse(transform.rotation) * 
            (Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 
            screenPoint.z)) - transform.position);
        if (p.x > 0 && value > 0)
        {
            value--;
        }
        if (p.x < 0 && value < valueMax)
        {
            value++;
        }
        gameObject.transform.localEulerAngles = savedEulerAngles + new Vector3(0, 0, (value-5) * 15.0f);
    }

    public int getValue()
    {
        return value;
    }
}
