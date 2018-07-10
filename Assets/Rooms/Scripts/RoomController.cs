﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomController : MonoBehaviour {
	public static class Direction {
		public const string NORTH = "North/DoorNorth";
		public const string EAST = "East/DoorEast";
		public const string SOUTH = "South/DoorSouth";
		public const string WEST = "West/DoorWest";
	}

	private Dictionary<string, Transform> doors = new Dictionary<string, Transform>();

	void Awake () {
		doors.Add(Direction.NORTH, transform.Find(Direction.NORTH));
		doors.Add(Direction.EAST, transform.Find(Direction.EAST));
		doors.Add(Direction.SOUTH, transform.Find(Direction.SOUTH));
		doors.Add(Direction.WEST, transform.Find(Direction.WEST));

		SetAllDoors(true);
	}

	public void SetDoor (bool open, string direction) {
		Transform door = doors[direction];
		door.gameObject.SetActive(!open);
	}

	public void SetAllDoors(bool open) {
		SetDoor(open, Direction.NORTH);
		SetDoor(open, Direction.EAST);
		SetDoor(open, Direction.SOUTH);
		SetDoor(open, Direction.WEST);
	}
}
