using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;


public class PrinterScript : MonoBehaviour
{
    [SerializeField]
    private PrinterDrumScript printerDrumScript;
    [SerializeField]
    private TeleprinterKeyboardScript teleprinterKeyboardScript;
    [SerializeField]
    private TapeScript tapeReadScript, tapePunchScript;
    [SerializeField]
    private OnOffButtonScript buttonReadScript, buttonEchoScript, buttonPunchScript;
    [SerializeField]
    private PushButtonScript pbReadScript, pbRew, pbCutScript;

    private byte keyCode, typeCode, punchCode;

    private float timeElapsed;
    private const float secondsPerChar = 0.05f;

    // Start is called before the first frame update
    void Start()
    {
        keyCode = 0xff;
        typeCode = 0xff;
        punchCode = 0xff;
        buttonReadScript.setOn(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (pbCutScript.isPushed())
        {
            printerDrumScript.printToFile();
            return;
        }

        byte b;
        b = teleprinterKeyboardScript.getCode();

        if (b < 32)
        {
            keyCode = b;
            if (buttonEchoScript.isOn())
                typeCode = b;
            if (buttonPunchScript.isOn())
                punchCode = b;
        }

        if (typeCode == 0xff && pbReadScript.isPushed())
        {
            b = tapeReadScript.read();
            if (b != 0xff)
            {
                typeCode = b;
                if (buttonPunchScript.isOn())
                    punchCode = b;
            }
        }

        if (typeCode == 0xff && buttonReadScript.isOn())
        {
            b = tapeReadScript.read();
            if (b != 0xff)
            {
                typeCode = b;
                if (buttonPunchScript.isOn())
                    punchCode = b;
            }
            else
            {
                buttonReadScript.setOn(false);
            }
        }

        if (typeCode != 0xff)
        {
            if (printerDrumScript.type(typeCode))
                typeCode = 0xff;
        }

        if (punchCode != 0xff)
        {
            if (punchCode == 31)
                tapePunchScript.rewind();
            if (tapePunchScript.punch(punchCode))
                punchCode = 0xff;
        }

        if (pbRew.isPushed())
        {
            tapePunchScript.rewind();
        }
    }

    public byte read()
    {
        byte b = keyCode;
        keyCode = 0xff;
        return b;
    }

    public bool print(byte b) // true if ok
    {
        if (typeCode != 0xff)
            return false;
        return printerDrumScript.type(b);
    }
}
