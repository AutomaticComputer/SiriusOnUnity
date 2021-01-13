using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleprinterKeyboardScript : MonoBehaviour
{
    public GameObject keyPrefab, wideKeyPrefab;

    [SerializeField]
    private Texture2D teleprinterFont;

    private GameObject[] keyboard;

    const float KeyW = 0.015f, KeyH = 0.015f, KeyY = 0.0125f,
        KeyDownY = 0.0075f, CollideZ = 0.015f,
        KeyboardStartX = -0.12f, KeyboardStartZ = 0.04f;

    private char[] keyCharFS, keyCharLS;
    private int[] keyX, keyZ, keyWide, keyFont, keyFS, keyLS;

    private bool isLetterShift;
    private byte c;
    // Start is called before the first frame update
    void Start()
    {
        keyX = new int[] 
            {
                1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 16, 
                1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15,
                0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 
                0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12,
                2, 6, 10
            };
        keyZ = new int[]
        {
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
        1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
        2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2,
        3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
        4, 4, 4
        };
        keyWide = new int[]
        {
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        1, 1, 1
        };
        keyFont = new int[]
        {
        1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 21, 22, 62, 63, 32, // 32: blank
        49, 55, 37, 50, 52, 57, 53, 41, 47, 48, 26, 10, 25, 19, 20,
        30, 33, 51, 36, 38, 39, 40, 42, 43, 44, 61, 12, 18, 30, 
        13, 58, 56, 35, 54, 34, 46, 45, 31, 23, 11, 17, 13,
        16, 14, 27
        };
        keyFS = new int[]
        {
        1, 2, 19, 4, 21, 22, 7, 8, 25, 16, 5, 6, -1, 31, 32, // 32: blnk
        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 10, 26, 9, 3, 20,
        30, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 28, 18, 30,
        13, -1, 24, -1, 12, -1, 29, -1, 15, 23, 11, 17, 13,
        -1, 14, 27
        };
        keyLS = new int[]
        {
        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 30, 31, -1, 
        17, 23, 5, 18, 20, 25, 21, 9, 15, 16, -1, -1, -1, -1, -1,
        -1, 1, 19, 4, 6, 7, 8, 10, 11, 12, 29, 28, -1, -1,
        -1, 26, 24, 3, 22, 2, 14, 13, -1, -1, -1, -1, -1,
        0, -1, -1
        };
        keyCharFS = new char[]
        {
            '1', '2', '3', '4', '5', '6', '7', '8', '9', '0', '(', ')', '\0', '\b', '|', 
            '\0', '\0', '\0', '\0', '\0', '\0', '\0', '\0', '\0', '\0', '=', '+', '#', '*', '^',
            '\r', '\0', '\0', '\0', '\0', '\0', '\0', '\0', '\0', '\0', '\0', '.', '}', '\r',
            '\t', '\0', 'x', '\0', 'v', '\0', 'n', '\0', ',', '/', '-', '>', '\t',
            '\0', ' ', '\0'
        }; 
        keyCharLS = new char[]
        {
            '\0', '\0', '\0', '\0', '\0', '\0', '\0', '\0', '\0', '\0', '\0', '\0', 'L', '\b', '\0', 
            'q', 'w', 'e', 'r', 't', 'y', 'u', 'i', 'o', 'p', '\0', '\0', '\0', '\0', '\0',
            '\0', 'a', 's', 'd', 'f', 'g', 'h', 'j', 'k', 'l', '?', '.', '\0', '\0',
            '\0', 'z', 'x', 'c', 'v', 'b', 'n', 'm', '\0', '\0', '\0', '\0', '\0',
            '\0', '\0', '\0'
        };

        keyboard = new GameObject[keyX.Length];

        for (int i = 0; i < keyboard.Length; i++)
        {
            if (keyWide[i] == 0)
            {
                keyboard[i] = Instantiate(keyPrefab,
                     transform.position + transform.rotation *
                     new Vector3(KeyboardStartX + keyX[i] * KeyW + keyZ[i] * (KeyW * 0.5f), 
                     KeyY, KeyboardStartZ - keyZ[i] * KeyH),
                     keyPrefab.transform.rotation * transform.rotation);
            }
            else
            {
                keyboard[i] = Instantiate(wideKeyPrefab,
                     transform.position + transform.rotation *
                     new Vector3(KeyboardStartX + keyX[i] * KeyW + keyZ[i] * (KeyW * 0.5f),
                     KeyY, KeyboardStartZ - keyZ[i] * KeyH),
                     keyPrefab.transform.rotation * transform.rotation);
            }
            Texture2D texture = new Texture2D(12, 16);
            for (int x = 0; x < 12; x++)
                for (int y = 0; y < 16; y++)
                    texture.SetPixel(x, y, Color.white);
            texture.SetPixels(3, 4, 6, 8,
                teleprinterFont.GetPixels(keyFont[i] * 6, 0, 6, 8));
            keyboard[i].GetComponent<Renderer>().material.mainTexture = texture;
            keyboard[i].GetComponent<KeyColorScript>().setColors(Color.white, Color.gray);
            texture.Apply();
        }

        isLetterShift = false;
        setColor();
        c = 0xff;
    }


    // Update is called once per frame
    void Update()
    {
        if (c != 0xff)
            return;

        if (isLetterShift && Input.GetKeyDown(KeyCode.LeftControl))
        {
            isLetterShift = false;
            c = 0; // FS
            setColor();
            return;
        }
        if (!isLetterShift && Input.GetKeyDown(KeyCode.RightControl))
        {
            isLetterShift = true;
            c = 27; // LS
            setColor();
            return;
        }

        if (!isLetterShift && Input.GetKeyDown(KeyCode.Tab))
        {
            c = 13; // LF : it seems that we can't read Tab with Input.inputString
            return;
        }

        string s = Input.inputString;
        if (s.Length > 0)
        {
            char ch = s[0];
            for (int k = 0; k < keyboard.Length; k++)
            {
                if (!isLetterShift && keyCharFS[k] == ch)
                    c = (byte)keyFS[k];
                if (isLetterShift && keyCharLS[k] == ch)
                    c = (byte)keyLS[k];
            }
        }
        if (c == 32) // blank
            c = 0;
    }

    private void setColor()
    {
        for(int i=0; i<keyboard.Length; i++)
        {
            bool b = ((!isLetterShift && keyFS[i] >= 0) || (isLetterShift && keyLS[i] >= 0));
            keyboard[i].GetComponent<KeyColorScript>().setDark(!b);
        }
    }

    public byte getCode()
    {
        byte b = c;
        c = 0xff;
        return b;
    }
    private void OnMouseDown()
    {
        int i, j;
        int k;
        Vector3 dir = Quaternion.Inverse(transform.rotation) * 
                Camera.main.ScreenPointToRay(Input.mousePosition).direction;
        Vector3 q = Quaternion.Inverse(transform.rotation) * 
                (Camera.main.transform.position - transform.position);
        float px = q.x + dir.x * (CollideZ - q.y) / dir.y; 
        float pz = q.z + dir.z * (CollideZ - q.y) / dir.y;

        j = (int)Math.Round((KeyboardStartZ - pz) / KeyH);
        i = (int)Math.Round((px - KeyboardStartX - KeyW * 0.5 * j) / KeyW);
        for (k=0; k < keyboard.Length; k++)
        {
            if (keyX[k] == i && keyZ[k] == j)
            {
                if (!isLetterShift && keyFS[k] >= 0)
                {
                    c = (byte) keyFS[k];
                }
                if (isLetterShift && keyLS[k] >= 0)
                {
                    c = (byte) keyLS[k];
                }
            }
        }
        if (c == 0) // FS
        {
            if (isLetterShift)
            {
                isLetterShift = false;
                setColor();
            }
            else
                c = 0xff;
        }
        if (c == 27) // LS
        {
            if (!isLetterShift)
            {
                isLetterShift = true;
                setColor();
            }
            else
                c = 0xff;
        }
        if (c == 32) // blank
        {
            c = 0;
        }
    }
}
