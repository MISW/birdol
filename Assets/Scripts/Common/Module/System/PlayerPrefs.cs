using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrefs : UnityEngine.PlayerPrefs
{
    public static void SetUint(string key, uint value)
    {
        string v = value.ToString();
        PlayerPrefs.SetString(key,v);
    }
    public static uint GetUint(string key)
    {
        try
        {
            string v = PlayerPrefs.GetString(key);
            uint value = System.Convert.ToUInt32(v);
            return value;
        }
        catch
        {
            return 0;
        }
        
    }
}
