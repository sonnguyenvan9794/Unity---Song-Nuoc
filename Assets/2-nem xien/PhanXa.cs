using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhanXa : MonoBehaviour {

    Rigidbody myRigidbody;
    Vector3 oldVel;
    int count;
    bool isRunning;

    void Start()
    {
        myRigidbody = GetComponent<Rigidbody>();
        myRigidbody.velocity = new Vector3(80, 10, 0);
        isRunning = true;

        count = 0;
    }

    void FixedUpdate()
    {
        
        oldVel = myRigidbody.velocity;
    }
     
    void OnCollisionEnter(Collision c)
    {
        //In ra các thông tin va chạm
        print("vận tốc góc: " + myRigidbody.angularVelocity + ", vận tốc: " + myRigidbody.velocity);


        //Lấy điểm va chạm đầu tiên
        ContactPoint cp = c.contacts[0];

        //--- Thông tin va chạm -----
        //print("Points colliding: " + c.contacts.Length);
        //print("First normal of the point that collide: " + c.contacts[0].normal);

        // calculate with addition of normal vector
        Vector3 velocity = oldVel * 3 / 4;

        

        //myRigidbody.velocity = oldVel + cp.normal * 1.5f* oldVel.magnitude;
        // calculate with Vector3.Reflect
        myRigidbody.velocity = Vector3.Reflect(velocity, cp.normal);

        // bumper effect to speed up ball
        //myRigidbody.velocity += cp.normal * 2.0f;


    }

    void OnCollisionStay(Collision c)
    {
        if (isRunning)
        {
            //print("count = " + ++count);
            if (oldVel.magnitude < 0.1f)
            {
                print("ket thuc");
                myRigidbody.velocity = new Vector3(0, 0, 0);
                myRigidbody.Sleep() ;
                isRunning = false;
            }
        }
    }
    
}
