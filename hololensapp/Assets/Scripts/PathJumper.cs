using UnityEngine;
using System.Collections;

public class PathJumper : MonoBehaviour
{

    LineRenderer lr;
    int line_res;

    float velocity;
    float angle;
    float g;
    float dt;

    float Vy_old;
    float Vy;
    float Vx;

    Transform my_cube;

    void Start()
    {
        g = 9.81f;
        dt = 0.1f;
        velocity = 9;
        angle = 13;

        my_cube = this.transform;
        line_res = 100;
        lr = gameObject.GetComponent<LineRenderer>();
        lr.SetWidth(0.2f, 0.2f);
        lr.SetVertexCount(line_res);
        lr.SetColors(Color.green, Color.green);

    }


    void Update()
    {
        Vy_old = velocity * Mathf.Cos(angle * Mathf.Deg2Rad);
        Vx = velocity * Mathf.Sin(angle * Mathf.Deg2Rad); //checked

        for (int i = 0; i < line_res; i++)
        {

            Vy = Vy_old - g * dt;
            Vy_old = Vy;
            lr.SetPosition(i, new Vector2(Vx * i * dt, Vy * i * dt));
        }
    }


    void OnGUI()
    {

        velocity = GUI.HorizontalSlider(new Rect(0, 20, 200, 30), velocity, 0, 500);
        GUI.Label(new Rect(0, 40, 150, 30), velocity.ToString());

        angle = GUI.HorizontalSlider(new Rect(0, 60, 200, 30), angle, 0, 90);
        GUI.Label(new Rect(0, 80, 150, 30), angle.ToString());

        GUI.Label(new Rect(0, 100, 150, 30), "Vx:   " + Vx.ToString());
        GUI.Label(new Rect(0, 120, 150, 30), "Vy_old:   " + Vy.ToString());
    }
}