//using System.Collections.Generic;
//using UnityEditor;
//using UnityEngine;

//public class AddMesh : MonoBehaviour {
    

//    // Use this for initialization
//    void Start () {
//        /*MeshFilter meshFilter = GetComponent<MeshFilter>();
//        if (meshFilter == null)
//        {
//            Debug.LogError("MeshFilter not found! - (AddMesh Class)");
//            return;
//        }*/

//        //Vector3 p0 = new Vector3(0, 0, 0);
//        //Vector3 p1 = new Vector3(1, 0, 0);
//        //Vector3 p2 = new Vector3(0.5f, 0, Mathf.Sqrt(0.75f));
//        //Vector3 p3 = new Vector3(0.5f, Mathf.Sqrt(0.75f), Mathf.Sqrt(0.75f) / 3);
//        /**
//        Vector3 p0 = new Vector3(0, 0, 0);
//        Vector3 p1 = new Vector3(1, 0, 0);
//        Vector3 p2 = new Vector3(0, 1, 0);
//        Vector3 p3 = new Vector3(1, 1, 0);

//        Vector3 p4 = new Vector3(0, 0, 1);
//        Vector3 p5 = new Vector3(1, 0, 1);
//        Vector3 p6 = new Vector3(0, 1, 1);
//        Vector3 p7 = new Vector3(1, 1, 1);

//        /*Mesh mesh = meshFilter.sharedMesh;
//        if (mesh == null)
//        {
//            meshFilter.mesh = new Mesh();
//            mesh = meshFilter.sharedMesh;
//        }
//        //mesh.Clear();
//        Vector3[] tmp = new Vector3[] { p0, p1, p2, p3, p4, p5, p6, p7 };
//        int[] diem = new int[]{
//            0,1,2,
//            1,2,3,
//            1,3,5,
//            3,5,7,
//            5,6,7,
//            4,5,6,
//            0,2,4,
//            2,4,6,
//            0,1,4,
//            1,4,5,
//            2,3,6,
//            3,6,7
//        };
//        */

//        Vector3[] tmp = new Vector3[]
//        {
//            new Vector3(-1, 1, 0),
//            new Vector3( 1, 1, 0),
//            new Vector3( 1,-1, 0),
//            new Vector3(-1,-1, 0)
//        };
//        int[] diem = new int[]
//        {
//            0, 1, 2,
//            0, 2, 3
//        };

//        Vector2[] uv = new Vector2[]
//        {
//            new Vector2(0,0),
//            new Vector2(0, 1),
//            new Vector2(1, 1),
//            new Vector2(1, 0)
//        };

//        Mesh mesh = new Mesh();
//        mesh.vertices = tmp;
//        mesh.triangles = diem;
//        mesh.uv = uv;


//        mesh.RecalculateBounds();
//        mesh.RecalculateNormals();
//        if (!GetComponent<MeshFilter>())
//            gameObject.AddComponent<MeshFilter>();
//        if (!GetComponent<MeshRenderer>())
//            gameObject.AddComponent<MeshRenderer>();
//        gameObject.GetComponent<MeshFilter>().mesh = mesh;

//    }
	
//	// Update is called once per frame
//	void Update () {
//	}
//}
