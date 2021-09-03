using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SceneVisor : MonoBehaviour
{
    public virtual IEnumerator Init(gamestate before)
    {
        print($"{this.gameObject.scene.name}:Init");

        yield break;
    }

    public virtual void Updater()
    {
        print($"{this.gameObject.scene.name}:Updater");
    }

    public virtual IEnumerator Finalizer(gamestate after)
    {
        print($"{this.gameObject.scene.name}:Finalizer");

        yield break;
    }

    public void TestFunction()
    {
        StartCoroutine(Tester());        
    }

    private void Update()
    {
        if (testflag)
        {
            Updater();
        }

    }

    bool testflag = false;
    IEnumerator Tester()
    {
        gamestate Pre_GameState = gamestate.Undefined;
        {
            yield return StartCoroutine(Init(Pre_GameState));
            testflag = true;
            yield break;
        }
    }

}
