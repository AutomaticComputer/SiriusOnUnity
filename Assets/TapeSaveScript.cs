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
        string fileNameBase, fileName;

        fileNameBase = Application.persistentDataPath + @"/Tapes/" + System.DateTime.Now.ToString("yyyyMMddHHmmss");

        for(int i=0; ; i++) {
            if (i == 0)
                fileName = fileNameBase + ".ptp";
            else
                fileName = fileNameBase + i + ".ptp";
            if (!System.IO.File.Exists(fileName))
                break;
        }

        System.IO.StreamWriter file = new System.IO.StreamWriter(fileName);
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
