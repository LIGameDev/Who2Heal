using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils {

    public static int Clamp(this int num, int min, int max)
    {
        if (num < min)
        {
            num = min;
        }

        if (num > max)
        {
            num = max;
        }

        return num; 
    }

}
