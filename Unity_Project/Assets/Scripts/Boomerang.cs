using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boomerang : MonoBehaviour
{
    private bool initialized = false;
    private Transform hex;

    private NeuralNetwork net;
    private Rigidbody2D rBody;
    private Material[] mats;

    // Initially called before any update methods when the script is enabled, only preceded by the Awake() function.
    void Start()
    {
        // Returns the possibly attached component (to the game-object) or null.
        rBody = GetComponent<Rigidbody2D>();
        //GUI.Label(new Rect(Screen.width / 2, Screen.height / 2, 200f, 200f), "TEST");
        mats = new Material[transform.childCount];
        for (int i = 0; i < mats.Length; i++)
            mats[i] = transform.GetChild(i).GetComponent<Renderer>().material;
    }

    void FixedUpdate()
    {
        if (initialized == true)
        {
            float distance = Vector2.Distance(transform.position, hex.position);
            if (distance > 20f)
                distance = 20f;
            for (int i = 0; i < mats.Length; i++)
                mats[i].color = new Color(distance / 20f, (1f - (distance / 20f)), (1f - (distance / 20f)));

            float[] inputs = new float[1];


            float angle = transform.eulerAngles.z % 360f;
            if (angle < 0f)
                angle += 360f;

            Vector2 deltaVector = (hex.position - transform.position).normalized;


            float rad = Mathf.Atan2(deltaVector.y, deltaVector.x);
            rad *= Mathf.Rad2Deg;

            rad = rad % 360;
            if (rad < 0)
            {
                rad = 360 + rad;
            }

            rad = 90f - rad;
            if (rad < 0f)
            {
                rad += 360f;
            }
            rad = 360 - rad;
            rad -= angle;
            if (rad < 0)
                rad = 360 + rad;
            if (rad >= 180f)
            {
                rad = 360 - rad;
                rad *= -1f;
            }
            rad *= Mathf.Deg2Rad;

            inputs[0] = rad / (Mathf.PI);


            float[] output = net.FeedForward(inputs);

            rBody.velocity = 2.5f * transform.up;
            rBody.angularVelocity = 500f * output[0];

            net.AddFitness((1f - Mathf.Abs(inputs[0])));
        }
    }

    public void Init(NeuralNetwork net, Transform hex)
    {
        this.hex = hex;
        this.net = net;
        initialized = true;
    }


}