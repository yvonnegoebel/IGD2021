﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHit : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void OnCollisionEnter(Collision other)
    {
        Debug.Log("Treffer");
        if(other.gameObject.tag == "Vehicle")
        {
            Destroy(gameObject);
        }
        
    }
}
