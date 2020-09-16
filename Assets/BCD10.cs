using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UIElements;

public class BCD10 
{
    public byte[] digits;
    const int DIGITS = 10;

    public static bool flagOVR;

    public BCD10()
    {
        digits = new byte[DIGITS];
    }

    public void setHex64(UInt64 n)
    {
        for (int i=0; i < DIGITS; i++)
        {
            digits[i] = (byte)(n & 0xf);
            n = (n >> 4);
        }
    }

    public void setUInt64(UInt64 n)
    {
        for (int i = 0; i < DIGITS; i++)
        {
            digits[i] = (byte)(n % 10);
            n = n / 10;
        }
    }

    public BCD10(UInt64 n)
    {
        digits = new byte[DIGITS];
        setUInt64(n); 
    }

    public BCD10(BCD10 b)
    {
        digits = new byte[DIGITS];
        set(b);
    }

    public void set(BCD10 b)
    {
        for (int i = 0; i < DIGITS; i++)
            digits[i] = b.digits[i];
    }
    public void setBCD6(BCD6 bcd6) // with sign extension
    {
        for (int i = 0; i < 6; i++)
        {
            digits[i] = bcd6.digits[i]; 
        }
        if (bcd6.isNegative())
            for (int i = 6; i < DIGITS; i++)
                digits[i] = 9;
        else
            for (int i = 6; i < DIGITS; i++)
                digits[i] = 0;
    }

    public BCD10(BCD6 bcd6) // with sign extension
    {
        digits = new byte[DIGITS];
        setBCD6(bcd6);
    }

    public bool isNegative()
    {
        return (digits[DIGITS - 1] >= 5);
    }

    public byte msd()
    {
        return digits[DIGITS - 1];
    }

    public bool isEqual(BCD10 bcd10)
    {
        for (int i = 0; i < DIGITS; i++)
            if (digits[i] != bcd10.digits[i])
                return false;
        return true;
    }
    public bool isZero()
    {
        for (int i = 0; i < DIGITS; i++)
            if (digits[i] != 0)
                return false;
        return true;
    }


    public UInt64 toUInt64()
    {
        UInt64 result = 0;
        for (int i = 0; i < DIGITS; i++)
        {
            result = result * 10;
            result = result + digits[DIGITS - 1 - i];
        }
        return result;
    }
    public Int64 toInt64()
    {
        Int64 result = 0;
        for (int i = 0; i < DIGITS; i++)
        {
            result = result * 10;
            result = result + digits[DIGITS - 1 - i];
        }
        if (isNegative())
            return result - 10000000000L;
        else
            return result;
    }
    public UInt64 toHex64()
    {
        UInt64 result = 0;
        for (int i = 0; i < DIGITS; i++)
        {
            result = (result << 4);
            result = result + digits[DIGITS - 1 - i];
        }
        return result;
    }


    public void add(BCD10 b)
    {
        int c = 0;
        bool wasNegative = isNegative();

        for (int i = 0; i < DIGITS; i++)
        {
            int n = digits[i] + b.digits[i] + c;
            c = n / 10;
            digits[i] = (byte) (n % 10);
        }
        flagOVR = ((wasNegative && b.isNegative() && !isNegative()) ||
                    (!wasNegative && !b.isNegative() && isNegative()));
    }
    public void sub(BCD10 b)
    {
        int c = 0;
        bool wasNegative = isNegative();

        for (int i = 0; i < DIGITS; i++)
        {
            int n = 10 + digits[i] - b.digits[i] - c;
            c = 1 - n / 10;
            digits[i] = (byte)(n % 10);
        }
        flagOVR = ((wasNegative && !b.isNegative() && !isNegative()) ||
                    (!wasNegative && b.isNegative() && isNegative()));
    }
    public void negAdd(BCD10 b)
    {
        int c = 0;
        bool wasNegative = isNegative();

        for (int i = 0; i < DIGITS; i++)
        {
            int n = 10 + b.digits[i] - digits[i] - c;
            c = 1 - n / 10;
            digits[i] = (byte)(n % 10);
        }
        flagOVR = ((!wasNegative && b.isNegative() && !isNegative()) ||
                    (wasNegative && !b.isNegative() && isNegative()));
    }
    public void negSub(BCD10 b)
    {
        int c = 0;
        bool wasNegative = isNegative();

        for (int i = 0; i < DIGITS; i++)
        {
            int n = 20 - digits[i] - b.digits[i] - c;
            c = 2 - n / 10;
            digits[i] = (byte)(n % 10);
        }
        flagOVR = ((!wasNegative && !b.isNegative() && !isNegative()) ||
                    (wasNegative && b.isNegative() && isNegative()));
    }

    public void shiftLeft(int n)
    {
        for (int i = DIGITS - 1; i >= 0; i--)
        {
            if (i >= n)
            {
                digits[i] = digits[i - n];
            }
            else
            {
                digits[i] = 0;
            }
        }
    }
}
