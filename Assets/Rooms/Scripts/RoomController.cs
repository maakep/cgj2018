﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomController : MonoBehaviour {
	public static class Direction {
		public const string NORTH = "North";
		public const string EAST = "East";
		public const string SOUTH = "South";
		public const string WEST = "West";
	}

	private Dictionary<string, DoorController> doors = new Dictionary<string, DoorController>();
	private Light light;
	public float lightTriggerRadius = 15f;
	public float lightMaxRadius = 2f;
	public float maxLight = 30;
    // private float magicFociNumber = 3.47f;
	
	public GameObject[] monsters;
	public Vector2[] spawnPoints;

	private List<GameObject> myMonsters = new List<GameObject>(); 

	private bool isActive = false;
	private GameObject fog;

	private const bool SPAWNMONSTERS = true;

	void Awake () {
		doors.Add(Direction.NORTH, transform.Find(Direction.NORTH).GetComponent<DoorController>());
		doors.Add(Direction.EAST, transform.Find(Direction.EAST).GetComponent<DoorController>());
		doors.Add(Direction.SOUTH, transform.Find(Direction.SOUTH).GetComponent<DoorController>());
		doors.Add(Direction.WEST, transform.Find(Direction.WEST).GetComponent<DoorController>());

		light = transform.Find("Light").gameObject.GetComponent<Light>();

		fog = transform.Find("Fog").gameObject;
	}

	void Update() {
		if (Vector2.Distance(PlayerManager.PlayerObjects[0].transform.position, light.transform.position) < lightTriggerRadius) {
			light.gameObject.SetActive(true);
			float normdist = Mathf.Max(Vector2.Distance(PlayerManager.PlayerObjects[0].transform.position, light.transform.position) - lightMaxRadius, 0);
			float factor = 1 - normdist / (lightTriggerRadius - lightMaxRadius);
			light.range = maxLight * factor;
		} else {
			light.gameObject.SetActive(false);
		}

		if (SPAWNMONSTERS && !fog.activeInHierarchy && !isActive) {
			SpawnMonsters();
			isActive = true;
		}

		// Does room still have MORTAL ENEMYYYY?
		if (isActive && myMonsters.Where(x => x != null).Count() == 0) {
			Debug.Log("Congratulations, you win this room.");
			foreach (DoorController rc in doors.Values) {
				rc.UnlockDoor();
			}
		}

		// TODO: Fix for both players
	}

	// PERS ELLIPSE FOH-SIGH SHIZ

    // void Update()
    // {
    //     var foci = Mathf.Sqrt(Mathf.Pow(lightTriggerRadius + magicFociNumber, 2) + Mathf.Pow(lightTriggerRadius, 2));  // major axis^2 - minor axis^2 

    //     Vector2 leftFociPoint = new Vector2(light.transform.position.x - foci, light.transform.position.y);
    //     Vector2 rightFociPoint = new Vector2(light.transform.position.x + foci, light.transform.position.y);
    //     var totalDistanceFromFoci = Vector2.Distance(leftFociPoint, PlayerManager.PlayerObjects[0].transform.position) + Vector2.Distance(rightFociPoint, PlayerManager.PlayerObjects[0].transform.position);
    //     //if (totalDistanceFromFoci < 50)
    //     //Debug.Log("FociDistance = " + totalDistanceFromFoci + " lighttriggerradius = " + lightTriggerRadius);

    //     if (totalDistanceFromFoci < 51)
    //     {
    //         light.gameObject.SetActive(true);
    //         float normdist = Mathf.Max(Vector2.Distance(PlayerManager.PlayerObjects[0].transform.position, light.transform.position) - lightMaxRadius, 0);
    //         float factor = 1 - normdist / (lightTriggerRadius - lightMaxRadius);
    //         light.range = maxLight * factor;


    //         //if (Vector2.Distance(PlayerManager.PlayerObjects[0].transform.position, light.transform.position) < lightTriggerRadius)
    //         //{
    //         //light.gameObject.SetActive(true);
    //         //float normdist = Mathf.Max(Vector2.Distance(PlayerManager.PlayerObjects[0].transform.position, light.transform.position) - lightMaxRadius, 0);
    //         //float factor = 1 - normdist / (lightTriggerRadius - lightMaxRadius);
    //         //light.range = maxLight * factor;
    //     }
    //     else
    //     {
    //         light.gameObject.SetActive(false);
    //     }

    //     // TODO: Fix for both players
    // }

    //Keep if needed to update foci calculations
    //private void OnDrawGizmos()
    //{
    //    var foci = Mathf.Sqrt(Mathf.Pow(lightTriggerRadius + magicFociNumber, 2) + Mathf.Pow(lightTriggerRadius, 2));  // major axis^2 - minor axis^2 
        
    //    Gizmos.DrawWireSphere(new Vector3(light.transform.position.x - foci, light.transform.position.y, 0), 5.0f);
    //    Gizmos.DrawWireSphere(new Vector3(light.transform.position.x + foci, light.transform.position.y, 0), 5.0f);
    //}

	private void SpawnMonsters() {
		// Create a new spawnpoint from the first
		Vector2 myFirstSpawnPoint = spawnPoints[0];
		myFirstSpawnPoint.x -= 2;

		GameObject initMon = monsters[Random.Range(0, monsters.Length)];
		GameObject myMon = Instantiate(initMon, myFirstSpawnPoint, Quaternion.identity);
		myMonsters.Add(myMon);

		foreach (Vector2 spawnPoint in spawnPoints) {
			if (Random.Range(0f, 1f) < 0.8) {
				GameObject mon = monsters[Random.Range(0, monsters.Length)];
				GameObject obj = Instantiate(mon, spawnPoint, Quaternion.identity);
				myMonsters.Add(obj);
			}
		}
	}

	public void SetDoor (bool open, string direction) {
		DoorController door = doors[direction];
		door.LockDoor();
	}

	public void SetAllDoors(bool open) {
		SetDoor(open, Direction.NORTH);
		SetDoor(open, Direction.EAST);
		SetDoor(open, Direction.SOUTH);
		SetDoor(open, Direction.WEST);
	}
}