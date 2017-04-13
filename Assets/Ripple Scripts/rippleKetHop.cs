using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rippleKetHop : MonoBehaviour {

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

    //Các thông số động
    /// <summary>
    /// 1/ khoangCach là Hệ số đỉnh trên mesh, tỷ lệ thuận với số đỉnh. Khoảng cách cần nên là số hữu hạn 1 chữ số thập phân
    /// </summary>
    float khoangCach = 0.5f;
    /// <summary>
    /// Khoảng cách mỗi đơn vị ao, tỷ lệ tuận với kích thước ao
    /// </summary>
    float distance = 1f;

    public int cols;
    public int rows;

    /// <summary>
    /// Ma trận mặt nước
    /// </summary>
    bool[][] maTranBool;
    bool[] maTranBool1Chieu;

    // Use this for initialization
    void Start()
    {
        //Xác định ma trận mặt hồ
        taoMaTranMatHo();

        int max = maTranBool.Length > maTranBool[0].Length? maTranBool.Length + 2 : maTranBool[0].Length + 2;
        print("max: " + max);

        BuildMesh( max);

        //L?y mesh
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

    void BuildMesh(int max)
    {
        cols = max;
        rows = max;

        float k = 1f / distance;

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

                if (i<maTranBool.Length && j<maTranBool[0].Length && maTranBool[i][j])
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
                if ( !(maTranBool1Chieu[hangiTest] && maTranBool1Chieu[hangicong1Test] && maTranBool1Chieu[hangiTest + 1] && maTranBool1Chieu[hangicong1Test + 1]))
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


    void splashAtPoint(int x, int y)
    {
        print("2");
        if (x <= 1 || y <= 1 || x >= cols - 2 || y >= rows - 2)
        {
            print("x = " + x + ", y = " + y);
            return;
        }
        if (!maTranBool1Chieu[(y * (cols + 1)) + x])
        {
            print("loại");
            return;
        }
        int position = 99999999;
        try
        {
            print("3");
            position = ((y * (cols + 1)) + x);
            if (maTranBool1Chieu[position])
                buffer1[position] = splashForce;
            if (maTranBool1Chieu[position - 1])
                buffer1[position - 1] = splashForce;
            if (maTranBool1Chieu[position + 1])
                buffer1[position + 1] = splashForce;
            if (maTranBool1Chieu[position + (cols + 1)])
                buffer1[position + (cols + 1)] = splashForce;
            if (maTranBool1Chieu[position + (cols + 1) + 1])
                buffer1[position + (cols + 1) + 1] = splashForce;
            if (maTranBool1Chieu[position + (cols + 1) - 1])
                buffer1[position + (cols + 1) - 1] = splashForce;
            if (maTranBool1Chieu[position - (cols + 1)])
                buffer1[position - (cols + 1)] = splashForce;
            if (maTranBool1Chieu[position - (cols + 1) + 1])
                buffer1[position - (cols + 1) + 1] = splashForce;
            if (maTranBool1Chieu[position - (cols + 1) - 1])
                buffer1[position - (cols + 1) - 1] = splashForce;

            //buffer1[position] = splashForce;
            //buffer1[position - 1] = splashForce;
            //buffer1[position + 1] = splashForce;
            //buffer1[position + (cols + 1)] = splashForce;
            //buffer1[position + (cols + 1) + 1] = splashForce;
            //buffer1[position + (cols + 1) - 1] = splashForce;
            //buffer1[position - (cols + 1)] = splashForce;
            //buffer1[position - (cols + 1) + 1] = splashForce;
            //buffer1[position - (cols + 1) - 1] = splashForce;
        }
        catch (System.Exception)
        {
            print("" + (position + 1));
            throw;
        }
    }

    // Update is called once per frame
    void Update()
    {
        checkInput();

        int[] currentBuffer;
        if (swapMe)
        {
            // process the ripples for this frame
            processRipples(buffer1, buffer2);
            currentBuffer = buffer2;
        }
        else
        {
            processRipples(buffer2, buffer1);
            currentBuffer = buffer1;
        }
        swapMe = !swapMe;
        // apply the ripples to our buffer
        Vector3[] theseVertices = new Vector3[vertices.Length];
        int vertIndex;
        int i = 0;

        //độ cao sóng
        for (i = 0; i < currentBuffer.Length; i++)
        {
            vertIndex = vertexIndices[i];
            if (!maTranBool1Chieu[i])
            {
                currentBuffer[i] = 0;
                //theseVertices[vertIndex] = vertices[vertIndex];
                //theseVertices[vertIndex].y = -100;
                //continue;
            }
                theseVertices[vertIndex] = vertices[vertIndex];
                theseVertices[vertIndex].y += (currentBuffer[i] * 1.0f / splashForce) * maxWaveHeight;
        }
        mesh.vertices = theseVertices;


        // swap buffers		
    }

    void checkInput()
    {
        if (Input.GetMouseButton(0))
        {
            print("1");
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
            {
                if (hit.collider.tag == "vacham")
                {
                    print("hit child collider");
                }
                else
                {
                    Bounds bounds = mesh.bounds;
                    float xStep = (bounds.max.x - bounds.min.x) / cols;
                    float zStep = (bounds.max.z - bounds.min.z) / rows;
                    float xCoord = (bounds.max.x - bounds.min.x) * (hit.textureCoord.x - 1);
                    float zCoord = (bounds.max.z - bounds.min.z) * (1 - hit.textureCoord.y);
                    float column = ( cols + xCoord / xStep);// + 0.5;
                    float row = (rows - zCoord / zStep);// + 0.5;
                    print("distance: " + hit.distance);
                    print("hit x: " + hit.textureCoord.x + ", hit y: " + hit.textureCoord.y + ", (" + (int)column + "," + (int)row + ")");
                    splashAtPoint((int)column, (int)row);
                    //print("tag = " + hit.collider.tag);
                }
            }
        }
    }


    void processRipples(int[] source, int[] dest)
    {
        int x = 0;
        int y = 0;
        int position = 0;
        for (y = 1; y < rows - 1; y++)
        {
            for (x = 1; x < cols; x++)
            {
                position = (y * (cols + 1)) + x;
                int tmp = 0;
                if (maTranBool1Chieu[position - 1])
                    tmp += source[position - 1];
                if (maTranBool1Chieu[position + 1])
                    tmp += source[position + 1];
                if (maTranBool1Chieu[position - (cols + 1)])
                    tmp += source[position - (cols + 1)];
                if (maTranBool1Chieu[position + (cols + 1)])
                    tmp += source[position + (cols + 1)];

                dest[position] = ((tmp >> 1) - dest[position]);

                //dest[position] = (((source[position - 1] +
                //                     source[position + 1] +
                //                     source[position - (cols + 1)] +
                //                     source[position + (cols + 1)]) >> 1) - dest[position]);
                dest[position] = (int)(dest[position] * dampner);
            }
        }
    }

    private bool taoMaTranMatHo()
    {
        DuLieu dulieu = new DuLieu();

        //hienThiConsoleThongSoManHinh(dulieu);

        //Lấy tọa độ (xMin, yMin) và (xMax, yMax)
        float xMIN = 999999, yMIN = 999999;
        float xMAX = -999999, yMAX = -999999;
        for (int i = 0; i < dulieu.screen.Length; i++)
        {
            ScreenThongSo screen = dulieu.screen[i];

            if (screen.xMin < xMIN)
                xMIN = screen.xMin;
            if (screen.xMax > xMAX)
                xMAX = screen.xMax;
            if (screen.yMin < yMIN)
                yMIN = screen.yMin;
            if (screen.yMax > yMAX)
                yMAX = screen.yMax;
        }
        //hienThiConsoleXYMinMax(xMin, xMax, yMin, yMax);

        //Tọa độ bắt đầu ma trận
        float xSTART = lamTronTheoTungDoan(xMIN, khoangCach);
        float ySTART = lamTronTheoTungDoan(yMIN, khoangCach);

        //Kich thuoc ma tran
        int numRow = Mathf.RoundToInt((xMAX - xSTART) / khoangCach);
        int numCol = Mathf.RoundToInt((yMAX - ySTART) / khoangCach);
        int maxIndexMaTran = numRow > numCol ? numRow : numCol;

        if (maxIndexMaTran > 200)
        {
            khoangCach += 0.1f;
            taoMaTranMatHo();
            return true;
        }

        //Tạo ma trận bool
        maTranBool = new bool[numRow][];
        for (int i = 0; i < numRow; i++)
        {
            maTranBool[i] = new bool[numCol];
        }
        //hienThiConsoleNumRowNumCol(numRow, numCol);

        //Xác định những điểm nào thuộc ma trận
        for (int i = 0; i < dulieu.screen.Length; i++)
        {
            ScreenThongSo screen = dulieu.screen[i];
            
            //xStart, yStart
            //float xStartScreen = lamTronTheoTungDoan(screen.xMin, khoangCach);
            //float yStartScreen = lamTronTheoTungDoan(screen.yMin, khoangCach);

            int indexXMin = Mathf.RoundToInt((screen.xMin - xSTART) / khoangCach);
            int indexYMin = Mathf.RoundToInt((screen.yMin - ySTART) / khoangCach);

            int indexXMax = Mathf.RoundToInt((screen.xMax - xSTART) / khoangCach);
            int indexYMax = Mathf.RoundToInt((screen.yMax - ySTART) / khoangCach);

            //Ghi nhận vào ma trận Boolean
            for (int j = indexXMin; j < indexXMax; j++)
            {
                for (int k = indexYMin; k < indexYMax; k++)
                {
                    maTranBool[j][k] = true;
                }
            }
        }
        return true;
    }

    private float lamTronTheoTungDoan(float number, float distance)
    {
        return Mathf.RoundToInt(number / distance) * distance;
    }


    //--------------- Hiển thị Console -----------------------

    private void hienThiConsoleThongSoManHinh(DuLieu dulieu)
    {
        for (int i = 0; i < dulieu.screen.Length; i++)
        {
            print("(" + dulieu.screen[i].centerPoint.x + ", " + dulieu.screen[i].centerPoint.y + ")"
                + ", " + dulieu.screen[i].width
                + ", " + dulieu.screen[i].height);
        }
    }

    private void hienThiConsoleXYMinMax(float xMin, float xMax, float yMin, float yMax)
    {
        print("xMin: " + xMin); print("xMax: " + xMax); print("yMin: " + yMin); print("yMax: " + yMax);
    }

    private void hienThiConsoleNumRowNumCol(int numRow, int numCol)
    {
        print("numRow: " + numRow);
        print("numCol: " + numCol);
    }

    private void hienThiConsoleMaTranBool(bool[][] maTranBool)
    {
        //Đảo ma trận
        bool[][] maTran = new bool[maTranBool[0].Length][];
        for (int i = 0; i < maTran.Length; i++)
        {
            maTran[i] = new bool[maTranBool.Length];
            for (int j = 0; j < maTran[i].Length; j++)
            {
                maTran[i][j] = maTranBool[j][maTranBool[0].Length - 1 - i];
            }
        }

        for (int i = 0; i < maTran.Length; i++)
        {
            string str = "";
            for (int j = 0; j < maTran[i].Length; j++)
            {
                if (maTran[i][j])
                {
                    str += "1    ";
                } else
                {
                    str += "0    ";
                }
            }
            print(str);
        }
    }
}
