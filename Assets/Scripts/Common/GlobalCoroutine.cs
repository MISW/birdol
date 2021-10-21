using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Coroutineをnewキーワードを用いたクラス(= Without Monobehaivior)で使うためのクラス
/// </summary>
public class GlobalCoroutine : MonoBehaviour
{
    private static GlobalCoroutine _i;

    public static Coroutine StartCoroutineG(IEnumerator r)
    {
        if (_i == null)
        {
            var obj = new GameObject("GlobalCoroutineManager");
            _i = obj.AddComponent<GlobalCoroutine>();
            GameObject.DontDestroyOnLoad(_i);
        }
        return _i.StartCoroutine(_i.routine(r));
    }

    public IEnumerator routine(IEnumerator u)
    {
        yield return StartCoroutine(u);
    }
}
