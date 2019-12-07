using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Axis
{
    public KeyCode _positive;
    public KeyCode _negative;

    public Axis(KeyCode positive, KeyCode negative)
    {
        _positive = positive;
        _negative = negative;
    }

    public float GetAxis()
    {
        float value = 0;
        if (Input.GetKey(_positive))
        {
            value += 1;
        }
        if (Input.GetKey(_negative))
        {
            value -= 1;
        }

        return value;
    }
}
