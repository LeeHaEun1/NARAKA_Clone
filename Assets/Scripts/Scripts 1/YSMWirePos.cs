using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YSMWirePos : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        transform.right = Camera.main.transform.forward;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
