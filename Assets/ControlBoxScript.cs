using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Transactions;
using UnityEditor;
using UnityEngine;
// using UnityEngine.UIElements;

public class ControlBoxScript : MonoBehaviour
{
    public GameObject keyPrefab, wideKeyPrefab;
    private GameObject[,] keyboard;
    private GameObject[] accumulatorKeys;
    private GameObject primaryInputKey, isol0Key, free0Key, isol100Key, free100Key, 
        interruptKey, acceptInputKey, noKBWaitKey, kbWaitKey, autoKey, manKey, clearControlKey, waitKey, runKey, continueKey;
    private int[] pressed;
    private int _selectedAccumulator;
    public bool flagFree0, flagFree100, flagKBWait, flagManual, flagRun;
    public bool flagPrimaryInputKey, flagInterruptKey, flagAcceptInputKey, flagClearControlKey, flagContinueKey;

    [SerializeField]
    private Texture2D contolBoxTexture1, contolBoxTexture2, contolBoxTexture3;
    [SerializeField]
    private PushButtonScript btnReleaseScript;

    // The surface of keys is assumed to be z = this.z + CollideZ 
    const float KeyW = 0.021f, KeyH = 0.021f, KeyY = 0.005f, 
        KeyDownY = 0.007f, CollideZ = 0.015f,
        AccumulatorStartX = -0.14f, AccumulatorStartZ = -0.07f,
        KeyboardStartX = -0.1f, KeyboardStartZ = -0.07f, 
        WideKeyX = 0.14f, RadioKeyX = 0.13f; 

