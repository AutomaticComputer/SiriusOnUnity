using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class CPUScript : MonoBehaviour
{
    public NixieArrayScript displayScript0, displayScript1;
    public ControlBoxScript controlBoxScript;
    public IndicatorScript indicatorPIScript, indicatorOVRScript, indicator99WScript, 
        indicatorKSScript, indicator6910Script, indicatorRBScript, indicatorPBScript;

    public TapeScript tapePunchScript, tapeReaderScriptA, tapeReaderScriptB;
    public PrinterScript printerScript;

    private BCD10[] accumulators;
    const int controlRegister = 1;
    private BCD10 currentOrder; 
    private UInt64[] mainStore;
    private UInt64[] primaryInputMicroProgram;
    bool isPrimaryInput;
    private int primaryInputPC;
    private const int mainStoreSize = 10000, mainStoreModulus = 10000;

    private float timeLeft;
    private const float cycleTime = 0.00008f;

    private uint delayLinePos;
    
    private byte siriusOpcodeLow, siriusOpcodeHigh, accA, accB;
    private BCD6 siriusOperand;
    private BCD10 effectiveOperand;

    private BCD10 acceptedInput;

    private bool flagInputAccepted;
    private bool isStopped99, isStoppedKB, isStopped6910, isStoppedByError, flagOVR, 
        isWaitingR, isWaitingP, 
        isRBusy, isPBusy, isRetrying, isRetryingForFrame;

    private byte[, ] mask66_68, mask65_67;

// Start is called before the first frame update
void Start()
    {
        accumulators = new BCD10[10]; // accumulators[0] holds content to be displayed, not 0. 
        for (int i = 0; i < 10; i++)
            accumulators[i] = new BCD10();

        currentOrder = new BCD10();
        siriusOperand = new BCD6();
        effectiveOperand = new BCD10();

        mainStore = new UInt64[mainStoreSize];

        primaryInputMicroProgram = new UInt64[] {
            0x0000007140L, 0x0000065440L, 0x0000001544L, 0x0000001544L, 0x0000003954L,
            0x0000005500L, 0x0000006051L, 0x0000010010L, 0x0000007140L, 0x0000025940L,
            0x0000009900L
        };

        mask66_68 = new byte[,] // in the order: N+b, a
        {
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            {0, 1, 2, 3, 4, 5, 6, 7, 8, 9 },
            {0, 0, 2, 2, 4, 5, 5, 7, 7, 9 },
            {0, 1, 2, 3, 4, 5, 6, 7, 8, 9 },
            {0, 0, 0, 0, 4, 5, 5, 5, 5, 9 },
            {0, 0, 0, 0, 0, 5, 5, 5, 5, 5 },
            {0, 1, 2, 3, 4, 5, 6, 7, 8, 9 },
            {0, 0, 2, 2, 4, 5, 5, 7, 7, 9 },
            {0, 1, 2, 3, 4, 5, 6, 7, 8, 9 },
            {0, 0, 0, 0, 4, 5, 5, 5, 5, 9 }
        };
        mask65_67 = new byte[,] // in the order: N+b, a
        {
            {0, 1, 2, 3, 4, 5, 6, 7, 8, 9 },
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            {0, 1, 0, 1, 0, 0, 1, 0, 1, 0 }, 
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            {0, 1, 2, 3, 0, 0, 1, 2, 3, 0 },
            {0, 1, 2, 3, 4, 0, 1, 2, 3, 4 },
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            {0, 1, 0, 1, 0, 0, 1, 0, 1, 0 }, 
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            {0, 1, 2, 3, 0, 0, 1, 2, 3, 0 }
        };

        isPrimaryInput = false; 
        isStopped99 = false;
        isStoppedByError = false;
        isStoppedKB = false;
        isStopped6910 = false;
        isRetrying = false;
        isRetryingForFrame = false;
        isWaitingR = false;
        isWaitingP = false;
        isRBusy = false; // only when no tape
        isPBusy = false; // always false
        flagOVR = false;

        flagInputAccepted = false;

        delayLinePos = 0;
        timeLeft = 0.0f;
    }


    private bool runPressed, manualPressed, kbWaitPressed, flagContinue;
    // flagContinue: either Continue is pressed or primary input ended
    private BCD10 kbValue;


// Update is called once per frame
    void Update()
    {
        bool flagInterrupt = false;

        kbValue = controlBoxScript.getKeyboard();

        if (isPrimaryInput)
        {
            timeLeft += Time.deltaTime;
            isRetryingForFrame = isRetrying;

            while (isPrimaryInput && timeLeft > 0.0f) 
            {
                processOneOrder();
            }
        }
        else
        {
            if (!isRetryingForFrame)
            {
                runPressed = controlBoxScript.flagRun;
                manualPressed = controlBoxScript.flagManual;
                kbWaitPressed = controlBoxScript.flagKBWait;

                if (controlBoxScript.flagContinueKey)
                {
                    flagContinue = true;
                    controlBoxScript.flagContinueKey = false;
                }

                if (controlBoxScript.flagInterruptKey)
                {
                    flagInterrupt = true;
                    controlBoxScript.flagInterruptKey = false;
                }
                if (controlBoxScript.flagClearControlKey)
                {
                    controlBoxScript.flagClearControlKey = false;
                    if (!runPressed)
                        accumulators[controlRegister].setUInt64(0);
                }
            }

            if (manualPressed)
            {
                if (!isRetryingForFrame)
                {
                    if (controlBoxScript.flagPrimaryInputKey)
                    {
                        controlBoxScript.flagPrimaryInputKey = false;
                        isPrimaryInput = true;
                        primaryInputPC = 0;
                    }
                    if (controlBoxScript.flagAcceptInputKey)
                    {
                        controlBoxScript.flagAcceptInputKey = false;
                        flagInputAccepted = true;
                        acceptedInput = new BCD10(kbValue);
                    }
                }

                isRetryingForFrame = isRetrying;

                if (flagContinue)
                {
                    flagContinue = false;
                    processOneOrder();
                }
                timeLeft = 0.0f;
            }
            else
            {
                isRetryingForFrame = isRetrying;

                if (flagInterrupt)
                {
                    setMainStore_forced(1, accumulators[controlRegister].toHex64());
                    accumulators[controlRegister].setUInt64(2);
                }
                else
                {
                    if (flagContinue)
                    {
                        flagContinue = false;
                        if (!runPressed)
                        {
                            processOneOrder();
                        }
                        isStopped99 = false;
                        isStoppedKB = false;
                        isStopped6910 = false;
                        isStoppedByError = false;
                        timeLeft = 0.0f;
                    }
                    else
                    {
                        if (runPressed)
                        {
                            timeLeft += Time.deltaTime;
                        
                            while ((!isStopped99 && !isStoppedKB && !isStopped6910 && !isStoppedByError)
                                    && timeLeft > 0.0f)
                            {
                                processOneOrder();
                            }
                        }
                        else
                        {
                            timeLeft = 0.0f;
                        }
                    }
                }
            }
        }

        updateDisplay();
    }

    private int lastDisplayed;

    private void updateDisplay()
    {
        int a = lastDisplayed;

        if (!isRetryingForFrame)
        {
            a = controlBoxScript.selectedAccumulator();
            lastDisplayed = a;
        }

        BCD10 displayed = new BCD10();
        displayed.digits[0] = accB;
        displayed.digits[1] = accA;
        displayed.digits[2] = siriusOpcodeLow;
        displayed.digits[3] = siriusOpcodeHigh;
        for (int i = 0; i < 6; i++)
            displayed.digits[i + 4] = effectiveOperand.digits[i];

        displayScript0.setNumber(displayed);

        accumulators[0].set(accumulators[a]);
        displayScript1.setNumber(accumulators[0]);

        if (isPrimaryInput)
            indicatorPIScript.on();
        else
            indicatorPIScript.off();
        if (flagOVR)
            indicatorOVRScript.on();
        else
            indicatorOVRScript.off();
        if (isStopped99)
            indicator99WScript.on();
        else
            indicator99WScript.off();
        if (isStoppedKB)
            indicatorKSScript.on();
        else
            indicatorKSScript.off();
        if (isStopped6910)
            indicator6910Script.on();
        else
            indicator6910Script.off();
        if (isRBusy)
            indicatorRBScript.on();
        else
            indicatorRBScript.off();
        if (isPBusy)
            indicatorPBScript.on();
        else
            indicatorPBScript.off();
    }

    private void addCycles(uint i)
    {
        delayLinePos = (delayLinePos + i) % 50;
        timeLeft -= i * cycleTime;
    }

    private void waitForDelayLine(UInt64 addr)
    {
        uint a = (uint) (addr % 50);
        uint cycles = (uint)(((a * 3) % 50) + 50 - delayLinePos) % 50;
        addCycles(cycles);
    }

    private UInt64 getMainStore(UInt64 addr)
    {
        uint i = (uint) (addr % mainStoreModulus);
        waitForDelayLine(i);
        return mainStore[i];
    }

    private void setMainStore_forced(UInt64 addr, UInt64 data)
    {
        uint i = (uint)(addr % mainStoreModulus);
        waitForDelayLine(i);
        mainStore[i] = data;
    }

    private void setMainStore(UInt64 addr, UInt64 data)
    {
        uint i = (uint)(addr % mainStoreModulus);
        waitForDelayLine(i);
        if (i >= 200 || (i < 100 && controlBoxScript.flagFree0) || (i >= 100 && i < 200 && controlBoxScript.flagFree100))
            mainStore[i] = data;
    }

    public void processOneOrder()
    {
        if (isPrimaryInput)
        {
            if (!isWaitingR)
            {
                currentOrder.setHex64(primaryInputMicroProgram[primaryInputPC]);
                primaryInputPC = primaryInputPC + 1;
            }
        } 
        else
        {
            if (manualPressed)
            {
                if (!isRetrying)
                {
                    if (flagInputAccepted)
                    {
                        flagInputAccepted = false;
                        currentOrder.set(acceptedInput);
                    }
                    else
                    {
                        currentOrder.set(controlBoxScript.getKeyboard());

                    }
                }
            }
            else
            {
                if (!isRetrying)
                {
                    currentOrder.setHex64(getMainStore(accumulators[controlRegister].toUInt64()));
                    if (kbWaitPressed &&
                        accumulators[controlRegister].toUInt64() % mainStoreModulus == kbValue.toUInt64() % mainStoreModulus)
                        isStoppedKB = true;
                    accumulators[controlRegister].add(new BCD10(1)); // OK? 
                }
            }
        }

        accB = currentOrder.digits[0];
        accA = currentOrder.digits[1];
        siriusOpcodeLow = currentOrder.digits[2];
        siriusOpcodeHigh = currentOrder.digits[3]; 
        siriusOperand.setUInt32((UInt32) (currentOrder.toUInt64() / 10000L));
        effectiveOperand.setBCD6(siriusOperand);

        addCycles(3);
        switch (siriusOpcodeHigh)
        {
            case 0:
                processOrder0();
                break;
            case 1:
                processOrder1();
                break;
            case 2:
                processOrder2();
                break;
            case 3:
                processOrder3();
                break;
            case 4:
                processOrder4();
                break;
            case 5:
                processOrder5();
                break;
            case 6:
                processOrder6();
                break;
            case 7:
                processOrder7();
                break;
            case 8:
                processOrder8();
                break;
            case 9:
                processOrder9();
                break;
            default:
                break;
        }

        isRetryingForFrame &= isRetrying;
    }

    private void processOrder0()
    {
        BCD10 b;
        if (accB > 0)
            effectiveOperand.add(accumulators[accB]);
        if (accA == 0)
            accumulators[0].set(controlBoxScript.getKeyboard());

        b = new BCD10(effectiveOperand);
        if (siriusOpcodeLow >= 5)
            b.shiftLeft(4);

        switch (siriusOpcodeLow)
        {
            case 0:
            case 5:
                accumulators[accA].add(b);
                flagOVR |= BCD10.flagOVR;
                break;
            case 1:
            case 6:
                accumulators[accA].sub(b);
                flagOVR |= BCD10.flagOVR;
                break;
            case 2:
            case 7:
                accumulators[accA].negSub(b);
                flagOVR |= BCD10.flagOVR;
                break;
            case 3:
            case 8:
                accumulators[accA].negAdd(b);
                flagOVR |= BCD10.flagOVR;
                break;
            case 4:
            case 9:
                accumulators[accA].set(b); 
                break;
        }
    }

    private void processOrder1()
    {
        BCD10 b = new BCD10();
        if (siriusOpcodeLow < 5)
        {
            if (accB > 0)
                effectiveOperand.add(accumulators[accB]);
            b.setHex64(getMainStore(effectiveOperand.toUInt64()));
            if (accA == 0)
                accumulators[0].set(controlBoxScript.getKeyboard());
        }
        else
        {
            if (accB == 0)
                b.set(controlBoxScript.getKeyboard());
            else
                b.set(accumulators[accB]);
        }

        switch (siriusOpcodeLow)
        {
            case 0:
                accumulators[accA].add(b);
                flagOVR |= BCD10.flagOVR;
                break;
            case 1:
                accumulators[accA].sub(b);
                flagOVR |= BCD10.flagOVR;
                break;
            case 2:
                accumulators[accA].negSub(b);
                flagOVR |= BCD10.flagOVR;
                break;
            case 3:
                accumulators[accA].negAdd(b);
                flagOVR |= BCD10.flagOVR;
                break;
            case 4:
                accumulators[accA].set(b);
                break;
            case 5:
                accumulators[accA].add(b);
                break;
            case 6:
                accumulators[accA].sub(b);
                break;
            case 7:
                accumulators[accA].negSub(b);
                break;
            case 8:
                accumulators[accA].negAdd(b);
                break;
            case 9:
                accumulators[accA].set(b);
                break;
        }
    }

    private void processOrder2()
    {
        BCD10 b;
        bool wasNegative;
        if (accB > 0)
            effectiveOperand.add(accumulators[accB]);
        if (accA == 0)
            accumulators[0].set(controlBoxScript.getKeyboard());

        b = new BCD10(effectiveOperand);
        if (siriusOpcodeLow >= 5)
            b.shiftLeft(4);
        wasNegative = accumulators[accA].isNegative();

        flagOVR |= (accumulators[accA].digits[9] != 0 && accumulators[accA].digits[9] != 9);

        for (int i = 9; i > 0; i--)
            accumulators[accA].digits[i] = accumulators[accA].digits[i - 1];
        accumulators[accA].digits[0] = 0;

        flagOVR |= (wasNegative != accumulators[accA].isNegative());

        switch (siriusOpcodeLow)
        {
            case 0:
            case 5:
                accumulators[accA].add(b);
                flagOVR |= BCD10.flagOVR;
                break;
            case 1:
            case 6:
                accumulators[accA].sub(b);
                flagOVR |= BCD10.flagOVR;
                break;
            case 2:
            case 7:
                accumulators[accA].negSub(b);
                flagOVR |= BCD10.flagOVR;
                break;
            case 3:
            case 8:
                accumulators[accA].negAdd(b);
                flagOVR |= BCD10.flagOVR;
                break;
            case 4:
            case 9:
                accumulators[accA].digits[0] = b.digits[9];
                break;
        }
    }

    private void processOrder3()
    {
        BCD10 b = new BCD10();
        bool wasNegative = accumulators[accA].isNegative();
        bool shiftOVR;
        byte msdB;

        shiftOVR = (accumulators[accA].digits[9] != 0 && accumulators[accA].digits[9] != 9);

        if (siriusOpcodeLow < 5)
        {
            if (accB > 0)
                effectiveOperand.add(accumulators[accB]);
            b.setHex64(getMainStore(effectiveOperand.toUInt64()));
            if (accA == 0)
                accumulators[0].set(controlBoxScript.getKeyboard());
            flagOVR |= shiftOVR | (wasNegative != accumulators[accA].isNegative());

        }
        else
        {
            if (accB == 0)
                b.set(controlBoxScript.getKeyboard());
            else
                b.set(accumulators[accB]);        
        }

        msdB = b.msd(); // in case accA = accB ...

        for (int i = 9; i > 0; i--)
            accumulators[accA].digits[i] = accumulators[accA].digits[i - 1];
        accumulators[accA].digits[0] = 0;

        switch (siriusOpcodeLow)
        {
            case 0:
                accumulators[accA].add(b);
                flagOVR |= BCD10.flagOVR;
                break;
            case 1:
                accumulators[accA].sub(b);
                flagOVR |= BCD10.flagOVR;
                break;
            case 2:
                accumulators[accA].negSub(b);
                flagOVR |= BCD10.flagOVR;
                break;
            case 3:
                accumulators[accA].negAdd(b);
                flagOVR |= BCD10.flagOVR;
                break;
            case 4:
                accumulators[accA].digits[0] = msdB;
                break;
            case 5:
                accumulators[accA].add(b);
                break;
            case 6:
                accumulators[accA].sub(b);
                break;
            case 7:
                accumulators[accA].negSub(b);
                break;
            case 8:
                accumulators[accA].negAdd(b);
                break;
            case 9:
                accumulators[accA].digits[0] = msdB;
                break;
        }
    }

    private void processOrder4()
    {
        byte lsd, lsdN;

        if (accA == 0)
            accumulators[0].set(controlBoxScript.getKeyboard());

        if (siriusOpcodeLow >= 5)
        {
            if (accB > 0)
                effectiveOperand.add(accumulators[accB]);
        }
        lsdN = effectiveOperand.digits[0];

        lsd = accumulators[accA].digits[0];
        for (int i = 0; i < 9; i++)
            accumulators[accA].digits[i] = accumulators[accA].digits[i + 1];

        if (siriusOpcodeLow < 5) 
        {
            if (accumulators[accA].digits[8] < 5)
            {
                accumulators[accA].digits[9] = 0;
            }
            else
            {
                accumulators[accA].digits[9] = 9;
            }
        }
        else
        {
            accumulators[accA].digits[9] = lsdN;
        }
        switch (siriusOpcodeLow)
        {
            case 0:
            case 1:
            case 2:
            case 3:
                if (lsd >= 5)
                    accumulators[accA].add(new BCD10(1));
                break;
            case 4:
                break;
            case 5:
            case 6:
            case 7:
            case 8:
                if (lsd >= 5)
                    accumulators[accA].add(new BCD10(1));
                break;
            case 9:
                break;
        }
    }

    private int delayLineDistance(UInt64 a0, UInt64 a1)
    {
        int b0 = (int)(a0 % 50), b1 = (int)(a1 % 50);

        b0 = (b0 * 3) % 50;
        b1 = (b1 * 3) % 50;

        return (b1 + 50 - b0) % 50;
    }

    private void setPC(BCD10 addr)
    {
        if (isPrimaryInput)
        {
            primaryInputPC = (int) addr.toUInt64();
        }
        else
        {
            accumulators[controlRegister].set(addr);
        }
    }

    private void processOrder5()
    {
        BCD10 a = new BCD10();
        if (accA == 0)
            a.set(controlBoxScript.getKeyboard());
        else
            a.set(accumulators[accA]);

        if (accB > 0)
            effectiveOperand.add(accumulators[accB]);

        switch (siriusOpcodeLow)
        {
            case 0: // DUMMY
                break;
            case 1:
                if (a.msd() != 0)
                    setPC(effectiveOperand);
                break;
            case 2:
                if (!a.isZero())
                    setPC(effectiveOperand);
                break;
            case 3:
                if (flagOVR)
                    setPC(effectiveOperand);
                flagOVR = false;
                break;
            case 4:
                if (a.isNegative())
                    setPC(effectiveOperand);
                break;
            case 5:
                setPC(effectiveOperand);
                break;
            case 6:
                if (a.msd() == 0)
                    setPC(effectiveOperand);
                break;
            case 7:
                if (a.isZero())
                    setPC(effectiveOperand);
                break;
            case 8:
                if (!flagOVR)
                    setPC(effectiveOperand);
                flagOVR = false;
                break;
            case 9:
                if (!a.isNegative())
                    setPC(effectiveOperand);
                break;
        }
    }

    private void processOrder6()
    {
        if (accA == 0 && siriusOpcodeLow != 4)
            accumulators[0].set(controlBoxScript.getKeyboard());

        if (accB > 0)
            effectiveOperand.add(accumulators[accB]);

        switch (siriusOpcodeLow)
        {
            case 0:
            case 1:
            case 2:
            case 3:
                setMainStore(effectiveOperand.toUInt64(), accumulators[accA].toHex64());
                break;
            case 4:
                setMainStore(effectiveOperand.toUInt64(), 0);
                break;
            case 9:
                if (accA == controlRegister)
                {
                    isStopped6910 = true;
                }
                else
                {
                    accumulators[accA].set(accumulators[controlRegister]);
                    setPC(effectiveOperand);
                }
                break;
            case 5:
                for(int i = 0; i < 10; i++)
                    accumulators[accA].digits[i] = mask65_67[effectiveOperand.digits[i], accumulators[accA].digits[i]];
                break;
            case 6:
                for (int i = 0; i < 10; i++)
                    accumulators[accA].digits[i] = mask66_68[effectiveOperand.digits[i], accumulators[accA].digits[i]];
                break;
            case 7:
                for (int i = 4; i < 10; i++)
                    accumulators[accA].digits[i] = mask65_67[effectiveOperand.digits[i-4], accumulators[accA].digits[i]];
                break;
            case 8:
                for (int i = 0; i < 4; i++)
                    accumulators[accA].digits[i] = 0; 
                for (int i = 4; i < 10; i++)
                    accumulators[accA].digits[i] = mask66_68[effectiveOperand.digits[i-4], accumulators[accA].digits[i]];
                break;
        }
    }

    private byte tape5BitToBCD2(byte b)
    {
        byte c = 0;
        bool isOdd = false;
        if ((b & 1) != 0)
        {
            c += 4;
            isOdd = !isOdd;
        }
        if ((b & 2) != 0)
        {
            c += 5;
            isOdd = !isOdd;
        }
        if ((b & 4) != 0)
        {
            c += 0x10;
            isOdd = !isOdd;
        }
        if ((b & 8) != 0)
        {
            c += 0x20;
            isOdd = !isOdd;
        }
        if (((b & 0x10) == 0 && !isOdd) || ((b & 0x10) != 0 && isOdd))
        {
            c += 0x50;
        }
        return c;
    }

    private byte toTapeCode(BCD10 bcd10)
    {
        byte c = 0;
        bool isOdd = false;
        switch(bcd10.digits[8])
        {
            default:
                break;
            case 4:
                c += 1;
                isOdd = !isOdd;
                break;
            case 5:
            case 6:
            case 7:
            case 8:
                c += 2;
                isOdd = !isOdd;
                break;
            case 9:
                c += 3;
                break;
        }
        switch (bcd10.digits[9])
        {
            default:
                break;
            case 1:
            case 6:
                c += 4;
                isOdd = !isOdd;
                break;
            case 2:
            case 7:
                c += 8;
                isOdd = !isOdd;
                break;
            case 3:
            case 8:
                c += 12;
                break;
        }

        if ((!isOdd && bcd10.digits[9] < 5) || (isOdd && bcd10.digits[9] >= 5))
            c += 16;
        return c;

    }

    private uint multDivCycles;

    private int[] mult_result;
    private void mult(BCD10 b, BCD10 c, bool negate)
    {
        mult_result = new int[20];
        int carry;

        multDivCycles = 22;
        for (int i = 0; i < 10; i++)
            multDivCycles += (uint) b.digits[i] * 2;

        for (int i = 0; i < 10; i++)
            for (int j = 0; j < 10; j++)
                mult_result[i + j] += b.digits[i] * c.digits[j];

        carry = 0;
        for (int i=0; i < 20; i++)
        {
            mult_result[i] += carry;
            carry = mult_result[i] / 10;
            mult_result[i] = mult_result[i] % 10;
        }
        if (negate)
        {
            carry = 0;
            for (int i = 0; i < 20; i++)
            {
                int n = 10 - mult_result[i] - carry;
                carry = 1 - n / 10;
                mult_result[i] = n % 10;
            }
        }
    }


    private bool sub20(byte[] x, byte[] y, int shift) // subtract y from x: if negative, put it back and return false. 
    {
        int borrow = 0;
        for (int i = 0; i < 10; i++)
        {
            int n = 10 + x[i + shift] - y[i] - borrow;
            x[i + shift] = (byte) (n % 10);
            borrow = 1 - n / 10;
        }
        if (borrow == 0)
            return true;
        if (shift < 10 && x[shift + 10] > 0)
        {
            x[shift + 10]--;
            return true;
        }

        int carry = 0;
        for (int i = 0; i < 10; i++)
        {
            int n = x[i + shift] + y[i] + carry;
            x[i + shift] = (byte) (n % 10);
            carry = n / 10;
        }
        return false;
    }

    private byte[] div_quotient, div_remainder;
    private bool division(BCD10 b, BCD10 c, BCD10 d) // Divide (b, c) by d. Return true if the quotient >= 1
    {
        div_remainder = new byte[20];
        div_quotient = new byte[10];

        for (int i = 0; i < 10; i++)
            div_remainder[i + 10] = b.digits[i];
        for (int i = 0; i < 10; i++)
            div_remainder[i] = c.digits[i];
        for (int i = 0; i < 10; i++)
            div_quotient[i] = 0;

        multDivCycles = 22;

        if (sub20(div_remainder, d.digits, 10))
            return true;

        for (int s = 9; s >= 0; s--)
        {
            for (int i = 0; i < 10; i++)
            {
                if (sub20(div_remainder, d.digits, s))
                {
                    div_quotient[s]++;
                }
                else
                {
                    break;
                }
            }
        }

        for (int i = 0; i < 10; i++)
            multDivCycles += (uint) div_quotient[i] * 2;

        return false;
    }

    private void readInputChannel()
    {
        TapeScript readScript;
        byte b = 0xff;

        if (effectiveOperand.toUInt64() % 5 == 0)
        {
            if ((effectiveOperand.toUInt64() % 10 == 0))
                readScript = tapeReaderScriptA;
            else
                readScript = tapeReaderScriptB;
            isRBusy = readScript.isRBusy();
            if (!isRBusy)
            {
                b = readScript.read();
            }
        }
        if ((effectiveOperand.toUInt64() % 10 == 1))
        {
            isRBusy = printerScript.isRBusy();
            if (!isRBusy)
            {
                b = printerScript.read();
            }
        }
        if (b == 0xff)
        {
            isRetrying = true;
            isWaitingR = true;
        }
        else
        {
            isRetrying = false;
            isWaitingR = false;
            accumulators[accA].setHex64(((UInt64)tape5BitToBCD2(b)) << 32);
        }
    }

    private void writeOutputChannel()
    {
        if (effectiveOperand.toUInt64() % 10 == 0)
        {
            isWaitingP = !tapePunchScript.punch(toTapeCode(accumulators[accA]));
        }
        if (effectiveOperand.toUInt64() % 10 == 1)
        {
            isWaitingP = !printerScript.print(toTapeCode(accumulators[accA]));
        }
        isRetrying = isWaitingP;
    }

    private void processOrder7()
    {
        BCD10 bcd0, bcd1;
        bool negate;

        if (accB > 0)
            effectiveOperand.add(accumulators[accB]);

        switch (siriusOpcodeLow)
        {
            case 1:
            case 6:
                readInputChannel();
                break;
            case 2:
            case 7:
                writeOutputChannel();
                break;
            case 3:
            case 8: 
                if (!isRetrying || isWaitingR)
                {
                    readInputChannel();
                }
                if (!isWaitingR)
                {
                    writeOutputChannel();
                }
                break;
            case 4: // accB=0: KB 
                // displayed operand: 0?
                negate = false;

                if (accB == 0)
                    accumulators[accB].set(controlBoxScript.getKeyboard());
                if (accumulators[accB].isNegative())
                {
                    bcd0 = new BCD10(0);
                    negate = !negate;
                    bcd0.sub(accumulators[accB]);
                }
                else
                {
                    bcd0 = new BCD10(accumulators[accB]);
                }
                mult(bcd0, accumulators[9], negate);
                for (int i = 0; i < 10; i++)
                    accumulators[9].digits[i] = (byte) mult_result[i];
                for (int i = 0; i < 10; i++)
                    accumulators[accA].digits[i] = (byte)mult_result[i + 10];
                break;
            case 9:
                // displayed operand: 0?
                negate = false;

                if (accB == 0)
                    accumulators[accB].set(controlBoxScript.getKeyboard());
                if (accumulators[accB].isNegative())
                {
                    bcd0 = new BCD10(0);
                    negate = !negate;
                    bcd0.sub(accumulators[accB]);
                }
                else
                {
                    bcd0 = new BCD10(accumulators[accB]);
                }
                if (accumulators[9].isNegative())
                {
                    bcd1 = new BCD10(0);
                    negate = !negate;
                    bcd1.sub(accumulators[9]);
                }
                else
                {
                    bcd1 = new BCD10(accumulators[9]);
                }
                mult(bcd0, bcd1, negate);
                for (int i = 0; i < 10; i++)
                    accumulators[9].digits[i] = (byte)mult_result[i];
                for (int i = 0; i < 10; i++)
                    accumulators[accA].digits[i] = (byte)mult_result[i + 10];
                break;
            case 0:
                flagOVR |= division(accumulators[accA], accumulators[9], accumulators[accB]);
                flagOVR |= (div_quotient[9] >= 5);
                for (int i = 0; i < 10; i++)
                    accumulators[9].digits[i] = div_quotient[i];
                for (int i = 0; i < 10; i++)
                    accumulators[accA].digits[i] = div_remainder[i];
                break;
            case 5:
                flagOVR |= division(accumulators[accA], accumulators[9], accumulators[accB]);
                for (int i = 0; i < 10; i++)
                    accumulators[9].digits[i] = div_quotient[i];
                for (int i = 0; i < 10; i++)
                    accumulators[accA].digits[i] = div_remainder[i];
                break;
        }
        if (siriusOpcodeLow == 0 || siriusOpcodeLow == 4 || siriusOpcodeLow == 5 || siriusOpcodeLow == 9)
        {
            addCycles(multDivCycles);
        }
    }

    private void processOrder8()
    {
        // dummy instructions
    }

    private void processOrder9()
    {
        if (isPrimaryInput)
        {
            isPrimaryInput = false;
            flagContinue = true;
        }
        else
        {
            isStopped99 = true;
        }
    }
}
