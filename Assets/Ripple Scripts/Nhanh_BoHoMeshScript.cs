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
        taoCacKhoiNen(listKhoiNen);
    }
	
	// Update is called once per frame
	void Update () {

    }

    //================== tạo các khổi nền =====================

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

        //Thêm các khối nền biên
        int themMeshCanh = DuLieu.getInstance().getChieuRongMeshBoAo();
        int dai = maTranTmp.Length;
        int rong = maTranTmp[0].Length;
        Vector4 tmp1 = new Vector4(0, 0, themMeshCanh, rong + themMeshCanh * 2);
        Vector4 tmp2 = new Vector4(themMeshCanh, 0, themMeshCanh + dai, themMeshCanh);
        Vector4 tmp3 = new Vector4(themMeshCanh, themMeshCanh + rong, themMeshCanh + dai, themMeshCanh * 2 + rong);
        Vector4 tmp4 = new Vector4(themMeshCanh + dai, 0, themMeshCanh * 2 + dai, themMeshCanh * 2 + rong);

        listKhoiNen.Add(tmp1);
        listKhoiNen.Add(tmp2);
        listKhoiNen.Add(tmp3);
        listKhoiNen.Add(tmp4);

        //Thêm các khối đường bao

        //Thử in ra các khối
        foreach (Vector4 item in listKhoiNen)
        {
            print("x = " + item.x + ", y = " + item.y + ", z = " + item.z + ", w = " + item.w);
        }

        return listKhoiNen;
    }

    /// <summary>
    /// Tạo Nền cho bờ ao
    /// </summary>
    /// <param name="listKhoiNen">Vector4 gồm tọa độ đầu (x,y) và tọa độ sau (z,w)</param>
    private void taoCacKhoiNen(List<Vector4> listKhoiNen)
    {

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
                dai--;
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

            for (int i = x1; i <= x1 + dai; i++)
            {
                if (maTranTmp[i][y1 + rong] == true)
                {
                    checkRong = true;
                    rong--;
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
        thongSo.z = x1 + dai + 1;
        thongSo.w = y1 + rong + 1;

        return thongSo;
    }

    //------------------------------------
}