    // Start is called before the first frame update
    void Start()
    {
        Color lightGray, darkGray, darkRed, darkGreen, darkYellow;
        lightGray = new Color(0.7f, 0.7f, 0.7f);
        darkGray = new Color(0.35f, 0.35f, 0.35f);
        darkRed = new Color(0.7f, 0.0f, 0.0f);
        darkGreen = new Color(0.0f, 0.7f, 0.0f);
        darkYellow = new Color(0.7f, 0.7f, 0.0f);

        Texture2D[] textures = new Texture2D[11];
        for(int i=0; i < 11; i++)
        {
            textures[i] = new Texture2D(32, 32);
            textures[i].SetPixels(0, 0, 32, 32, contolBoxTexture1.GetPixels(i * 32, 0, 32, 32));
            textures[i].Apply();
        }

        Texture2D[] tex2 = new Texture2D[10];
        for (int i = 0; i < 10; i++)
        {
            tex2[i] = new Texture2D(32, 32);
            tex2[i].SetPixels(0, 0, 32, 32, contolBoxTexture2.GetPixels(i * 32, 0, 32, 32));
            tex2[i].Apply();
        }
        Texture2D[] tex3 = new Texture2D[5];
        for (int i = 0; i < 5; i++)
        {
            tex3[i] = new Texture2D(64, 32);
            tex3[i].SetPixels(0, 0, 64, 32, contolBoxTexture3.GetPixels(i * 64, 0, 64, 32));
            tex3[i].Apply();
        }

        keyboard = new GameObject[10, 10];
        accumulatorKeys = new GameObject[10];
        pressed = new int[10];
        for(int i=0; i<10; i++)
        {
            for(int j=0; j<10; j++)
            {
                keyboard[i, j] = Instantiate(keyPrefab, 
                    transform.position + transform.rotation * 
                    new Vector3(KeyboardStartX + i * KeyW, KeyY, KeyboardStartZ + j * KeyH),
                    keyPrefab.transform.rotation* transform.rotation);
                if (i < 3 || i == 6 || i == 7 || i == 9)
                {
                    keyboard[i, j].GetComponent<KeyColorScript>().setColors(Color.white, lightGray);
                }
                else
                {
                    keyboard[i, j].GetComponent<KeyColorScript>().setColors(Color.gray, darkGray);
                }
                keyboard[i, j].GetComponent<Renderer>().material.mainTexture = textures[j]; 
            }
            pressed[i] = -1;
        }
        for (int a = 0; a < 10; a++)
        {
            accumulatorKeys[a] = Instantiate(keyPrefab,
                transform.position + transform.rotation *
                new Vector3(AccumulatorStartX, KeyY, AccumulatorStartZ + a * KeyH),
                keyPrefab.transform.rotation * transform.rotation);
            if (a == 0)
            {
                accumulatorKeys[a].GetComponent<KeyColorScript>().setColors(Color.white, lightGray);
                accumulatorKeys[a].GetComponent<Renderer>().material.mainTexture = textures[10];
            }
            else
            {
                accumulatorKeys[a].GetComponent<KeyColorScript>().setColors(Color.gray, darkGray);
                accumulatorKeys[a].GetComponent<Renderer>().material.mainTexture = textures[a];
            }
        }
        accumulatorKeys[0].transform.Translate(transform.rotation * new Vector3(0.0f, KeyDownY, 0.0f));
        _selectedAccumulator = 0;

        primaryInputKey = Instantiate(wideKeyPrefab,
            transform.position + transform.rotation *
            new Vector3(WideKeyX, KeyY, KeyboardStartZ + 9 * KeyH),
            wideKeyPrefab.transform.rotation * transform.rotation);
        primaryInputKey.GetComponent<Renderer>().material.mainTexture = tex3[0];
        primaryInputKey.GetComponent<KeyColorScript>().setColors(Color.white, lightGray);

        isol0Key = Instantiate(keyPrefab,
            transform.position + transform.rotation *
            new Vector3(RadioKeyX, KeyY, KeyboardStartZ + 8 * KeyH),
            keyPrefab.transform.rotation * transform.rotation);
        isol0Key.GetComponent<Renderer>().material.mainTexture = tex2[0];
        isol0Key.GetComponent<KeyColorScript>().setColors(Color.gray, darkGray);
        free0Key = Instantiate(keyPrefab,
            transform.position + transform.rotation *
            new Vector3(RadioKeyX + KeyW, KeyY, KeyboardStartZ + 8 * KeyH),
            keyPrefab.transform.rotation * transform.rotation);
        free0Key.GetComponent<Renderer>().material.mainTexture = tex2[1];
        free0Key.GetComponent<KeyColorScript>().setColors(Color.white, lightGray);
        isol100Key = Instantiate(keyPrefab,
            transform.position + transform.rotation *
            new Vector3(RadioKeyX, KeyY, KeyboardStartZ + 7 * KeyH),
            keyPrefab.transform.rotation * transform.rotation);
        isol100Key.GetComponent<Renderer>().material.mainTexture = tex2[2];
        isol100Key.GetComponent<KeyColorScript>().setColors(Color.gray, darkGray);
        free100Key = Instantiate(keyPrefab,
            transform.position + transform.rotation *
            new Vector3(RadioKeyX + KeyW, KeyY, KeyboardStartZ + 7 * KeyH),
            keyPrefab.transform.rotation * transform.rotation);
        free100Key.GetComponent<Renderer>().material.mainTexture = tex2[3];
        free100Key.GetComponent<KeyColorScript>().setColors(Color.white, lightGray);

        interruptKey = Instantiate(wideKeyPrefab,
            transform.position + transform.rotation *
            new Vector3(WideKeyX, KeyY, KeyboardStartZ + 6 * KeyH),
            wideKeyPrefab.transform.rotation * transform.rotation);
        interruptKey.GetComponent<Renderer>().material.mainTexture = tex3[1];
        interruptKey.GetComponent<KeyColorScript>().setColors(Color.white, lightGray);

        acceptInputKey = Instantiate(wideKeyPrefab,
            transform.position + transform.rotation *
            new Vector3(WideKeyX, KeyY, KeyboardStartZ + 5 * KeyH),
            wideKeyPrefab.transform.rotation * transform.rotation);
        acceptInputKey.GetComponent<Renderer>().material.mainTexture = tex3[2];
        acceptInputKey.GetComponent<KeyColorScript>().setColors(Color.white, lightGray);

        noKBWaitKey = Instantiate(keyPrefab,
            transform.position + transform.rotation *
            new Vector3(RadioKeyX, KeyY, KeyboardStartZ + 4 * KeyH),
            keyPrefab.transform.rotation * transform.rotation);
        noKBWaitKey.GetComponent<Renderer>().material.mainTexture = tex2[4];
        noKBWaitKey.GetComponent<KeyColorScript>().setColors(Color.gray, darkGray);
        kbWaitKey = Instantiate(keyPrefab,
            transform.position + transform.rotation *
            new Vector3(RadioKeyX + KeyW, KeyY, KeyboardStartZ + 4 * KeyH),
            keyPrefab.transform.rotation * transform.rotation);
        kbWaitKey.GetComponent<Renderer>().material.mainTexture = tex2[5];
        kbWaitKey.GetComponent<KeyColorScript>().setColors(Color.white, lightGray);

        autoKey = Instantiate(keyPrefab,
            transform.position + transform.rotation *
            new Vector3(RadioKeyX, KeyY, KeyboardStartZ + 3 * KeyH),
            keyPrefab.transform.rotation * transform.rotation);
        autoKey.GetComponent<Renderer>().material.mainTexture = tex2[6];
        autoKey.GetComponent<KeyColorScript>().setColors(Color.gray, darkGray);
        manKey = Instantiate(keyPrefab,
            transform.position + transform.rotation *
            new Vector3(RadioKeyX + KeyW, KeyY, KeyboardStartZ + 3 * KeyH),
            keyPrefab.transform.rotation * transform.rotation);
        manKey.GetComponent<Renderer>().material.mainTexture = tex2[7];
        manKey.GetComponent<KeyColorScript>().setColors(Color.white, Color.gray);

        clearControlKey = Instantiate(wideKeyPrefab,
            transform.position + transform.rotation *
            new Vector3(WideKeyX, KeyY, KeyboardStartZ + 2 * KeyH),
            wideKeyPrefab.transform.rotation * transform.rotation);
        clearControlKey.GetComponent<Renderer>().material.mainTexture = tex3[3];
        clearControlKey.GetComponent<KeyColorScript>().setColors(Color.white, lightGray);

        waitKey = Instantiate(keyPrefab,
            transform.position + transform.rotation *
            new Vector3(RadioKeyX, KeyY, KeyboardStartZ + 1 * KeyH),
            keyPrefab.transform.rotation * transform.rotation);
        waitKey.GetComponent<Renderer>().material.mainTexture = tex2[8];
        waitKey.GetComponent<KeyColorScript>().setColors(Color.red, darkRed);
        runKey = Instantiate(keyPrefab,
            transform.position + transform.rotation *
            new Vector3(RadioKeyX + KeyW, KeyY, KeyboardStartZ + 1 * KeyH),
            keyPrefab.transform.rotation * transform.rotation);
        runKey.GetComponent<Renderer>().material.mainTexture = tex2[9];
        runKey.GetComponent<KeyColorScript>().setColors(Color.green, darkGreen);
        
        continueKey = Instantiate(wideKeyPrefab,
            transform.position + transform.rotation *
            new Vector3(WideKeyX, KeyY, KeyboardStartZ + 0 * KeyH),
            wideKeyPrefab.transform.rotation * transform.rotation);
        continueKey.GetComponent<Renderer>().material.mainTexture = tex3[4];
        continueKey.GetComponent<KeyColorScript>().setColors(Color.yellow, darkYellow);
        
        flagFree0 = true;
        flagFree100 = true;
        flagKBWait = false;
        flagManual = true;
        flagRun = false;

        pressKey(free0Key);
        pressKey(free100Key);
        pressKey(noKBWaitKey);
        pressKey(manKey);
        pressKey(waitKey);

        flagPrimaryInputKey = false;
        flagInterruptKey = false;
        flagAcceptInputKey = false;
        flagClearControlKey = false;
        flagContinueKey = false;
    }


