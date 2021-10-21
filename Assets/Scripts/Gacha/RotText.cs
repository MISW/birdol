using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class RotText : UIBehaviour, IMeshModifier
{
    [SerializeField] string[] RotChars, AlignChars;
    Text text;
    public new void OnValidate()
    {
        base.OnValidate();

        Graphic graphic = base.GetComponent<Graphic>();
        if (graphic != null) graphic.SetVerticesDirty();
        
        text = GetComponent<Text>();
    }

    public void ModifyMesh(Mesh mesh) { }
    public void ModifyMesh(VertexHelper verts)
    {
        if (!this.IsActive()) return;

        List<UIVertex> vertexList = new List<UIVertex>();
        verts.GetUIVertexStream(vertexList);

        ModifyVertices(vertexList);

        verts.Clear();
        verts.AddUIVertexTriangleStream(vertexList);
    }

    void ModifyVertices(List<UIVertex> vertexList)
    {
        for (int i = 0, vertexListCount = vertexList.Count; i < vertexListCount; i += 6)
        {
            for (int j = 0; j < RotChars.Length; j++)
            {
                if (text.text.Substring(i / 6, 1).Equals(RotChars[j]))
                {
                    Vector2 centerPos = (vertexList[i].position + vertexList[i + 3].position) / 2;

                    for (int k = 0; k < 6; k++)
                    {
                        UIVertex uvs = vertexList[i + k];
                        Vector2 pos = (Vector2)uvs.position - centerPos;
                        Vector2 newPos = new Vector2(
                            pos.x * Mathf.Cos(90 * Mathf.Deg2Rad) - pos.y * Mathf.Sin(90 * Mathf.Deg2Rad),
                            pos.x * Mathf.Sin(90 * Mathf.Deg2Rad) + pos.y * Mathf.Cos(90 * Mathf.Deg2Rad)
                        );

                        uvs.position = (Vector3)(newPos + centerPos);
                        vertexList[i + k] = uvs;
                    }
                }
            }

            for (int j = 0; j < AlignChars.Length; j++)
            {
                if (text.text.Substring(i / 6, 1).Equals(AlignChars[j]))
                {
                    Vector2 centerPos = (vertexList[i].position + vertexList[i + 3].position) / 2;
                    Vector2 pos = new Vector2(150, 150);

                    for (int k = 0; k < 6; k++)
                    {
                        UIVertex uvs = vertexList[i + k];

                        uvs.position += (Vector3)pos;
                        vertexList[i + k] = uvs;
                    }
                }

            }
        }
    }
}