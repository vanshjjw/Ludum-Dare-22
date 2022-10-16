using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObjects : MonoBehaviour {
    public string category;  //zombie, person or soldier
    public float speed;      //speed for each
    public Vector3 position; //instantenous position
    public Vector3 positionOfCentre;  //only for soldier's central location
    public static Vector3[] directions = new Vector3[]{new Vector3(1f,0f,0f), new Vector3(0f,1f,0f), new Vector3(-1f,0f,0f), new Vector3(0f,-1f,0f)};
    public Vector3 cur_dir;
    public int choices;

    public float susceptibility = 1f;
    public static float soundRadius = 1;
    public static float sightRadius = 5;
    public bool isDead = false;
    //Multiplier Variables to be stored
    public System.DateTime last_attack;
    public ArrayList soldier_properties;
    public int type;

    public bool hasCrossedLine = false;

    public void initialise(string name, float s, ArrayList properties = null) {
       category = name;
       //SpriteRenderer sc = gameObject.AddComponent<SpriteRenderer>();
        SpriteRenderer sc = gameObject.GetComponent<SpriteRenderer>();
        GameObject GM = GameObject.Find("GameManager");
       if(name.Equals("zombie")){  
         type = 1;
         sc.sprite = GM.GetComponent<spriteList>().zombieDesign;
       }
       else if(name.Equals("person")){  
         type = 0;
         sc.sprite = GM.GetComponent<spriteList>().civilianDesign;
       }
       else   {                        
         type = 2;
         sc.sprite = GM.GetComponent<spriteList>().soldierDesign;
       }
       soldier_properties = properties;
       speed = s;
       position = transform.position;
       //need to add Mesh filters and looks  when we begin
    }

    void Start() {
        List<Vector3> sight_direction = Ava_direction(1.5f);
        this.choices = sight_direction.Count;
        int ran = Random.Range(0, this.choices);
        if(this.type != 2)
             this.cur_dir = sight_direction[ran];
        else                                              //ADDED NOW
             this.cur_dir = new Vector3(0,0,0);

        //Debug.Log(speed);
        transform.Translate(this.cur_dir*speed*Time.deltaTime);
        this.last_attack = System.DateTime.Now;
        //Debug.Log("start");
        
    }

    void Update() {
        // Debug.Log("update");
        position = transform.position;
        foreach(Vector3 direction in directions){
            Debug.DrawLine(this.transform.position + direction * 1.5f, this.transform.position, Color.green);
            Debug.DrawLine(this.transform.position, this.transform.position + direction * 0.5f, Color.black);
        }
        (List<Vector3> move_direction, List<Vector3> sight_direction) = (Ava_direction(0.05f), Ava_direction(0.5f)); 
    
        (List<Vector3> civilian, List<Vector3> zombie, List<Vector3> soldier) = NearbyObjects();
        if(this.type == 0) {
            CivilianMovement(zombie, soldier, move_direction, sight_direction);
        }
        else if (this.type == 1) {
            ZombieMovement(civilian, soldier, move_direction, sight_direction);
        }
        else if (this.type == 2){
            SoldierMovement(zombie, move_direction, sight_direction);
        }
        Death();

    } 

    void RandomMovement(List<Vector3> move_direction, List<Vector3> sight_direction){
        //Debug.Log(this.choices + ", " + sight_direction.Count);
        if(this.choices >= sight_direction.Count && move_direction.Contains(this.cur_dir)){
            transform.Translate(this.cur_dir*speed*Time.deltaTime);
            this.choices = sight_direction.Count;
        }
        else{
            //Debug.Log("elsed");
            this.choices = sight_direction.Count;
            Vector3 neg = -1f * this.cur_dir;

            int index = sight_direction.IndexOf(neg);

            if(index > -1){
                if(this.choices > 1){
                    sight_direction.RemoveAt(index);
                }
            }

            int ran = Random.Range(0, sight_direction.Count);
            this.cur_dir = sight_direction[0];
            transform.Translate(this.cur_dir*speed*Time.deltaTime);
        }     
    }   

    Vector3 ClosestTarget(List<Vector3> targets){
        Vector3 closest_target = new Vector3();
        float closest_distance = 10000f;
        foreach(Vector3 target in targets){
            float distance = (transform.position - target).magnitude;
            if(distance < closest_distance){
                closest_distance = distance;
                closest_target = target;
            }
        }
        return closest_target;
    }

    Vector3 CenterOfMass(List<Vector3> threats){
        Vector3 center_of_mass = new Vector3(0, 0, 0);
        foreach(Vector3 threat in threats){
            Vector3 v = threat - this.transform.position;
            float m = 1/v.magnitude;
            center_of_mass += Vector3.Scale(new Vector3(m,m,m), v.normalized);
        }
        return center_of_mass;
    }

    void FleeOrChase(Vector3 center_of_mass, float dir, List<Vector3> move_direction){
        Vector3 best_dir = Vector3.zero;
        float best_distance = (0.001f * best_dir + this.transform.position - center_of_mass).magnitude;
        foreach(Vector3 direction in move_direction){
            float distance = (0.001f * direction + this.transform.position - center_of_mass).magnitude;
            if(distance > best_distance){
                best_dir = direction;
                best_distance = distance;
            }
        }
        best_dir = Vector3.Scale(best_dir, new Vector3(dir, dir, dir));
        this.cur_dir = best_dir;
        transform.Translate(this.cur_dir*speed*Time.deltaTime);
    }

    void CivilianMovement(List<Vector3> zombie, List<Vector3> soldier, List<Vector3> move_direction, List<Vector3> sight_direction){
        if(zombie.Count == 0) {
            RandomMovement(move_direction, sight_direction);
        }
        else {
            FleeOrChase(CenterOfMass(zombie), 1f, move_direction);
        }
    }
    void ZombieMovement(List<Vector3> civilian, List<Vector3> soldier, List<Vector3> move_direction, List<Vector3> sight_direction){

        civilian.AddRange(soldier);
        List<Vector3> targets = civilian;

        if(targets.Count == 0) {
            RandomMovement(move_direction, sight_direction);
        }
        else {
            Vector3 closest_target = ClosestTarget(targets);
            FleeOrChase(closest_target, -1f, move_direction);
        }
    }

    void SoldierMovement(List<Vector3> zombies, List<Vector3> move_direction, List<Vector3> sight_direction){
        List<Vector3> all_zombie = zombies;
        List<Vector3> target_zombie = new List<Vector3>();
        foreach(Vector3 zombie in all_zombie){
            float distance = (zombie - this.position).magnitude;
            bool temp = Physics.Linecast(this.transform.position, zombie);
            if(distance < (float)this.soldier_properties[1] && temp){
                target_zombie.Add(zombie);
            }
        }
        int count = all_zombie.Count;
        if(count == 0) {
            //Debug.Log("No zombie");
        }
        else{
            Vector3 center_of_mass = CenterOfMass(all_zombie);
            float factor = count/((center_of_mass - this.transform.position).magnitude);
            factor = (2*Mathf.Atan(factor)/Mathf.PI);  //Mapping to a number between 0 and 1;
            if(factor > (float) this.soldier_properties[3]){
                Debug.Log("Bravary overwhelmed as factor was: " + factor + "so running away from a position: " + center_of_mass);
                FleeOrChase(center_of_mass, 1f, move_direction);
                //Debug.Log("Running");
            }
            else{
                if((System.DateTime.Now - this.last_attack).Seconds > 1/((float)soldier_properties[5])){
                    //Debug.Log("Fighting");
                    SoldierAttack(target_zombie);
                    this.last_attack = System.DateTime.Now;
                }
                else{
                    //FleeOrChase(center_of_mass, 1f, move_direction);
                }
            }
        }
    }

   public void Death() {
        Vector3[] positions = MasterControl.positions;
        int[] Types = MasterControl.Types;
        for(int i=0; i<Types.Length; i++) {
            if(Vector3.Distance(this.transform.position, positions[i]) < 0.1f) {
                if(this.type != 1 && Types[i] == 1) {
                    if(Random.Range(0,1) < susceptibility) {
                        initialise("zombie", MasterControl.ZombieSpeed);
                        return;
                    }
                }   
            }
        }
        if(this.type == 1 && isDead) {
           StartCoroutine(PlaceHolder());            
        }

        IEnumerator PlaceHolder() {
            SpriteRenderer sc = gameObject.GetComponent<SpriteRenderer>();
            GameObject GM = GameObject.Find("GameManager");
            this.speed = 0;
            for(int i=0; i<11; i++) {
                sc.sprite = GM.GetComponent<spriteList>().deadpics[i];
                yield return new WaitForSeconds(0.2f);
            }
            Destroy(gameObject);
        }

    }

    public void SoldierAttack(List<Vector3> zombposes) {
        Vector3[] positions = MasterControl.positions;
        int[] Types = MasterControl.Types;
        //filter non-close zombies
        //find closest guy
        float minDis = 1000f;
        int index = -1;
        for (int i = 0; i < positions.Length; i++) {
            if(Vector3.Distance(positions[i],this.position) < minDis && Types[i]==1) {   //zombie and closer than previous zombie 
                minDis = Vector3.Distance(positions[i],this.position);
                index = i;   
            }
        } 
        if(Random.Range(0,1) < (float)this.soldier_properties[2]) {
              if(Random.Range(0,1) < 0.5) {
                  AreaDamage(index);
              }
              MasterControl.references[index].isDead = true;
              //Debug.Log("killed by officer of accuracy: " + soldier_properties[2] + "  at index:  " + index);          
        }
    }
    

    List<Vector3> Ava_direction(float distance){
        List<Vector3> ava_direction = new List<Vector3>();
        foreach (Vector3 direction in directions) {
            if(Physics.Linecast(this.transform.position, this.transform.position + direction*distance)){
                //Debug.Log("Hit" + direction);
            }
            else{
                //Debug.Log("Available" + direction);
                ava_direction.Add(direction);
            }
        }
        return ava_direction;
    }

    public (List<Vector3>, List<int>) Detection(float radius) {
          List<Vector3> locations = new List<Vector3>();
          List<int> type = new List<int>();
          for (int i = 0; i < MasterControl.positions.Length; i++) {
              if(Vector3.Distance(this.position, MasterControl.positions[i]) < radius) {
                  locations.Add(MasterControl.positions[i]);
                  type.Add(MasterControl.Types[i]);
              }
          }
          return(locations, type);
    }

    public (List<Vector3>, List<Vector3>, List<Vector3>) NearbyObjects(){

        (List<Vector3> soundDetected_coordinates, List<int> soundDetected_type) = Detection(1);
        (List<Vector3> sightDetected_coordinates, List<int> sightDetected_type) = Detection(5);

        List<Vector3> soldier = new List<Vector3>();

        List<Vector3> civilian = new List<Vector3>();

        List<Vector3> zombie = new List<Vector3>();

        //0 is civilian
        //1 is zombie
        //2 is soldier

        for(int i = 0; i < soundDetected_type.Count; i++){
            if(soundDetected_type[i] == 0){
                civilian.Add(soundDetected_coordinates[i]);
            }
            else if(soundDetected_type[i] == 1){
                zombie.Add(soundDetected_coordinates[i]);
            }
            else if(soundDetected_type[i] == 2){
                soldier.Add(soundDetected_coordinates[i]);
            }
            else{
                Debug.Log("unknown type");
            }
        }

        for(int i = 0; i < sightDetected_type.Count; i++){

            if(Physics.Linecast(this.transform.position, sightDetected_coordinates[i])){
                continue;
            }
            else{
                if(sightDetected_type[i] == 0){
                    civilian.Add(sightDetected_coordinates[i]);
                }
                else if(sightDetected_type[i] == 1){
                    zombie.Add(sightDetected_coordinates[i]);
                }
                else if(sightDetected_type[i] == 2){
                    soldier.Add(sightDetected_coordinates[i]);
                }
            }
        }
        return (civilian, zombie, soldier);
    }
}
