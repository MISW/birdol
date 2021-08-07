using UnityEngine;

public class GachaManager : MonoBehaviour
{
    public string[] R5, R4, R3, R2, R1; //多次元配列だとエディタ―でいじれないので、レア度ごとに配列を作成
    float[] probVec = { 0.01f, 0.05f, 0.2f, 0.3f, 0.44f };

    public void onButtonPressed10()
    {
        int resR = 5; //当選レア度
        int res; //当選アイテム番号
        for (int i = 0; i < 10; i++)
        {
            float f = Random.Range(0, 1f);
            float prob = 0;
            for (int j = 0; j < 5; j++)
            {
                prob += probVec[j];
                if (f <= prob)
                {
                    resR = j; //当選レア度を決定（0が最高、4が最低であることに注意）
                    break;
                }
            }
            switch (resR)
            {
                case 0:
                    res = Random.Range(0, R5.Length);
                    Debug.Log(R5[res]);
                    break;

                case 1:
                    res = Random.Range(0, R4.Length);
                    Debug.Log(R4[res]);
                    break;

                case 2:
                    res = Random.Range(0, R3.Length);
                    Debug.Log(R3[res]);
                    break;

                case 3:
                    res = Random.Range(0, R2.Length);
                    Debug.Log(R2[res]);
                    break;

                case 4:
                    res = Random.Range(0, R1.Length);
                    Debug.Log(R1[res]);
                    break;
            }
        }
    }
}