    private float pressTimeout;

    // Update is called once per frame
    void Update()
    {
        if (pressTimeout > 0)
        {
            pressTimeout = pressTimeout - Time.deltaTime;
            if (pressTimeout <= 0.0f)
                releaseKey(currentlyPressed);
        }

        if (btnReleaseScript.isPushed()) 
        {
            for(int i = 0; i < 10; i++)
            {
                if (pressed[i] >= 0) {
                    releaseKey(keyboard[i, pressed[i]]);
                    pressed[i] = -1;
                }
            }
        }
    }

    GameObject currentlyPressed;
    private void pressKeyWithTimeout(GameObject key)
    {
        if (pressTimeout > 0.0f)
            releaseKey(currentlyPressed);
        pressKey(key);
        currentlyPressed = key;
        pressTimeout = 0.25f;
    }

    private void pressKey(GameObject key)
    {
        key.transform.Translate(transform.rotation * new Vector3(0.0f, KeyDownY, 0.0f));
        key.GetComponent<KeyColorScript>().setDark(true);
    }
    private void releaseKey(GameObject key)
    {
        key.transform.Translate(transform.rotation * new Vector3(0.0f, -KeyDownY, 0.0f));
        key.GetComponent<KeyColorScript>().setDark(false);
    }
    private void OnMouseDown()
    {
        int i, j;
        Vector3 dir = Quaternion.Inverse(transform.rotation) * 
                Camera.main.ScreenPointToRay(Input.mousePosition).direction;
        Vector3 q = Quaternion.Inverse(transform.rotation) * 
                (Camera.main.transform.position - transform.position);
        float px = q.x + dir.x * (CollideZ - q.y) / dir.y; 
        float pz = q.z + dir.z * (CollideZ - q.y) / dir.y;

        // check keyboard
        i = (int) Math.Round((px - KeyboardStartX) / KeyW); 
        j = (int) Math.Round((pz - KeyboardStartZ) / KeyH);
        if (i >= 0 && i < 10 && j >= 0 && j < 10)
        {
            if (pressed[i] >= 0)
            {
                releaseKey(keyboard[i, pressed[i]]);
            }
            pressKey(keyboard[i, j]);
            pressed[i] = j;
        }

        // check accumulator keys
        i = (int) Math.Round((px - AccumulatorStartX) / KeyW);
        j = (int) Math.Round((pz - AccumulatorStartZ) / KeyH);
        if (i == 0 && j >= 0 && j < 10)
        {
            releaseKey(accumulatorKeys[_selectedAccumulator]); 
            pressKey(accumulatorKeys[j]); 
            _selectedAccumulator = j;
        }

        // check other keys
        i = (int)Math.Round((px - RadioKeyX) / KeyW);
        j = (int)Math.Round((pz - AccumulatorStartZ) / KeyH);
        if (i >= 0 && i < 2 && j >= 0 && j < 10)
        {
            bool newFlag = (i == 1);

            switch (j)
            {
                case 0:
                    flagContinueKey = true;
                    pressKeyWithTimeout(continueKey);
                    break;
                case 1:
                    if (flagRun != newFlag)
                    {
                        flagRun = newFlag;
                        if (flagRun)
                        {
                            releaseKey(waitKey);
                            pressKey(runKey);
                        }
                        else
                        {
                            pressKey(waitKey);
                            releaseKey(runKey);
                        }
                    }
                    break;
                case 2:
                    flagClearControlKey = true;
                    pressKeyWithTimeout(clearControlKey);
                    break;
                case 3:
                    if (flagManual != newFlag)
                    {
                        flagManual = newFlag;
                        if (flagManual)
                        {
                            releaseKey(autoKey);
                            pressKey(manKey);
                        }
                        else
                        {
                            pressKey(autoKey);
                            releaseKey(manKey);
                        }
                    }
                    break;
                case 4:
                    if (flagKBWait != newFlag)
                    {
                        flagKBWait = newFlag;
                        if (flagKBWait)
                        {
                            releaseKey(noKBWaitKey);
                            pressKey(kbWaitKey);
                        }
                        else
                        {
                            pressKey(noKBWaitKey);
                            releaseKey(kbWaitKey);
                        }
                    }
                    break;
                case 5:
                    flagAcceptInputKey = true;
                    pressKeyWithTimeout(acceptInputKey);
                    break;
                case 6:
                    flagInterruptKey = true;
                    pressKeyWithTimeout(interruptKey);
                    break;
                case 7:
                    if (flagFree100 != newFlag)
                    {
                        flagFree100 = newFlag;
                        if (flagFree100)
                        {
                            releaseKey(isol100Key);
                            pressKey(free100Key);
                        }
                        else
                        {
                            pressKey(isol100Key);
                            releaseKey(free100Key);
                        }
                    }
                    break;
                case 8:
                    if (flagFree0 != newFlag)
                    {
                        flagFree0 = newFlag;
                        if (flagFree0)
                        {
                            releaseKey(isol0Key);
                            pressKey(free0Key);
                        }
                        else
                        {
                            pressKey(isol0Key);
                            releaseKey(free0Key);
                        }
                    }
                    break;
                case 9:
                    flagPrimaryInputKey = true;
                    pressKeyWithTimeout(primaryInputKey);
                    break;
            }
        }

    }

    public int selectedAccumulator()
    {
        return _selectedAccumulator;
    }

    public BCD10 getKeyboard()
    {
        BCD10 result = new BCD10();

        for(int i=0; i<10; i++)
        {
            int n = pressed[i];
            if (n < 0)
                n = 0;
            result.digits[9 - i] = (byte) n;
        }
        return result;
    }
}

