using UnityEngine.UI;

namespace ISAACS
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using ISAACS;
	
	public class SelectedDroneMenu : MonoBehaviour {

		public GameObject Individual_DroneUI_Prefab;
		public GameObject Group_DroneUI_Prefab;	

		private Dictionary<string, GameObject> Individual_Drones;
		private Dictionary<string, GameObject> Group_Drones;
		private Vector3 nextPostion_Indi;
		private Vector3 nextPostion_Group;
		// Use this for initialization
		void Start () {
			Individual_Drones  = new Dictionary<string, GameObject> ();
			Group_Drones = new Dictionary<string, GameObject> ();
			nextPostion_Indi = Individual_DroneUI_Prefab.transform.position;
			nextPostion_Group = Group_DroneUI_Prefab.transform.position;
		}

		public void UpdateMenu_Select_Drone(string droneID){
			//GameObject Individual_DroneUI = Instantiate (Individual_DroneUI_Prefab, transform);
			//Individual_DroneUI.transform.parent = Individual_DroneUI_Prefab.transform.parent;
			//Individual_DroneUI.GetComponent<Text> ().text = droneID;
			//Individual_DroneUI.transform.position = nextPostion_Indi;
			//Individual_Drones.Add(droneID, Individual_DroneUI);	
			//nextPostion_Indi -= new Vector3 (0, 0, -1.0f);
			Individual_DroneUI_Prefab.GetComponent<Text> ().text += " " + droneID;
		}

		public void UpdateMenu_Select_Group(string groupID){
			/*
			GameObject Group_DroneUI = Instantiate (Group_DroneUI_Prefab);
			Group_DroneUI.GetComponent<Text> ().text = groupID;
			Group_DroneUI.transform.position = nextPostion_Group;
			Group_Drones.Add(groupID, Group_DroneUI);	
			nextPostion_Group -= new Vector3 (0, 0, -1.0f);
			*/
			Group_DroneUI_Prefab.GetComponent<Text> ().text += " " +groupID;
		}

		public void UpdateMenu_Deselect_Drone(string droneID){
			//Destroy(Individual_Drones[droneID]); 
			//Individual_Drones.Remove (droneID);
		}

		public void UpdateMenu_Deselect_Group(string groupID){
			//Destroy(Group_Drones[groupID]); 
			//Group_Drones.Remove (groupID);
		}

	}

}