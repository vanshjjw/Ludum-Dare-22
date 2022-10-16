using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Should be in the background folder which has all the spawn objects
public class Properties : MonoBehaviour {
    public static Transform[] spawnpoints;
    void Start() {
    }
    void Awake() {
        spawnpoints = new Transform[transform.childCount];
        for (int i = 0; i < spawnpoints.Length; i++) {
            spawnpoints[i] = transform.GetChild(i);
            //Debug.Log("Properties Works");
        }
    }
}
