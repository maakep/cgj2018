﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomController : MonoBehaviour
{
    public static class Direction
    {
        public const string NORTH = "North";
        public const string EAST = "East";
        public const string SOUTH = "South";
        public const string WEST = "West";
    }

    private enum State
    {
        Active,
        Inactive,
        Finished
    }
    private State state = State.Inactive;

    private Dictionary<string, DoorController> doors = new Dictionary<string, DoorController>();
    private Light roomLight;
    private LightController lightController;
    public float lightTriggerRadius = 25f;
    public float lightMaxRadius = 2f;
    public float maxLight = 30;
    // private float magicFociNumber = 3.47f;
    public Color startColor, endColor;

    public GameObject[] monsters;
    public Vector2[] spawnPoints;
    public float minSpawnFactor = 0.4f;
    public float maxSpawnFactor = 0.9f;
    private float level;

    private List<GameObject> myMonsters = new List<GameObject>();

    private GameObject fog;

    private bool shouldSpawnMonsters = true;

    void Awake()
    {
        doors.Add(Direction.EAST, transform.Find(Direction.EAST).GetComponent<DoorController>());
        doors.Add(Direction.SOUTH, transform.Find(Direction.SOUTH).GetComponent<DoorController>());

        roomLight = transform.Find("Lights/Light").gameObject.GetComponent<Light>();
        lightController = GetComponentInChildren<LightController>();
        fog = transform.Find("Fog").gameObject;

        SetWestWallActive(false);
        SetNorthWallActive(false);
        SetSouthWallActive(false);
        SetEastWallActive(false);
    }

    void Update()
    {
        if (PlayerManager.PlayerObjects.Count == 0)
        {
            return;
        }

        bool playerInRange =
        Vector2.Distance(PlayerManager.PlayerObjects[0].transform.position, roomLight.transform.position) < lightTriggerRadius ||
            (PlayerManager.PlayerObjects.Count == 2 &&
            Vector2.Distance(PlayerManager.PlayerObjects[1].transform.position, roomLight.transform.position) < lightTriggerRadius);
        if (playerInRange)
        {
            // light.gameObject.SetActive(true);
            lightController.SetActive(true);
            float minPlayerDist = Vector2.Distance(PlayerManager.PlayerObjects[0].transform.position, roomLight.transform.position) - lightMaxRadius;
            if (PlayerManager.PlayerObjects.Count == 2)
            {
                minPlayerDist = Mathf.Min(minPlayerDist, Vector2.Distance(PlayerManager.PlayerObjects[1].transform.position, roomLight.transform.position) - lightMaxRadius);
            }
            float normdist = Mathf.Max(minPlayerDist, 0);
            float factor = 1 - normdist / (lightTriggerRadius - lightMaxRadius);
            lightController.SetRange(maxLight * factor);
        }
        else
        {
            lightController.SetActive(false);
        }

        if (!fog.activeInHierarchy && state == State.Inactive)
        {
            if (shouldSpawnMonsters)
            {
                SpawnMonsters();
                state = State.Active;
            }
        }

    }

    private int _monstersKilled;
    void OnMonsterDeath(Unit unit)
    {
        _monstersKilled++;
        if (state == State.Active && _monstersKilled >= myMonsters.Count)
        {
            state = State.Finished;
            ParticleSpawner.instance.SpawnParticleEffect(roomLight.transform.position, ParticleTypes.BlueGlitter_OverTime, lifetime: 2);
            var spawner = GetComponentInParent<RoomSpawner>();
            if (spawner != null)
                spawner.UnlockAllRooms();
        }
    }

    //Keep if needed to update foci calculations
    //private void OnDrawGizmos()
    //{
    //    var foci = Mathf.Sqrt(Mathf.Pow(lightTriggerRadius + magicFociNumber, 2) + Mathf.Pow(lightTriggerRadius, 2));  // major axis^2 - minor axis^2 

    //    Gizmos.DrawWireSphere(new Vector3(light.transform.position.x - foci, light.transform.position.y, 0), 5.0f);
    //    Gizmos.DrawWireSphere(new Vector3(light.transform.position.x + foci, light.transform.position.y, 0), 5.0f);
    //}

    private void SpawnMonsters()
    {
        foreach (Vector2 spawnPoint in spawnPoints)
        {
            float spawnFactor = minSpawnFactor + level * (maxSpawnFactor - minSpawnFactor);
            if (Random.Range(0f, 1f) < spawnFactor)
            {
                Vector2 temp = spawnPoint;
                temp.x += transform.position.x;
                temp.y += transform.position.y;
                GameObject mon = monsters[Random.Range(0, monsters.Length)];
                GameObject obj = Instantiate(mon, temp, mon.transform.rotation);
                myMonsters.Add(obj);
                var unit = obj.GetComponent<Unit>();
                unit.Stats.GainExperience((int)(1000 * level * LevelManager.TempleFloor));
                unit.OnDeath += OnMonsterDeath;
                // TODO: Add temple level
            }
        }
    }

    public void SetDoor(bool open, string direction)
    {
        DoorController door = doors[direction];
        door.LockDoor();
    }

    public void SetAllDoors(bool open)
    {
        SetDoor(open, Direction.EAST);
        SetDoor(open, Direction.SOUTH);
    }

    public void LockAllDoors()
    {
        doors[Direction.EAST].LockDoor();
        doors[Direction.SOUTH].LockDoor();
    }

    public void UnlockAllDoors()
    {
        doors[Direction.EAST].UnlockDoor();
        doors[Direction.SOUTH].UnlockDoor();
    }

    public void SetLevel(float level)
    {
        this.level = level;
        lightController.SetColor(startColor + level * (endColor - startColor));
    }

    void OnDrawGizmos()
    {
        foreach (Vector2 v in spawnPoints)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(v, 0.5f);
        }
    }

    public void SetNorthWallActive(bool active)
    {
        GameObject northWall = transform.Find("NorthSolid").gameObject;
        northWall.SetActive(active);
    }

    public void SetWestWallActive(bool active)
    {
        GameObject westWall = transform.Find("WestSolid").gameObject;
        westWall.SetActive(active);
    }

    public void SetSouthWallActive(bool active)
    {
        GameObject southWall = transform.Find("South").gameObject;
        GameObject southSolidWall = transform.Find("SouthSolid").gameObject;
        southWall.SetActive(!active);
        southSolidWall.SetActive(active);
    }

    public void SetEastWallActive(bool active)
    {
        GameObject eastWall = transform.Find("East").gameObject;
        GameObject eastSolidWall = transform.Find("EastSolid").gameObject;
        eastWall.SetActive(!active);
        eastSolidWall.SetActive(active);
    }

    public void SetShouldSpawnMonsters(bool ayy)
    {
        shouldSpawnMonsters = ayy;
        UnlockAllDoors();
    }
}