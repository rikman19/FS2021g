﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StayY : MonoBehaviour
{
	public float FixedY;
	
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(transform.position.x, FixedY, transform.position.z);
    }
}
