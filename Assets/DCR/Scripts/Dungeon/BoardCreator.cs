using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BoardCreator : MonoBehaviour
{
    // The type of tile that will be laid in a specific position.
    public enum TileType
    {
        Wall, Floor,
    }
    public int boardColumns = 100;                                 // The number of boardColumns on the board (how wide it will be).
    public int boardRows = 100;                                    // The number of boardRows on the board (how tall it will be).
    public IntRange numRooms = new IntRange(15, 20);         // The range of the number of rooms there can be.
    public IntRange roomWidth = new IntRange(3, 10);         // The range of widths rooms can have.
    public IntRange roomHeight = new IntRange(3, 10);        // The range of heights rooms can have.
    public IntRange corridorLength = new IntRange(6, 10);    // The range of lengths corridors between rooms can have.

    [Tooltip("Size of one tile.It will be used as [x] and [y] value for Vector3)")]
    public int tileSize = 1;

    public List<DungeonData> dungeonSets;
    private int dungeonID;
    public GameObject bossRoom;

    public GameObject enemySpawner;
    public GameObject player;
    public GameObject enter, exit, boss;
    public Transform navmeshFloor;
    public GameObject lightPref;
    public GameObject corridorProps;
    public List<NavMeshSurface> floors;

    private TileType[][] tiles;                               // A jagged array of tile types representing the board, like a grid.
    private Room[] rooms;                                     // All the rooms that are created for this board.
    private Corridor[] corridors;                             // All the corridors that connect the rooms.
    [HideInInspector] public GameObject boardHolder;          // GameObject that acts as a container for all other tiles.
    private GameObject propsHolder;
    private GameObject spawnersHolder;
    private GameObject floorsHolder;
    private GameObject wallsHolder;
    private GameObject lightHolder;

    private void Awake()
    {
        // Create the board holder.
        boardHolder = new GameObject("BoardHolder");
    }

    private void Start()
    {
        HoldersInit();

        SetupTilesArray();

        CreateRoomsAndCorridors();

        SetTilesValuesForRooms();
        SetTilesValuesForCorridors();

        InstantiateTiles();
        InstantiateOuterWalls();

        // BUILD NAVMESH RUNTIME! WOOOHOO!
        floorsHolder.GetComponent<NavMeshSurface>().BuildNavMesh();
        //boardHolder.transform.position = new Vector3(-(boardColumns * tileSize) / 2, 0, -(boardRows * tileSize) / 2);
    }

    void HoldersInit()
    {
        dungeonID = Random.Range(0, dungeonSets.Count);

        propsHolder = new GameObject("PropsHolder");
        propsHolder.transform.parent = boardHolder.transform;

        spawnersHolder = new GameObject("SpawnersHolder");
        spawnersHolder.transform.parent = boardHolder.transform;

        floorsHolder = new GameObject("FloorsHolder");
        floorsHolder.AddComponent<NavMeshSurface>();
        floorsHolder.GetComponent<NavMeshSurface>().collectObjects = CollectObjects.Children;
        floorsHolder.GetComponent<NavMeshSurface>().useGeometry = NavMeshCollectGeometry.PhysicsColliders;
        floorsHolder.transform.parent = boardHolder.transform;

        wallsHolder = new GameObject("WallsHolder");
        wallsHolder.transform.parent = boardHolder.transform;

        lightHolder = new GameObject("LightHolder");
        lightHolder.transform.parent = boardHolder.transform;

        navmeshFloor.localScale = new Vector3(boardColumns / 2 + 0.5f, -2.5f, boardRows / 2 + 0.5f);
    }

    void SetupTilesArray()
    {
        // Set the tiles jagged array to the correct width.
        tiles = new TileType[boardColumns][];

        // Go through all the tile arrays...
        for (int i = 0; i < tiles.Length; i++)
        {
            // ... and set each tile array is the correct height.
            tiles[i] = new TileType[boardRows];
        }


    }



    void CreateRoomsAndCorridors()
    {
        // Create the rooms array with a random size.
        rooms = new Room[numRooms.Random];

        // There should be one less corridor than there is rooms.
        corridors = new Corridor[rooms.Length - 1];

        // Create the first room and corridor.
        rooms[0] = new Room();
        corridors[0] = new Corridor();

        // Setup the first room, there is no previous corridor so we do not use one.
        rooms[0].SetupRoom(roomWidth, roomHeight, boardColumns, boardRows);

        // Setup the first corridor using the first room.
        corridors[0].SetupCorridor(rooms[0], corridorLength, roomWidth, roomHeight, boardColumns, boardRows, true);

        Vector3 playerPos = new Vector3(rooms[0].xPos * tileSize, 0, rooms[0].zPos * tileSize);
        GameObject playerGo = Instantiate(player, playerPos, Quaternion.identity);
        playerGo.transform.parent = boardHolder.transform;
        

        GameObject entertPref = Instantiate(enter, playerPos, Quaternion.identity);
        entertPref.transform.parent = boardHolder.transform;

        for (int i = 1; i < rooms.Length; i++)
        {
            // Create a room.
            rooms[i] = new Room();

            // Setup the room based on the previous corridor.
            rooms[i].SetupRoom(roomWidth, roomHeight, boardColumns, boardRows, corridors[i - 1]);

            // If we haven't reached the end of the corridors array...
            if (i < corridors.Length)
            {
                // ... create a corridor.
                corridors[i] = new Corridor();

                // Setup the corridor based on the room that was just created.
                corridors[i].SetupCorridor(rooms[i], corridorLength, roomWidth, roomHeight, boardColumns, boardRows, false);
            }

            if ((i % 2) == 0)
            {
                Vector3 lightPos = new Vector3(rooms[i].xPos * tileSize + rooms[i].roomHeight, 5.625f, rooms[i].zPos * tileSize + rooms[i].roomWidth);
                GameObject lightPrefInst = Instantiate(lightPref, lightPos, Quaternion.identity) as GameObject;
                lightPrefInst.transform.parent = lightHolder.transform;
            }

            if (i > 1 && i < rooms.Length)
            {

                Vector3 propsPos = new Vector3(Random.Range(rooms[i].xPos * tileSize, rooms[i].xPos * tileSize + rooms[i].roomWidth), 0, Random.Range(rooms[i].zPos * tileSize, rooms[i].zPos * tileSize + rooms[i].roomHeight));
                GameObject propsInstance = Instantiate(dungeonSets[dungeonID].propsTiles[Random.Range(0, dungeonSets[dungeonID].propsTiles.Length)], propsPos, Quaternion.identity);

                propsInstance.transform.parent = propsHolder.transform;

                Vector3 spawnerPos = new Vector3(rooms[i].xPos * tileSize+rooms[i].roomWidth, 0, rooms[i].zPos * tileSize+ rooms[i].roomHeight);
                GameObject enemySpawnerInstance = Instantiate(enemySpawner, spawnerPos, Quaternion.identity);

                enemySpawnerInstance.transform.parent = spawnersHolder.transform;
            }
        }

        Vector3 exitPos = new Vector3(
                                        Random.Range(rooms[rooms.Length - 1].xPos * tileSize, rooms[rooms.Length - 1].xPos * tileSize + rooms[rooms.Length - 1].roomWidth),
                                        0,
                                        Random.Range(rooms[rooms.Length - 1].zPos * tileSize, rooms[rooms.Length - 1].zPos * tileSize + rooms[rooms.Length - 1].roomHeight)
                                        );
        if ((PlayerPrefs.GetInt("level id") % 5) == 0 && (PlayerPrefs.GetInt("level id") != 0))
        {
            GameObject bossPref = Instantiate(boss, exitPos, Quaternion.identity);
            bossPref.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("sfx vol");
            GameManager.portal = bossPref;
            bossPref.SetActive(false);
        }
        else
        {

            GameObject exitPref = Instantiate(exit, exitPos, Quaternion.identity);
            exitPref.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("sfx vol");
            GameManager.portal = exitPref;
            exitPref.SetActive(false);
            exitPref.transform.parent = boardHolder.transform;
        }
    }


    void SetTilesValuesForRooms()
    {
        // Go through all the rooms...
        for (int i = 0; i < rooms.Length; i++)
        {
            Room currentRoom = rooms[i];

            // ... and for each room go through it's width.
            for (int j = 0; j < currentRoom.roomWidth; j++)
            {
                int xCoord = currentRoom.xPos + j;

                // For each horizontal tile, go up vertically through the room's height.
                for (int k = 0; k < currentRoom.roomHeight; k++)
                {
                    int zCoord = currentRoom.zPos + k;

                    // The coordinates in the jagged array are based on the room's position and it's width and height.
                    tiles[xCoord][zCoord] = TileType.Floor;
                }
            }

        }
    }


    void SetTilesValuesForCorridors()
    {
        // Go through every corridor...
        for (int i = 0; i < corridors.Length; i++)
        {
            Corridor currentCorridor = corridors[i];

            // and go through it's length.
            for (int j = 0; j < currentCorridor.corridorLength; j++)
            {
                // Start the coordinates at the start of the corridor.
                int xCoord = currentCorridor.startXPos;
                int zCoord = currentCorridor.startZPos;

                // Depending on the direction, add or subtract from the appropriate
                // coordinate based on how far through the length the loop is.
                switch (currentCorridor.direction)
                {
                    case Direction.North:
                        zCoord += j;
                        break;
                    case Direction.East:
                        xCoord += j;
                        break;
                    case Direction.South:
                        zCoord -= j;
                        break;
                    case Direction.West:
                        xCoord -= j;
                        break;
                }
                
                // Set the tile at these coordinates to Floor.
                tiles[xCoord][zCoord] = TileType.Floor;
            }
            GameObject corridorPropsGo = Instantiate(corridorProps, new Vector3(currentCorridor.startXPos * tileSize, 0.6f, currentCorridor.startZPos * tileSize), Quaternion.Euler(Quaternion.identity.x, Random.Range(-360,360), Quaternion.identity.z));
            corridorPropsGo.transform.parent = propsHolder.transform;
        }
    }

    void InstantiateTiles()
    {
        // Go through all the tiles in the jagged array...
        for (int i = 0; i < tiles.Length; i++)
        {
            for (int j = 0; j < tiles[i].Length; j++)
            {
                if (tiles[i][j] == TileType.Floor)
                {
                    // ... and instantiate a floor tile for it.
                    InstantiateFromArray(dungeonSets[dungeonID].floorTiles, i * tileSize, j * tileSize, floorsHolder);
                }
                
                // If the tile type is Wall...
                if (tiles[i][j] == TileType.Wall)
                {
                    // ... instantiate a wall over the top.
                    InstantiateFromArray(dungeonSets[dungeonID].wallTiles, i*tileSize, j*tileSize, wallsHolder);
   
                }
            }
        }
    }


    void InstantiateOuterWalls()
    {
        // The outer walls are one unit left, right, up and down from the board.
        float leftEdgeX = -1f;
        float rightEdgeX = boardColumns + 0f;
        float bottomEdgeY = -1f;
        float topEdgeY = boardRows + 0f;

        // Instantiate both vertical walls (one on each side).
        InstantiateVerticalOuterWall(leftEdgeX, bottomEdgeY, topEdgeY);
        InstantiateVerticalOuterWall(rightEdgeX, bottomEdgeY, topEdgeY);

        // Instantiate both horizontal walls, these are one in left and right from the outer walls.
        InstantiateHorizontalOuterWall(leftEdgeX + 1f, rightEdgeX - 1f, bottomEdgeY);
        InstantiateHorizontalOuterWall(leftEdgeX + 1f, rightEdgeX - 1f, topEdgeY);
    }


    void InstantiateVerticalOuterWall(float xCoord, float startingZ, float endingZ)
    {
        // Start the loop at the starting value for Y.
        float currentZ = startingZ;

        // While the value for Y is less than the end value...
        while (currentZ <= endingZ)
        {
            // ... instantiate an outer wall tile at the x coordinate and the current y coordinate.
            InstantiateFromArray(dungeonSets[dungeonID].outerWallTiles, xCoord*tileSize, currentZ*tileSize, wallsHolder);

            currentZ++;
        }
    }


    void InstantiateHorizontalOuterWall(float startingX, float endingX, float zCoord)
    {
        // Start the loop at the starting value for X.
        float currentX = startingX;

        // While the value for X is less than the end value...
        while (currentX <= endingX)
        {
            // ... instantiate an outer wall tile at the y coordinate and the current x coordinate.
            InstantiateFromArray(dungeonSets[dungeonID].outerWallTiles, currentX*tileSize, zCoord*tileSize, wallsHolder);

            currentX++;
        }
    }

    void InstantiateFromArray(GameObject[] prefabs, float xCoord, float zCoord, GameObject holder)
    {
        // Create a random index for the array.
        int randomIndex = Random.Range(0, prefabs.Length);

        // The position to be instantiated at is based on the coordinates.
        Vector3 position = new Vector3(xCoord, 0f, zCoord);

        // Create an instance of the prefab from the random index of the array.
        GameObject tileInstance = Instantiate(prefabs[randomIndex], position, Quaternion.identity) as GameObject;

        // Set the tile's parent to the board holder.
        tileInstance.transform.parent = holder.transform;

    }
}