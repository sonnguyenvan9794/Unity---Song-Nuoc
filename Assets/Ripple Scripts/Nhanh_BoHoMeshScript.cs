using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nhanh_BoHoMeshScript : MonoBehaviour {
    
    bool[][] maTranTmp;

// Use this for initialization
    void Start ()
    {
        //TẠO CÁC KHỐI NỀN
        List<Vector4> listKhoiNen = layCacKhoiNen();

        //Có lỗi
        //List<Vector4> mepAos = layMepAo(listKhoiNen);
        //print("=== Tập hợp mép ao : " + mepAos.Count + " =======");
        //for (int i = 0; i < mepAos.Count; i++)
        //{
        //    print("x = " + mepAos[i].x + ", y = " + mepAos[i].y + ", z = " + mepAos[i].z + ", w = " + mepAos[i].w);
        //}
        //print("--------------------------------------------");

        //print("=== tạo nền bờ bao =======");

        //Cách 1
        //buildMeshBoBao(listKhoiNen);
        //Cách 2
        buildCubeNen(listKhoiNen);
        buildTuong(listKhoiNen);

        //Đặt vị trí
        gameObject.transform.position = new Vector3(0, DuLieu.getInstance().getDoCaoBoBao(), 0);
    }

    private List<Vector4> layMepAo(List<Vector4> list)
    {
        List<Vector4> mepAos = new List<Vector4>();

        for (int i = 0; i < list.Count; i++)
        {
            bool[][] maTranBoolCopy = DuLieu.maTranBool;
            int chieuRongMeshBoAo = DuLieu.getInstance().getChieuRongMeshBoAo();
            int x = (int)list[i].x - chieuRongMeshBoAo;
            int y = (int)list[i].y - chieuRongMeshBoAo;
            int z = (int)list[i].z - chieuRongMeshBoAo;
            int w = (int)list[i].w - chieuRongMeshBoAo;

            //print(maTranBoolCopy.Length + " | " + maTranBoolCopy[0].Length);
            //print(chieuRongMeshBoAo);
            //print(x + " | " + y + " | " + z + " | " + w);

            //Kiểm tra bên dưới (x,y) -> (z,y)
            if (y != (0 - chieuRongMeshBoAo))
            {
                if (maTranBoolCopy[x][y]
                 && maTranBoolCopy[z][y])
                {
                    print("+");
                    //Nếu cả 2 đều true và k phải và mép phải bờ thì nó chính là mép ao. Thêm vào list mép ao
                    Vector4 tmp = new Vector4();
                    tmp.x = x; tmp.y = y;
                    tmp.z = z; tmp.y = y;
                    mepAos.Add(tmp);
                }
            }
            else
            {
                continue;
            }
            //Kiểm tra bên trên (x,w) -> (z,w)
            if (w != (maTranBoolCopy[0].Length + chieuRongMeshBoAo - 1))
            {
                if (maTranBoolCopy[x][w]
                 && maTranBoolCopy[z][w])
                {
                    print("+");
                    Vector4 tmp = new Vector4();
                    tmp.x = x; tmp.w = w;
                    tmp.z = z; tmp.w = w;
                    mepAos.Add(tmp);
                }
            }
            else
            {
                continue;
            }
            //Kiểm tra bên phải (z,y) -> (z,w)
            if (z != (maTranBoolCopy.Length + chieuRongMeshBoAo - 1)
                 && maTranBoolCopy[z][y]
                 && maTranBoolCopy[z][w])
            {
                Vector4 tmp = new Vector4();
                tmp.z = z; tmp.y = y;
                tmp.z = z; tmp.w = w;
                mepAos.Add(tmp);
            }
            else
            {
                continue;
            }
            //Kiểm tra bên trái (x,y) -> (x,w)
            if (x != (0 - chieuRongMeshBoAo)
                 && maTranBoolCopy[x][ y]
                 && maTranBoolCopy[ x][ w])
            {
                Vector4 tmp = new Vector4();
                tmp.x = x; tmp.y = y;
                tmp.x = x; tmp.w = w;
                mepAos.Add(tmp);
            }
            else
            {
                continue;
            }
        }
        return mepAos;
    }

    //================== tạo các khổi nền =====================

    /// <summary>
    /// Lấy danh sách các khối nền với tọa độ đơn vị
    /// </summary>
    /// <returns></returns>
    private List<Vector4> layCacKhoiNen()
    {
        //sao 1 ma trận từ ma trận Bool để xác định các Cube tạo nền bờ ao
        maTranTmp = new bool[DuLieu.maTranBool.Length][];
        for (int j = 0; j < maTranTmp.Length; j++)
        {
            maTranTmp[j] = new bool[DuLieu.maTranBool[0].Length];
            for (int m = 0; m < maTranTmp[j].Length; m++)
            {
                maTranTmp[j][m] = DuLieu.maTranBool[j][m];
            }
        }
        //Xác định đường viền = true
        for (int j = 0; j < maTranTmp.Length; j++)
        {
            maTranTmp[j][0] = true;
            maTranTmp[j][maTranTmp[0].Length - 1] = true;
        }
        for (int j = 0; j < maTranTmp[0].Length; j++)
        {
            maTranTmp[0][j] = true;
            maTranTmp[maTranTmp.Length - 1][j] = true;
        }
        //Xác định các khối nền
        List<Vector4> listKhoiNen = new List<Vector4>();

        for (int j = 1; j < maTranTmp.Length - 1; j++)
        {
            for (int m = 1; m < maTranTmp[j].Length - 1; m++)
            {
                //nếu gặp 1 điểm false
                if (maTranTmp[j][m] == false)
                {
                    Vector4 vector2 = duyetKhoi(j, m);
                    listKhoiNen.Add(vector2);
                }
            }
        }

        //Dịch khối này lên (themMeshCanh, themMeshCanh);
        int themMeshCanh = DuLieu.getInstance().getChieuRongMeshBoAo();
        for (int i = 0; i < listKhoiNen.Count; i++)
        {
            Vector4 t = listKhoiNen[i];
            listKhoiNen[i] = new Vector4(t.x + themMeshCanh, t.y + themMeshCanh, t.z + themMeshCanh, t.w + themMeshCanh);
        }

        //Thêm các khối nền biên
        int dai = maTranTmp.Length;
        int rong = maTranTmp[0].Length;
        Vector4 tmp1 = new Vector4(0, 0, themMeshCanh, rong + 2 * themMeshCanh - 1);
        Vector4 tmp2 = new Vector4(themMeshCanh, 0, themMeshCanh + dai, themMeshCanh);
        Vector4 tmp3 = new Vector4(themMeshCanh, themMeshCanh + rong - 1, themMeshCanh + dai, 2 * themMeshCanh + rong - 1);
        Vector4 tmp4 = new Vector4(themMeshCanh + dai - 1, 0, 2 * themMeshCanh + dai - 1, 2 * themMeshCanh + rong - 1);

        listKhoiNen.Add(tmp1);
        listKhoiNen.Add(tmp2);
        listKhoiNen.Add(tmp3);
        listKhoiNen.Add(tmp4);

        //Xác định lại tọa độ chính xác:
        //for (int i = 0; i < listKhoiNen.Count; i++)
        //{
        //    listKhoiNen[i] = listKhoiNen[i] * DuLieu.getInstance().getDistance();
        //}

        //Thêm các khối đường bao

        //Thử in ra các khối
        print("======= Các khối nền bao==========");
        foreach (Vector4 item in listKhoiNen)
        {
            print("x = " + item.x + ", y = " + item.y + ", z = " + item.z + ", w = " + item.w);
        }
        print("----------------------------------");

        return listKhoiNen;
    }

    /// <summary>
    /// Tạo Nền cho bờ ao
    /// </summary>
    /// <param name="listKhoiNen">Vector4 gồm tọa độ đầu (x,y) và tọa độ sau (z,w)</param>
    private void buildMeshBoBao(List<Vector4> list)
    {
        int soDinh = list.Count * 4;
        Vector3[] vertices = new Vector3[ soDinh];   //vertices
        Vector2[] uv = new Vector2[ soDinh];        //uv
        int[] triangles = new int[6 * list.Count];//triangles


        //Khởi tạo Lấy các đỉnh
        float dai = (float) DuLieu.maTranBool.Length + 2 * DuLieu.getInstance().getChieuRongMeshBoAo();
        float rong = (float) DuLieu.maTranBool[0].Length + 2 * DuLieu.getInstance().getChieuRongMeshBoAo();

        float distance = DuLieu.getInstance().getDistance(); //Độ dài mỗi đơn vị
        float yValue = DuLieu.getInstance().getDoCaoBoBao(); //Lấy độ cao bờ bao, tuy nhiên bị ảnh hưởng bởi độ cao của gameObject
        
        int count1 = 0; //Biến chạy vertices và uv
        int count2 = 0; //Biến chạy triangles
        //Lấy các đỉnh vertices và uv
        for (int i = 0; i < list.Count; i++)
        {
            //tạo vertices và uv
            Vector4 tmp = list[i] * distance; // Lấy tọa độ thực từ tọa độ đơn vị
            //Gán theo thứ tự (0,0), (1,0), (1,1), (0,1)
            vertices[count1] = new Vector3(tmp.x, yValue, tmp.y);
            uv[count1] = new Vector2( tmp.x / dai, tmp.y /rong);

            vertices[count1 + 1] = new Vector3(tmp.z, yValue, tmp.y);
            uv[count1 + 1] = new Vector2(tmp.z / dai, tmp.y / rong);

            vertices[count1 + 2] = new Vector3(tmp.z, yValue, tmp.w);
            uv[count1 + 2] = new Vector2(tmp.z / dai, tmp.w / rong);

            vertices[count1 + 3] = new Vector3(tmp.x, yValue, tmp.w);
            uv[count1 + 3] = new Vector2(tmp.x / dai, tmp.w / rong);

            //Tạo triangles
            triangles[count2++] = count1;
            triangles[count2++] = count1 + 2;
            triangles[count2++] = count1 + 1;

            triangles[count2++] = count1;
            triangles[count2++] = count1 + 3;
            triangles[count2++] = count1 + 2;

            //Tăng biến đếm
            count1 += 4;
        }

        //Tạo mesh
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uv;

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        if (!GetComponent<MeshFilter>())
            gameObject.AddComponent<MeshFilter>();
        if (!GetComponent<MeshRenderer>())
            gameObject.AddComponent<MeshRenderer>();
        if (!GetComponent<MeshCollider>())
            gameObject.AddComponent<MeshCollider>();

        gameObject.GetComponent<MeshFilter>().mesh = mesh;

        MeshCollider meshc = gameObject.GetComponent<MeshCollider>();
        meshc.sharedMesh = mesh;
        //meshc.tag = "nen";

        print("=== Tạo xong bờ bao ====");
    }

    private void buildCubeNen(List<Vector4> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            Vector4 tmp = list[i] * DuLieu.getInstance().getDistance();
            list[i] = tmp;

            float xGiua = (list[i].z + list[i].x) / 2;
            float zGiua = (list[i].w + list[i].y) / 2;
            Vector3 position = new Vector3(xGiua, 0, zGiua);

            float xLocal = (list[i].z - list[i].x) ;
            float yLocal = DuLieu.getInstance().getDoCaoBoBao() * 2;
            float zLocal = (list[i].w - list[i].y) ;
            Vector3 localScale = new Vector3(xLocal, yLocal, zLocal);

            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = position;
            cube.transform.localScale = localScale;

            if (!GetComponent<MeshFilter>())
                cube.AddComponent<MeshFilter>();
            if (!GetComponent<MeshRenderer>())
                cube.AddComponent<MeshRenderer>();
            if (!GetComponent<MeshCollider>())
                cube.AddComponent<MeshCollider>();
        }
    }

    private void buildTuong(List<Vector4> list)
    {
        float xMax = (maTranTmp.Length + 2 * DuLieu.getInstance().getChieuRongMeshBoAo()) * DuLieu.getInstance().getDistance();
        float yMax = (maTranTmp[0].Length + 2 * DuLieu.getInstance().getChieuRongMeshBoAo()) * DuLieu.getInstance().getDistance();

        GameObject[] cube = new GameObject[4];

        cube[0] = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube[0].transform.position = new Vector3(-0.5f, 15, yMax / 2);
        cube[0].transform.localScale = new Vector3(1, 30, yMax);

        cube[1] = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube[1].transform.position = new Vector3( xMax + 0.5f, 15, yMax / 2);
        cube[1].transform.localScale = new Vector3(1, 30, yMax);

        cube[2] = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube[2].transform.position = new Vector3( xMax / 2, 15, -0.5f);
        cube[2].transform.localScale = new Vector3(xMax, 30, 1);

        cube[3] = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube[3].transform.position = new Vector3(xMax / 2, 15, yMax + 0.5f);
        cube[3].transform.localScale = new Vector3(xMax, 30, 1);

        for (int i = 0; i < cube.Length; i++)
        {
            if (!GetComponent<MeshFilter>())
                cube[i].AddComponent<MeshFilter>();
            if (!GetComponent<MeshRenderer>())
                cube[i].AddComponent<MeshRenderer>();
            if (!GetComponent<MeshCollider>())
                cube[i].AddComponent<MeshCollider>();
        }
    }



    private int kiemTraMaxDai(int dai, int rong, int x1, int y1)
    {
        bool checkDai = false;
        do
        {
            //Tăng dài lên 1
            dai++;

            if (maTranTmp[x1 + dai][y1] == true)
            {
                checkDai = true;
            }

        } while (checkDai == false);

        return dai;
    }

    private int kiemTraMaxRong(int dai, int rong, int x1, int y1)
    {
        bool checkRong = false;
        do
        {
            //Tăng rộng lên 1
            rong++;

            for (int i = x1; i < x1 + dai; i++)
            {
                if (maTranTmp[i][y1 + rong] == true)
                {
                    checkRong = true;
                }
            }
        } while (checkRong == false);
        return rong;
    }

    private void danhDauDaDuyet(int x1, int y1, int dai, int rong)
    {
        int endX = x1 + dai;
        int endY = y1 + rong;
        for (int i = x1; i < endX; i++)
        {
            for (int j = y1; j < endY; j++)
            {
                maTranTmp[i][j] = true;
            }
        }
    }

    /// <summary>
    /// Duyệt trong mảng vị trí để lấy ra các khối bờ ao hình chữ nhật
    /// </summary>
    /// <param name="x1">hoành độ điểm giá trị false</param>
    /// <param name="y1">tung độ điểm giá trị false</param>
    /// <returns></returns>
    private Vector4 duyetKhoi(int x1, int y1)
    {
        int dai = 0, rong = 0; // Dài rộng ban đầu là 1, 1, bắt đầu từ (j-1,m-1). còn 0,0 là bắt đầu từ điểm (j,m)

        //Kiểm tra dài
        dai = kiemTraMaxDai(dai, rong, x1, y1);

        //Kiểm tra rộng
        rong = kiemTraMaxRong(dai, rong, x1, y1);

        //Đánh dấu khối này đã duyệt
        danhDauDaDuyet(x1, y1, dai, rong);
        
        //(x,y) tọa độ điểm đầu, (z,w) tọa độ điểm cuối
        Vector4 thongSo = new Vector4();
        thongSo.x = x1 - 1;
        thongSo.y = y1 - 1;
        thongSo.z = x1 + dai;
        thongSo.w = y1 + rong;

        return thongSo;
    }

    //------------------------------------
}
