using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TapeSaveScript : MonoBehaviour
{
    [SerializeField]
    private TapeScript tapeScript;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseDown()
    {
        System.IO.StreamWriter file =
                    new System.IO.StreamWriter(Application.dataPath + @"/Tapes/" +
                    System.DateTime.Now.ToString("yyyyMMddHHmmss") + ".ptp");
        byte[] d = tapeScript.getData();

        for(int i = 0; i < d.Length; i++)
        {
            file.Write(d[i]);
            file.Write(' ');
            if (i % 10 == 9)
                file.WriteLine();
        }
        file.WriteLine();
        file.Close();

        tapeScript.clear();
    }
}
