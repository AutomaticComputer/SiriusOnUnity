using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BCD6
{
    public byte[] digits;
    const int DIGITS = 6;
    public BCD6()
    {
        digits = new byte[DIGITS];
    }

    public void setUInt32(UInt32 n)
    {
        for (int i = 0; i < DIGITS; i++)
        {
            digits[i] = (byte)(n % 10);
            n = n / 10;
        }
    }

    public BCD6(UInt32 n)
    {
        digits = new byte[DIGITS];
        setUInt32(n);
    }

    public bool isNegative()
    {
        return (digits[DIGITS - 1] >= 5);
    }

    public UInt32 toUInt32()
    {
        UInt32 result = 0;
        for (int i = 0; i < DIGITS; i++)
        {
            result = result * 10;
            result = result + digits[DIGITS - 1 - i];
        }
        return result;
    }
    public Int32 toInt32()
    {
        Int32 result = 0;
        for (int i = 0; i < DIGITS; i++)
        {
            result = result * 10;
            result = result + digits[DIGITS - 1 - i];
        }
        if (isNegative())
            return result - 1000000;
        else
            return result;
    }

}
