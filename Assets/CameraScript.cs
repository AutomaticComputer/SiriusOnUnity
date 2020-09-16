using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class CameraScript : MonoBehaviour
{
    // Start is called before the first frame update

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        float horizontalInput = 0.0f; 
        float verticalInput = 0.0f; 
        float depthInput = 0.0f;

        if (Input.GetKey(KeyCode.UpArrow))
            verticalInput = 1.0f;
        if (Input.GetKey(KeyCode.DownArrow))
            verticalInput = -1.0f;
        if (Input.GetKey(KeyCode.RightArrow))
            horizontalInput = 1.0f;
        if (Input.GetKey(KeyCode.LeftArrow))
            horizontalInput = -1.0f;
        if (Input.GetKey(KeyCode.PageUp))
            depthInput = 1.0f;
        if (Input.GetKey(KeyCode.PageDown))
            depthInput = -1.0f;

        transform.Translate(new Vector3(horizontalInput, verticalInput, depthInput) * Time.deltaTime * 0.2f);
    }
}
