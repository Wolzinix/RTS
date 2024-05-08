using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MooveWhenClickScript : MonoBehaviour
{
    private Rigidbody _rb;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    { 
        _rb.velocity = new Vector3(0,0, 0);
    }
}
