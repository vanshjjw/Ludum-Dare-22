using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Play_Button : MonoBehaviour
{
    // Start is called before the first frame update
    public static bool started;
    void Start()
    {
        //transform.position = new Vector3(-10,-3,0);
        started = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (started){
            //display timer?
            
        }
    }

    void OnMouseUp(){
        started = true;
    }

}
