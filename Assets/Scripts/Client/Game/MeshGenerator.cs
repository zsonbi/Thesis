using System.Collections.Generic;
using UnityEngine;

public static class MeshGenerator
{
    private static Vector3[] baseVertices = new Vector3[]
         {
            new Vector3(0, 0, 0),
            new Vector3(1, 0, 0),
            new Vector3(1, 1, 0),
            new Vector3(0, 1, 0),
            new Vector3(0, 1, 1),
            new Vector3(1, 1, 1),
            new Vector3(1, 0, 1),
            new Vector3(0, 0, 1),
         };

    private static int[] baseTriangles = new int[]
        {
            0, 2, 1, //face front
	        0, 3, 2,
            2, 3, 4, //face top
	        2, 4, 5,
            1, 2, 5, //face right
	        1, 5, 6,
            0, 7, 4, //face left
	        0, 4, 3,
            5, 4, 7, //face back
	        5, 7, 6,
            0, 6, 7, //face bottom
	        0, 1, 6
        };

    // Start is called before the first frame update

    //private void CreateShape()
    //{
    //    vertices = new Vector3[]
    //    {
    //        new Vector3 (0, 0, 0),
    //        new Vector3 (1, 0, 0),
    //        new Vector3 (1, 1, 0),
    //        new Vector3 (0, 1, 0),
    //        new Vector3 (0, 1, 1),
    //        new Vector3 (1, 1, 1),
    //        new Vector3 (1, 0, 1),
    //        new Vector3 (0, 0, 1),
    //    };
    //    triangles = new int[]
    //    {
    //        0, 2, 1, //face front
    //     0, 3, 2,
    //        2, 3, 4, //face top
    //     2, 4, 5,
    //        1, 2, 5, //face right
    //     1, 5, 6,
    //        0, 7, 4, //face left
    //     0, 4, 3,
    //        5, 4, 7, //face back
    //     5, 7, 6,
    //        0, 6, 7, //face bottom
    //     0, 1, 6
    //    };
    //}

    public static Mesh CreateMultiShape(List<Vector3> positions)
    {
        Vector3[] newVertices = new Vector3[positions.Count * 8];
        int[] newTriangles = new int[positions.Count * 36];

        for (int i = 0; i < positions.Count; i++)
        {
            for (int j = 0; j < baseVertices.Length; j++)
            {
                newVertices[i * 8 + j] = baseVertices[j] + positions[i];
            }
        }

        for (int i = 0; i < positions.Count; i++)
        {
            for (int j = 0; j < 36; j++)
            {
                newTriangles[i * 36 + j] = baseTriangles[j] + 8 * i;
            }
        }

        Mesh newMesh = new Mesh();
        newMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        newMesh.vertices = newVertices;
        newMesh.triangles = newTriangles;
        newMesh.Optimize();
        newMesh.RecalculateBounds();

        return newMesh;
    }
}