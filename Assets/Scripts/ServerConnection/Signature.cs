using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class Signature {
    /// <summary>
    /// Create RSA KeyPair
    /// </summary>
    /// <returns> Tuple containing PrivateKey XML and PublicKey XML </returns>
    public Tuple<string, string> CreateRsaKey() {
        int size = 1024;
        RSACryptoServiceProvider mCryptServiceProvider = new RSACryptoServiceProvider(size);
        // key pair (private, public)
        Tuple<string, string> Keys = new Tuple<string, string>(mCryptServiceProvider.ToXmlString(true), mCryptServiceProvider.ToXmlString(false));

        return Keys;
    }

    /// <summary>
    /// Sign to string using given PrivateKey
    /// </summary>
    /// <returns> Generated Signature </returns>
    public string SignToString(string privateKey, string baseString) {
        RSACryptoServiceProvider mCryptServiceProvider = new RSACryptoServiceProvider();
        mCryptServiceProvider.FromXmlString(privateKey);
        byte[] signature = mCryptServiceProvider.SignData(Encoding.UTF8.GetBytes(baseString), HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        return BitConverter.ToString(signature).Replace("-", "").ToLower();
    }

    /// <summary>
    /// Format rewuest data to signature base string
    /// </summary>
    /// <returns> Tuple containing signature base string and timestamp
    public Tuple<string, string> FormatSignBaseString(object requestBody){
        string bodyString = JsonUtility.ToJson(requestBody);
        DateTime now_ts = DateTime.Now;
        string timestamp = now_ts.ToString("yyyy-MM-dd-HH-mm-ss");
        string baseString = "v2:" + timestamp + ":" + bodyString;
        return new Tuple<string, string>(baseString, timestamp);
    }
}
