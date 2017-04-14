using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoHoMeshScript : MonoBehaviour
{
    public Collider collider;

    private int[] buffer1;
    private int[] buffer2;
    private int[] vertexIndices;

    private Mesh mesh;

    private Vector3[] vertices;
    //private Vector3[] normals ;

    public float dampner = 0.999f;
    public float maxWaveHeight = 5.0f;

    public int splashForce = 10000;

    //public int slowdown = 20;
    //private int slowdownCount = 0;
    private bool swapMe = true;


    public int cols;
    public int rows;

    /// <summary>
    /// Ma trận mặt nước 1 chiều
    /// </summary>
    bool[] maTranBool1Chieu;
    bool[][] maTranBoBao;
    int chenhLechMaTran;
    int themMeshCanh = 20;

    // Use this for initialization
    void Start()
    {
        //Tạo ma trân mặt hồ
        DuLieu.getInstance().taoMaTranMatHo();

        //Tạo ma trận bờ bao
        taoMaTranBoBao();

        //Xác định kích thước ma trận ao và chênh lệch rộng và dài
        int max, min ;
        if (DuLieu.maTranBool.Length > DuLieu.maTranBool[0].Length)
        {
            max = DuLieu.maTranBool.Length;
            min = DuLieu.maTranBool[0].Length;
            chenhLechMaTran = max - min;

            int themMeshTong = themMeshCanh * 2;
            maTranBoBao = new bool[max + themMeshTong][];

            for (int j = 0; j < themMeshCanh; j++)
            {
                maTranBoBao[j] = new bool[max + themMeshTong];
            }

            for (int j = themMeshCanh; j < themMeshCanh + max; j++)
            {
                maTranBoBao[j] = new bool[max + themMeshTong];

                int begin = themMeshCanh + chenhLechMaTran / 2;
                int end = themMeshCanh + chenhLechMaTran / 2 + min; 
                for (int index = begin; index < end; index++)
                {
                    print("**: " + index);
                    maTranBoBao[j][index] = DuLieu.maTranBool[j - themMeshCanh][index - begin];
                }
            }

            for (int j = themMeshCanh + max; j < max + themMeshTong; j++)
            {
                maTranBoBao[j] = new bool[max + themMeshTong];
            }
        } else
        {
            min = DuLieu.maTranBool.Length;
            max = DuLieu.maTranBool[0].Length;
            chenhLechMaTran = max - min;

            int themMeshTong = themMeshCanh * 2;
            maTranBoBao = new bool[max + themMeshTong][];
            
            int begin = themMeshCanh + chenhLechMaTran / 2;
            int end = themMeshCanh + chenhLechMaTran / 2 + min;

            for (int j = 0; j < begin; j++)
            {
                maTranBoBao[j] = new bool[max + themMeshTong];
            }
            for (int j = begin; j < end; j++)
            {
                maTranBoBao[j] = new bool[max + themMeshTong];

                for (int index = themMeshCanh; index < themMeshCanh + max; index++)
                {
                    maTranBoBao[j][index] = DuLieu.maTranBool[j - begin][index - themMeshCanh];
                }
            }
            for (int j = end; j < max + themMeshTong; j++)
            {
                maTranBoBao[j] = new bool[max + themMeshTong];
            }
        }

        print("canh = " + max + themMeshCanh * 2);

        //Xây dựng mesh
        BuildMesh(max + themMeshCanh * 2);

        //Lấy mesh và thiết lập
        MeshFilter mf = (MeshFilter)gameObject.GetComponent(typeof(MeshFilter));
        mesh = mf.mesh;
        //int[] triangles = mesh.GetTriangles(mesh.subMeshCount); ??? Nếu muốn xóa cái kia

        collider = GetComponent<Collider>();

        vertices = mesh.vertices;
        print("vertices length: " + vertices.Length);
        buffer1 = new int[vertices.Length];
        print("tổng: " + vertices.Length);

        buffer2 = new int[vertices.Length];

        Bounds bounds = mesh.bounds;

        float xStep = (bounds.max.x - bounds.min.x) / cols;
        float zStep = (bounds.max.z - bounds.min.z) / rows;

        vertexIndices = new int[vertices.Length];

        int i = 0;
        for (i = 0; i < vertices.Length; i++)
        {
            vertexIndices[i] = -1;
            buffer1[i] = 0;
            buffer2[i] = 0;
        }

        //// this will produce a list of indices that are sorted the way I need them to 
        //// be for the algo to work right
        for (i = 0; i < vertices.Length; i++)
        {
            float column = ((vertices[i].x - bounds.min.x) / xStep);// + 0.5;
            float rowz = ((vertices[i].z - bounds.min.z) / zStep);// + 0.5;
            float position = (rowz * (cols + 1)) + column;// + 0.5f;
            if (vertexIndices[(int)position] >= 0) print("smash");
            vertexIndices[(int)position] = i;
        }
        //splashAtPoint(cols / 2, rows / 2);
    }

    private void BuildMesh(int max)
    {
        cols = max;
        rows = max;

        float k = 1f / DuLieu.getInstance().getDistance();

        int row = rows + 1;
        int col = cols + 1;

        int rowCell = row - 1;
        int colCell = col - 1;

        Vector3[][] p = new Vector3[row][];
        Vector3[] tmp = new Vector3[row * col];
        maTranBool1Chieu = new bool[row * col];//Ma trận xác định ao
        Vector2[] uv = new Vector2[row * col];

        //------- vertices ---------
        int i = 0;
        for (i = 0; i < row; i++)
        {
            p[i] = new Vector3[col];

            for (int j = 0; j < col; j++)
            {
                p[i][j] = new Vector3(i / k, 0, j / k);
                //print("(" + i + "," + j + ",0)");
                tmp[i * row + j] = p[i][j];
                uv[i * row + j] = new Vector2((float)i / row, (float)j / col);

                if (i < maTranBoBao.Length && j < maTranBoBao[0].Length && maTranBoBao[i][j])
                    maTranBool1Chieu[i * row + j] = true;
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
        //------ triangles -------------
        int[] diem = new int[6 * rowCell * colCell];

        //print("giá trị đỉnh tạo: " + tmp.Length);
        int index = 0;
        for (i = 0; i < rowCell; i++)
        {

            for (int j = 0; j < colCell; j++)
            {
                int hangi = i * row + j;
                //print("int hangi = " + i + " * " + row + " + " + j + " = " + hangi);
                int hangicong1 = hangi + col;

                //Loaoij bỏ các trangles không thuộc ao
                int hangiTest = j * row + i;
                int hangicong1Test = hangiTest + col;
                if (maTranBool1Chieu[hangiTest] && maTranBool1Chieu[hangicong1Test] && maTranBool1Chieu[hangiTest + 1] && maTranBool1Chieu[hangicong1Test + 1])
                {
                    diem[index++] = 0; diem[index++] = 0; diem[index++] = 0;
                    diem[index++] = 0; diem[index++] = 0; diem[index++] = 0;
                    continue;
                }
                //print("int hangicong1 = " + hangi + " + " + col + " = " + hangicong1);
                //mat phang 1
                diem[index++] = hangi; diem[index++] = hangicong1 + 1; diem[index++] = hangicong1;

                //mat phang 2
                diem[index++] = hangi; diem[index++] = hangi + 1; diem[index++] = hangicong1 + 1;
                //print("i: " + hangi + ", i+1: " + hangicong1);
                //print("(" + hangi + "," + (hangicong1 + 1) + "," + hangicong1 + ")");
                //print("(" + hangi + "," + (hangicong1 + 1) + "," + (hangi + 1) + ")");
            }
        }

        //---- uv --------

        Mesh mesh2 = new Mesh();
        mesh2.vertices = tmp;
        mesh2.triangles = diem;
        mesh2.uv = uv;

        /*mesh.triangles = new int[]{
            0,1,2,
            0,2,3,
            2,1,3,
            0,3,1
        };*/

        mesh2.RecalculateNormals();
        mesh2.RecalculateBounds();
        if (!GetComponent<MeshFilter>())
            gameObject.AddComponent<MeshFilter>();
        if (!GetComponent<MeshRenderer>())
            gameObject.AddComponent<MeshRenderer>();
        if (!GetComponent<MeshCollider>())
            gameObject.AddComponent<MeshCollider>();

        gameObject.GetComponent<MeshFilter>().mesh = mesh2;

        MeshCollider meshc = gameObject.GetComponent<MeshCollider>();
        meshc.sharedMesh = mesh2;
        meshc.tag = "vacham";

        gameObject.GetComponent<Collider>().tag = "vacham";

        //------------------
        //Rigidbody rigid = GetComponent<Rigidbody>();
        /*
        Rigidbody rigidVat = vatThe.GetComponent<Rigidbody>();
        Vector3 vat = vatThe.transform.position;
        rigidVat.AddForceAtPosition(new Vector3(0,20,0), new Vector3(vat.x, 0, vat.z));
        */
        print("Khởi tạo bề mặt");
    }

    private void taoMaTranBoBao()
    {
       // DuLieu.maTranBool
    }
}
