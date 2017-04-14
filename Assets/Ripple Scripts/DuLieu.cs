using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuLieu : MonoBehaviour{

    private static DuLieu s_singleton;

    private ScreenThongSo[] screen;

    /// <summary>
    /// Ma trận mặt nước 2 chiều và 1 chiều
    /// </summary>
    static public bool[][] maTranBool;

    //Các thông số động
    /// <summary>
    /// 1/ khoangCach là Hệ số đỉnh trên mesh, tỷ lệ thuận với số đỉnh. Khoảng cách cần nên là số hữu hạn 1 chữ số thập phân
    /// </summary>
    private float khoangCach = 0.3f;
    /// <summary>
    /// Khoảng cách mỗi đơn vị ao, tỷ lệ tuận với kích thước ao
    /// </summary>
    private float distance = 0.5f;

    private DuLieu()
    {
        screen = new ScreenThongSo[3];

        screen[0] = new ScreenThongSo(new Vector2(0, 0), 20, 10);
        screen[1] = new ScreenThongSo(new Vector2(25, 12.5f), 40, 15);
        screen[2] = new ScreenThongSo(new Vector2(10, -12.5f), 30, 15);

        //screen[0] = new ScreenThongSo(new Vector2(25, 10), 50, 20);
        //screen[1] = new ScreenThongSo(new Vector2( 0, -15), 40, 30);

        //man4 = new ScreenThongSo(new Vector2(0, 0), 3, 2);
        //man5 = new ScreenThongSo(new Vector2(0, 0), 3, 2);
        //man6 = new ScreenThongSo(new Vector2(0, 0S), 3, 2);

    }

    static public DuLieu getInstance()
    {
        if (s_singleton == null)
        {
            s_singleton = new DuLieu();
        }
        return s_singleton;
    }

    //Tạo mặt hồ và tạo bờ hồ
    public bool taoMaTranMatHo()
    {
        DuLieu dulieu = DuLieu.getInstance();

        //hienThiConsoleThongSoManHinh(dulieu);
        
        //Lấy tọa độ (xMin, yMin) và (xMax, yMax)
        float xMIN = 999999, yMIN = 999999;
        float xMAX = -999999, yMAX = -999999;
        for (int i = 0; i < dulieu.getScreen().Length; i++)
        {
            ScreenThongSo screen = dulieu.getScreen()[i];

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
        for (int i = 0; i < dulieu.getScreen().Length; i++)
        {
            ScreenThongSo screen = dulieu.getScreen()[i];

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
    
    public float getDistance()
    {
        return distance;
    }

    public ScreenThongSo[] getScreen()
    {
        return screen;
    }


    //--------------- Hiển thị Console -----------------------

    //private void hienThiConsoleThongSoManHinh(DuLieu dulieu)
    //{
    //    for (int i = 0; i < dulieu.screen.Length; i++)
    //    {
    //        print("(" + dulieu.screen[i].centerPoint.x + ", " + dulieu.screen[i].centerPoint.y + ")"
    //            + ", " + dulieu.screen[i].width
    //            + ", " + dulieu.screen[i].height);
    //    }
    //}

    //private void hienThiConsoleXYMinMax(float xMin, float xMax, float yMin, float yMax)
    //{
    //    print("xMin: " + xMin); print("xMax: " + xMax); print("yMin: " + yMin); print("yMax: " + yMax);
    //}

    //private void hienThiConsoleNumRowNumCol(int numRow, int numCol)
    //{
    //    print("numRow: " + numRow);
    //    print("numCol: " + numCol);
    //}

    //private void hienThiConsoleMaTranBool(bool[][] maTranBool)
    //{
    //    //Đảo ma trận
    //    bool[][] maTran = new bool[maTranBool[0].Length][];
    //    for (int i = 0; i < maTran.Length; i++)
    //    {
    //        maTran[i] = new bool[maTranBool.Length];
    //        for (int j = 0; j < maTran[i].Length; j++)
    //        {
    //            maTran[i][j] = maTranBool[j][maTranBool[0].Length - 1 - i];
    //        }
    //    }

    //    for (int i = 0; i < maTran.Length; i++)
    //    {
    //        string str = "";
    //        for (int j = 0; j < maTran[i].Length; j++)
    //        {
    //            if (maTran[i][j])
    //            {
    //                str += "1    ";
    //            }
    //            else
    //            {
    //                str += "0    ";
    //            }
    //        }
    //        print(str);
    //    }
    //}

}

public class ScreenThongSo
{
    public Vector2 centerPoint;
    public float width;
    public float height;

    public float xMin;
    public float xMax;
    public float yMin;
    public float yMax;

    public ScreenThongSo() { }

    public ScreenThongSo(Vector2 centerPoint, float width, float height)
    {
        this.centerPoint = centerPoint;
        this.width = width;
        this.height = height;

        xMin = centerPoint.x - width / 2;
        xMax = centerPoint.x + width / 2;

        yMin = centerPoint.y - height / 2;
        yMax = centerPoint.y + height / 2;
    }
    
}
