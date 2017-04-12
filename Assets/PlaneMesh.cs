using System.Collections.Generic;
using UnityEngine;

public class PlaneMesh : MonoBehaviour{
    // Use this for initialization
    void Start()
    {
        /*MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null)
        {
            Debug.LogError("MeshFilter not found! - (AddMesh Class)");
            return;
        }

        //Vector3 p0 = new Vector3(0, 0, 0);
        //Vector3 p1 = new Vector3(1, 0, 0);
        //Vector3 p2 = new Vector3(0.5f, 0, Mathf.Sqrt(0.75f));
        //Vector3 p3 = new Vector3(0.5f, Mathf.Sqrt(0.75f), Mathf.Sqrt(0.75f) / 3);
        */
        int row = 30;
        int col = 30;
        int rowCell = row - 1;
        int colCell = col - 1;
        
        Vector3[][] p = new Vector3[row][];
        Vector3[] tmp = new Vector3[row * col];

        for (int i = 0; i< row; i++)
        {
            p[i] = new Vector3[col];

            for (int j=0; j< col; j++)
            {
                p[i][j] = new Vector3(i, j, 0);
                print("(" + i + "," + j + ",0)");
                tmp[i * row + j] = p[i][j];
            }
        }

        /*Mesh mesh = meshFilter.sharedMesh;
        if (mesh == null)
        {
            meshFilter.mesh = new Mesh();
            mesh = meshFilter.sharedMesh;
        }
        mesh.Clear();
        //mesh.vertices = new Vector3[] { p[0], p[1], p[2], p[3] };
        mesh.vertices = tmp;
        */
        int[] diem = new int[6 * rowCell * colCell];

        print("giá trị đỉnh tạo: " + tmp.Length);
        int index = 0;
        for (int i = 0; i < rowCell; i++)
        {

            for (int j = 0; j < colCell; j++)
            {
                int hangi = i * row + j;
                print("int hangi = " + i + " * " + row + " + " + j + " = " + hangi);
                int hangicong1 = hangi + col;
                print("int hangicong1 = " + hangi + " + " + col + " = " + hangicong1);
                //mat phang 1
                diem[index++] = hangi;      diem[index++] = hangicong1 + 1;      diem[index++] = hangicong1;

                //mat phang 2
                diem[index++] = hangi;      diem[index++] = hangi + 1;      diem[index++] = hangicong1 + 1;
                print("i: " + hangi + ", i+1: " + hangicong1);
                print("(" + hangi + "," + (hangicong1 + 1) + "," + hangicong1 + ")");
                print("(" + hangi + "," + (hangicong1 + 1) + "," + (hangi + 1) + ")");
            }
        }

        Mesh mesh = new Mesh();
        mesh.vertices = tmp;
        mesh.triangles = diem;

        /*mesh.triangles = new int[]{
            0,1,2,
            0,2,3,
            2,1,3,
            0,3,1
        };*/

        mesh.RecalculateNormals();
        //mesh.RecalculateBounds();
        if (!GetComponent<MeshFilter>())
            gameObject.AddComponent<MeshFilter>();
        if (!GetComponent<MeshRenderer>())
            gameObject.AddComponent<MeshRenderer>();
        gameObject.GetComponent<MeshFilter>().mesh = mesh;
    }   
}