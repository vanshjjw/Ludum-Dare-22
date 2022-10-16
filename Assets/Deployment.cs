using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deployment : MonoBehaviour
{
    // Start is called before the first frame update
    bool isDraggable;
    bool isDragging;
    Collider2D objectCollider;
    public GameObject MovablePrefab;
    //public int typeOfSoldier = 0;
    public float speed = 0;
    public float range = 0;
    public float accuracy = 0;
    public float bravery = 0;
    public float number = 0;
    public float attackspeed = 0;

    public ArrayList properties = new ArrayList();
    void Start()
    {
        objectCollider = GetComponent<Collider2D>();
        isDraggable = false;
        isDragging = false;
    }
    void Update(){
         DragAndDrop();
    }

    public void DragAndDrop(){
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (Input.GetMouseButtonDown(0)) {
            if(objectCollider == Physics2D.OverlapPoint(mousePosition)){
                isDraggable = true;
            }
            else { 
                isDraggable = false;
            }

            if (isDraggable)
                 isDragging = true;
            
        }
        if (isDragging) {
            this.transform.position = mousePosition;
        }

        if (Input.GetMouseButtonUp(0)) {
            isDraggable = false;    //This is not happening when i release the mouse pointer in quick successsion after dragging the object
            isDragging = false;     //If i drag to the position, wait for half a second, and the release mouse, the problem is not there
        }
    }

    public void SpawnSoliders() {
        List<Vector3> displacement = new List<Vector3>();
        ArrayList properties = new ArrayList(){speed, range, accuracy, bravery, number, attackspeed};
        for (int i = 0; i < number; i++){
            Vector3 dis = new Vector3(Mathf.Cos(i*2f*Mathf.PI/number), Mathf.Sin(i*2f*Mathf.PI/number), 0);
            displacement.Add(dis);
        } 
        for (int i = 0; i < number; i++) {
            GameObject a = Instantiate(MovablePrefab, this.transform.position + 0.1f * displacement[i], new Quaternion(0,0,0,1));
            a.transform.parent = GameObject.Find("GameManager").transform;
            MovingObjects m = a.AddComponent<MovingObjects>();
            m.initialise("soldier", (float)properties[0], properties); 
        }
    }
}
