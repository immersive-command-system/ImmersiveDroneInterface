var drone : GameObject;
 
function Start() {
    // original code
    //var pos : Vector3(0,0,0);
    //var rot : Quaternion = Quaternion.identity;

    var pos = new Vector3(0,0,0);
    var rot = Quaternion.identity;
    Instantiate(drone, pos, rot);
}
 
function Update() {
}

function AddObject() {
    Instantiate(drone, Vector3.zero, Quaternion.identity);
}