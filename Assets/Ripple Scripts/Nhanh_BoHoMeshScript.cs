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
        print("=== tạo nền bờ bao =======");
        buildMeshBoBao(listKhoiNen);

        //Đặt vị trí
        gameObject.transform.position = new Vector3(0, DuLieu.getInstance().getDoCaoBoBao(), 0);
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
