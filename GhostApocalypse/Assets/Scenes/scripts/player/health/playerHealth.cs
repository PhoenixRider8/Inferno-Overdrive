using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerHealth : MonoBehaviour
{
    public float health = 25;
    public void takeDamage()
    {
        health -= 1;
    }
}
