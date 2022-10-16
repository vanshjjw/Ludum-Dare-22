using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterControl : MonoBehaviour {
    // class variables 
    // git help file
    public static int[] Types;
    public static Vector3[] positions;
    public static MovingObjects[] references;
    public GameObject MovablePrefab;

    //Important Global Variables 
    public static float ZombieSpeed = 0.8f;
    public float ZombieSpeedPublic;
    public float PersonSpeedPublic = 0.4f;
    public float z;
    public float p;
    public int numberOfZombiesCrossed = 0;
    public int numberOfPeopleDied = 0;


    
    public void findVariables() {
        MovingObjects[] random = gameObject.GetComponentsInChildren<MovingObjects>();
        MovingObjects[] mobs = random;
        int c, j;
        for (c=0, j = 0; j < random.Length; j++) {
            if(random[j] != null) 
                mobs[c++] = random[j];
        }
        Types = new int[j];
        positions = new Vector3[j];

        for (int i = 0; i < j; i++) {
            if(mobs[i] != null) {
            Types[i] = mobs[i].type;
            positions[i] = mobs[i].position;
            }
            references = mobs;
        }
    }
    public void checkForZombieCrossings() {
        for (int i = 0; i < references.Length; i++) {
            if(references[i].type == 1 && references[i].transform.position.x > 15)
                  references[i].hasCrossedLine = true;
        }
    }
    public void countNumberofZombiesCrossed() {
        int c = 0;
        int numpeeps = 0;
        for (int i = 0; i < references.Length; i++) {
            if(references[i].hasCrossedLine)
                 c++;
            if(references[i].type == 0) {
                numpeeps++;
            }
        }
        numberOfZombiesCrossed = c;
        numberOfPeopleDied = (int)(z - numpeeps);
    }

    void Start() {
        findVariables();  
        MasterControl.ZombieSpeed = this.ZombieSpeedPublic;
    }

    void Update() {
        findVariables();
        //checkForZombieCrossings();
        //countNumberofZombiesCrossed();
    }

    public void spawnZombiesAndPeople() {
    int len = Properties.spawnpoints.Length; 
    StartCoroutine(Generate(z, 0.4f, "zombie", ZombieSpeedPublic, len-3, len));
    StartCoroutine(Generate(p, 0.1f, "person", PersonSpeedPublic, 0, len-3));
        //Spawning people      
    }  

    IEnumerator Generate(float num, float seconds, string name, float s, int begin, int end) {
        for (int i = 0; i < num; i++) {
                Vector3 random = new Vector3(Random.Range(0,1),Random.Range(0,1),0);
                random = 1f*random.normalized;
                random = Properties.spawnpoints[Random.Range(begin, end)].position + random;
                //bool checkForColliders = check(random);
                GameObject a = Instantiate(MovablePrefab, random, new Quaternion(0,0,0,1));
                a.transform.parent = GameObject.Find("GameManager").transform;
                MovingObjects m = a.AddComponent<MovingObjects>();
                // BoxCollider bc = a.AddComponent<BoxCollider>();
                // bc.size = new Vector3(0.1f, 0.1f, 0.1f);
                m.initialise(name, s); 
                yield return new WaitForSeconds(seconds);
        }
    }
}    



    
    
    
    
    
    
    
    
    
   

