using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuLieu {

    public static DuLieu s_singleton;

    public ScreenThongSo[] screen;

    public DuLieu()
    {
        s_singleton = this;

        screen = new ScreenThongSo[3];

        screen[0] = new ScreenThongSo(new Vector2(0, 0), 20, 10);
        screen[1] = new ScreenThongSo(new Vector2(25, 12.5f), 40, 15);
        screen[2] = new ScreenThongSo(new Vector2(10, -12.5f), 30, 15);
        //man4 = new ScreenThongSo(new Vector2(0, 0), 3, 2);
        //man5 = new ScreenThongSo(new Vector2(0, 0), 3, 2);
        //man6 = new ScreenThongSo(new Vector2(0, 0), 3, 2);
    }
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
