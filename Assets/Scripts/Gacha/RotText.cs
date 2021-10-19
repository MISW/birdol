using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class RotText : UIBehaviour, IMeshModifier
{
    [SerializeField] string[] NoRotChars;
    Text text;
    public new void OnValidate()
    {
        base.OnValidate();

        var graphics = base.GetComponent<Graphic>();
        if (graphics != null)
        {
            graphics.SetVerticesDirty();
        }

        text = GetComponent<Text>();

    }

    public void ModifyMesh(Mesh mesh) { }
    public void ModifyMesh(VertexHelper verts) //UIのメッシュ
    {
        if (!this.IsActive())
        {
            return;
        }

        List<UIVertex> vertexList = new List<UIVertex>(); //キャンバスの頂点リスト
        verts.GetUIVertexStream(vertexList); //三角形の頂点作成

        ModifyVertices(vertexList);

        verts.Clear(); //頂点除去
        verts.AddUIVertexTriangleStream(vertexList); //三角形リスト作成
    }

    void ModifyVertices(List<UIVertex> vertexList)
    {
        for (int i = 0, vertexListCount = vertexList.Count; i < vertexListCount; i += 6) //1文字頂点6つ
        {

            bool isCharEqual = false;
            for (int j = 0; j < NoRotChars.Length; j++)
            {
                if (text.text.Substring(i / 6, 1).Equals(NoRotChars[j]))
                {
                    isCharEqual = true;
                    break;
                }
            }

            if (!isCharEqual)
            {
                var center = Vector2.Lerp(vertexList[i].position, vertexList[i + 3].position, 0.5f); //対角線上の2つの頂点の中間点の座標（文字の中央）
                for (int r = 0; r < 6; r++)
                {
                    var element = vertexList[i + r]; //文字の各頂点
                    var pos = element.position - (Vector3)center; //各頂点と中心点との差
                    var newPos = new Vector2(
                        pos.x * Mathf.Cos(90 * Mathf.Deg2Rad) - pos.y * Mathf.Sin(90 * Mathf.Deg2Rad),
                        pos.x * Mathf.Sin(90 * Mathf.Deg2Rad) + pos.y * Mathf.Cos(90 * Mathf.Deg2Rad)
                    ); //90°回転

                    element.position = (Vector3)(newPos + center); //回転適用
                    vertexList[i + r] = element; //上書き
                }
            }
        }
    }
}