using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TapeLibraryScript : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject miniTapePrefab;

    private GameObject[] tapes;

    private TapeScript readTapeScript;
    [SerializeField]
    private Texture2D font;

    void Start()
    {
        string filePath = Application.persistentDataPath + @"/Tapes/";
        if (!Directory.Exists(filePath)) 
            Directory.CreateDirectory(filePath);
        string filePath0 = Application.streamingAssetsPath + @"/Tapes/";

        DirectoryInfo dir = new DirectoryInfo(filePath0);
        List<FileInfo> infoList = new List<FileInfo>();
        infoList.AddRange(dir.GetFiles("*.ptw"));
        infoList.AddRange(dir.GetFiles("*.ptr"));
        infoList.AddRange(dir.GetFiles("*.ptp"));
        infoList.AddRange(dir.GetFiles("*.txt"));

        foreach (FileInfo f in infoList)
        {
            File.Copy(filePath0 + f.Name, filePath + f.Name, true);
        }

        readTapeScript = null;
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void setTapeScript(TapeScript t)
    {
        if (readTapeScript != null)
            return;
        readTapeScript = t;

        string filePath = Application.persistentDataPath + @"/Tapes/";

        DirectoryInfo dir = new DirectoryInfo(filePath);
        List<FileInfo> infoList = new List<FileInfo>();
        infoList.AddRange(dir.GetFiles("*.ptw"));
        infoList.AddRange(dir.GetFiles("*.ptr"));
        infoList.AddRange(dir.GetFiles("*.ptp"));
        infoList.AddRange(dir.GetFiles("*.txt"));
        tapes = new GameObject[infoList.Count];

        int i = 0;
        foreach (FileInfo f in infoList)
        {
            tapes[i] = Instantiate(miniTapePrefab,
                t.gameObject.transform.position + new Vector3(-0.025f, 0.12f + 0.01f * i, 0.0f), 
                miniTapePrefab.transform.rotation);
            tapes[i].GetComponent<MiniTapeScript>().setName(f.Name);
            tapes[i].GetComponent<MiniTapeScript>().setData(System.IO.File.ReadAllText(f.ToString()));
            tapes[i].GetComponent<MiniTapeScript>().setTapeLibraryScript(this);
            tapes[i].GetComponent<MiniTapeScript>().setFont(font);
            i++;
        }
    }

    public void selected(MiniTapeScript m)
    {
        readTapeScript.readString(m.getData());
        for (int i = 0; i < tapes.Length; i++)
        {
            Destroy(tapes[i]);
        }
        tapes = null;
        readTapeScript = null;
    }
}
