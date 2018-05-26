using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class Maze : MonoBehaviour
{
    enum Direction { NORTH, WEST, EAST, SOUTH };

    [System.Serializable] // Makes class visible in the Inspector
    public class Cell
    {
        public bool visited, isNode = true, isPath = false; // Is visited upon creation of maze (and in path chasing), is a Node
        public GameObject north, west, east, south; // 1, 2, 3, 4 - Respectively
        public Cell northNeighbor, westNeighbor, eastNeighbor, southNeighbor; // Neighboring cells
        public GameObject cellFloor;
        public int x_coor;
        public int y_coor;
    }

    public class Floor
    {
        public int floorIndex;
    }

    public List<int> selected;

    // Variable declarations and initializations
	public static MazeSize sizes = new MazeSize();
    public GameObject wall, floor, sphere;
	public int xSize = sizes.xAxis;
	public int ySize = sizes.yAxis;
    public float wallLength = 1.0f;
    public float debugFlashWaitTime = 0.05f;
  
    private Vector3 initialPos;
    private GameObject wallHolder, floorHolder; // To hold wall clones, to hold floor clones
    public Cell[] cells; // Array to hold cells -- make private later
    public List<Cell> allNodes;
    public int[,] adjacencyMatrix;
    public int currentCell, totalNodes, totalPaths;
    private int totalCells;
    private int visitedCells; // Number of cells that have been visited
    private bool startedBuilding = false;
    private int currentNeighbor = 0;
    private List<int> lastCells;
    private int backingUp = 0, wallToBreak = 0;
    public Dijkstra dijkstra;
    public int start = 5;
    public int finish = 1;
    public int raiseFloorHeight = 10;
    public List<int> d_tree;
    private int[] floorIndices;
    public int startCellIndex, endCellIndex;
    private bool startChosen = false, endChosen = false, textSet = false;
    public bool ran_dijkstra = false;
    GameObject runner;
    // Use this for initialization
    void Start()
    {

		xSize = PlayerPrefs.GetInt("xAxis", 5);
		ySize = PlayerPrefs.GetInt("yAxis", 5);
        Debug.Log("Xsize: " + xSize);
        Debug.Log("Ysize: " + ySize);
        d_tree = new List<int>();
        dijkstra = new Dijkstra();
        CreateWallsFloors();
        StartCoroutine(CheckNodes()); // DEBUG
        //Debug.Log(string.Join(", ", d_tree));
    }

    // DEBUG
    IEnumerator CheckNodes()
    {
        yield return new WaitForSeconds(2.0f);
       
        for (int i = 0; i < adjacencyMatrix.GetLength(0); i++)
        {
            var canvas = allNodes[i].cellFloor.transform.GetComponentInChildren<Canvas>();
            var text = canvas.transform.GetComponentInChildren<Text>();
            text.text = i.ToString();
        }
    }

    void CreateWallsFloors()
    {
        totalNodes = xSize * ySize; // Initialize Node and Path counts
        totalPaths = 0;
        wallHolder = new GameObject(); // Create object for wallHolder
        wallHolder.name = "Maze_Walls"; // Rename to Maze in hierarchy
        floorHolder = new GameObject(); // Create object for floorHolder
        floorHolder.name = "Maze_Floors";
        initialPos = new Vector3((-xSize / 2) + wallLength / 2, 0f, (-ySize / 2) + wallLength / 2);
        Vector3 myPos = initialPos;
        GameObject tempWall, tempFloor;

        // For X Axis
        for (int i = 0; i < ySize; i++)
        {
            for (int j = 0; j <= xSize; j++) // <= because need one more wall on x-axis than walls on y
            {
                myPos = new Vector3(initialPos.x + (j * wallLength) - wallLength / 2, 0.0f, initialPos.z + (i * wallLength) - wallLength / 2);
                tempWall = Instantiate(wall, myPos, Quaternion.identity) as GameObject; // Spawn wall object
                tempWall.transform.parent = wallHolder.transform; // put each wall as a gameObject child in wallHolder
            }
        }

        // For Y Axis
        for (int i = 0; i <= ySize; i++)
        {
            for (int j = 0; j < xSize; j++) // <= because need one more wall on x-axis than walls on y
            {
                myPos = new Vector3(initialPos.x + (j * wallLength), 0.0f, initialPos.z + (i * wallLength) - wallLength); // not /2 because putting at bottom of initialPos
                tempWall = Instantiate(wall, myPos, Quaternion.Euler(0.0f, 90.0f, 0.0f)) as GameObject; // spawn gameObject as tempWall, rotate 90 degrees on Y-axis
                tempWall.transform.parent = wallHolder.transform; // put each wall as a gameObject child in wallHolder
            }
        }

        // Create floors
        for (int i = 0; i <= ySize - 1; i++)
        {
            for (int j = 0; j < xSize; j++)
            {
                myPos = new Vector3(initialPos.x + (j * wallLength), -0.5f, initialPos.z + (i * wallLength) - wallLength / 2);
                tempFloor = Instantiate(floor, myPos, Quaternion.identity) as GameObject;
                tempFloor.transform.parent = floorHolder.transform; // put each floor as a gameObject child in floorHolder
            }
        }

        CreateCells();
        LabelNodes();
        CreateAdjacencyMatrix();

    }

    void LabelNodes()
    {
        Debug.Log("in labelnodes");
        for (int i = 0; i < cells.Length; i++)
        {
            Cell currCell = cells[i];
            currCell.visited = false; // Reset for path chase

            // Check for parallel walls/openings for path
            if ((currCell.north == null && currCell.south == null
               && currCell.east != null && currCell.west != null) ||
               (currCell.north != null && currCell.south != null
               && currCell.east == null && currCell.west == null))
            {
                currCell.isNode = false;
                currCell.isPath = true;
                allNodes.Remove(currCell); // Remove this cell from Nodes list

                // Update Node/Path count
                totalNodes--;
                totalPaths++;

                // Change color of the path
                
            } 
        }
    }

    void CreateAdjacencyMatrix() 
    {
        adjacencyMatrix = new int[allNodes.Count, allNodes.Count];
        // Initialize path weights to -1 (nonexistent) except Node to self = 0
        for (int i = 0; i < adjacencyMatrix.GetLength(0); i++)
        {
            for (int j = 0; j < adjacencyMatrix.GetLength(1); j++)
            {
                if (i == j)
                    adjacencyMatrix[i, j] = 0;
                else
                    adjacencyMatrix[i, j] = -1;
            }
        }

        // Iterate through Nodes list and follow each path
        for (int i = 0; i < allNodes.Count; i++)
        {
            Cell checking = allNodes[i];
            int cellArrayIndex = System.Array.IndexOf(cells, allNodes[i]);

            if (checking.northNeighbor != null && checking.north == null)
            {
                // If adjacent Node to north
                if (checking.northNeighbor.isNode == true)
                {
                    UpdateAdjacencyMatrix(i, allNodes.IndexOf(cells[cellArrayIndex + xSize]), 1);
                }
                else
                    FollowPath(i, allNodes[i].northNeighbor, Direction.NORTH, 1);
            }

            if (checking.westNeighbor != null && checking.west == null)
            {
                // If adjacent Node to west
                if (checking.westNeighbor.isNode == true)
                    UpdateAdjacencyMatrix(i, allNodes.IndexOf(cells[cellArrayIndex - 1]), 1);
                else
                    FollowPath(i, allNodes[i].westNeighbor, Direction.WEST, 1);
            }

            if (checking.eastNeighbor != null && checking.east == null)
            {
                // If adjacent Node to east
                if (checking.eastNeighbor.isNode == true)
                    UpdateAdjacencyMatrix(i, allNodes.IndexOf(cells[cellArrayIndex + 1]), 1);
                else
                    FollowPath(i, allNodes[i].eastNeighbor, Direction.EAST, 1);
            }

            if (checking.southNeighbor != null && checking.south == null)
            {
                // If adjacent Node to east
                if (checking.southNeighbor.isNode == true)
                    UpdateAdjacencyMatrix(i, allNodes.IndexOf(cells[cellArrayIndex - xSize]), 1);
                else
                    FollowPath(i, allNodes[i].southNeighbor, Direction.SOUTH, 1);
            }
        }
    }


    // Follow a path until Node hit, call to update adjacency matrix
    void FollowPath(int originalIndex, Cell nextNeighbor, Direction direction, int edgeWeight) 
    {
        switch (direction)
        {
            case Direction.NORTH:
                Cell tempNorthNeighbor = nextNeighbor;
                // Path already followed
                if (tempNorthNeighbor.visited == true)
                {
                    Debug.Log(string.Format("North from {0} visited, breaking", originalIndex));
                    break;
                }
                else if (tempNorthNeighbor.isNode == true)
                { // Reached end of path 
                    UpdateAdjacencyMatrix(originalIndex, allNodes.IndexOf(tempNorthNeighbor), edgeWeight);
                    break;
                }
                // Else continue to follow path
                tempNorthNeighbor.visited = true; // Set this path to visited so no repeat
                FollowPath(originalIndex, tempNorthNeighbor.northNeighbor, Direction.NORTH, edgeWeight + 1);
                break;

            case Direction.WEST:
                Cell tempWestNeighbor = nextNeighbor;
                // Path already followed
                if (tempWestNeighbor.visited == true)
                {
                    Debug.Log(string.Format("West from {0} visited, breaking", originalIndex));
                    break;
                }
                else if (tempWestNeighbor.isNode == true)
                { // Reached end of path 
                    UpdateAdjacencyMatrix(originalIndex, allNodes.IndexOf(tempWestNeighbor), edgeWeight);
                    break;
                }
                // Else continue to follow path
                tempWestNeighbor.visited = true; // Set this path to visited so no repeat
                FollowPath(originalIndex, tempWestNeighbor.westNeighbor, Direction.WEST, edgeWeight + 1);
                break;

            case Direction.EAST:
                Cell tempEastNeighbor = nextNeighbor;
                // Path already followed
                if (tempEastNeighbor.visited == true)
                {
                    Debug.Log(string.Format("East from {0} visited, breaking", originalIndex));
                    break;
                }
                else if (tempEastNeighbor.isNode == true)
                { // Reached end of path 
                    UpdateAdjacencyMatrix(originalIndex, allNodes.IndexOf(tempEastNeighbor), edgeWeight);
                    break;
                }
                // Else continue to follow path
                tempEastNeighbor.visited = true; // Set this path to visited so no repeat
                FollowPath(originalIndex, tempEastNeighbor.eastNeighbor, Direction.EAST, edgeWeight + 1);
                break;

            case Direction.SOUTH:
                Cell tempSouthNeighbor = nextNeighbor;
                // Path already followed
                if (tempSouthNeighbor.visited == true)
                {
                    Debug.Log(string.Format("South from {0} visited, breaking", originalIndex));
                    break;
                }
                else if (tempSouthNeighbor.isNode == true)
                { // Reached end of path 
                    UpdateAdjacencyMatrix(originalIndex, allNodes.IndexOf(tempSouthNeighbor), edgeWeight);
                    break;
                }
                // Else continue to follow path
                tempSouthNeighbor.visited = true; // Set this path to visited so no repeat
                FollowPath(originalIndex, tempSouthNeighbor.southNeighbor, Direction.SOUTH, edgeWeight + 1);
                break;
        }
    }

    // Update adjency matrix mirroring path directions
    void UpdateAdjacencyMatrix(int index1, int index2, int edgeWeight)
    {
        adjacencyMatrix[index1, index2] = edgeWeight;
        adjacencyMatrix[index2, index1] = edgeWeight;
        Debug.Log(string.Format("New edge: {0} to {1} with weight {2}", index1, index2, edgeWeight));
    }

    void CreateCells()
    {
        // Variable declarations and initializations
        lastCells = new List<int>();
        lastCells.Clear();
        totalCells = xSize * ySize;
        int allWallCount = wallHolder.transform.childCount; // Amount of walls created
        int allFloorCount = floorHolder.transform.childCount;
        GameObject[] allWalls = new GameObject[allWallCount]; // Array of size all children walls
        GameObject[] allFloors = new GameObject[floorHolder.transform.childCount];
        cells = new Cell[xSize * ySize]; // size of rows and columns
        int westEastProcess = 0;
        int childProcess = 0;
        int termCount = 0;

        // Get all the children (all the walls)
        for (int i = 0; i < allWallCount; i++)
        {
            allWalls[i] = wallHolder.transform.GetChild(i).gameObject; // get child at index of i as gameobject and store into array
        }

        // Get all the floor children
        for (int i = 0; i < allFloorCount; i++)
        {
            allFloors[i] = floorHolder.transform.GetChild(i).gameObject;
        }

        // Assign walls to cells
        for (int cellProcess = 0; cellProcess < cells.Length; cellProcess++)
        {

            if (termCount == xSize) // If equal to last cell, reset horizontal count
            {
                westEastProcess++; // correction, not inc by 2, inc by 1
                termCount = 0;
            }

            cells[cellProcess] = new Cell();
            cells[cellProcess].x_coor = cellProcess % xSize;        // sets cetrigian coor for cell location
            cells[cellProcess].y_coor = cellProcess % ySize;
            cells[cellProcess].west = allWalls[westEastProcess];
            cells[cellProcess].south = allWalls[childProcess + (xSize + 1) * ySize]; // first wall created after all vertical walls

            westEastProcess++;
            termCount++;
            childProcess++;

            cells[cellProcess].east = allWalls[westEastProcess];
            cells[cellProcess].north = allWalls[(childProcess + (xSize + 1) * ySize) + xSize - 1];

            // Assign floor to Cell
            cells[cellProcess].cellFloor = allFloors[cellProcess];
            allNodes.Add(cells[cellProcess]); // TEST add nodes to list
        }

        AssignNeighborCells();
        CreateMaze();

    }

    void AssignNeighborCells()
    {

        for (int i = 0; i < cells.Length; i++)
        {
            // If cells are in the bottom row
            if (i < xSize)
            {
                // If cell is in bottom right corner
                if (i == (xSize - 1))
                {
                    cells[i].northNeighbor = cells[i + xSize];
                    cells[i].westNeighbor = cells[i - 1];
                }
                else
                {
                    switch (i)
                    {
                        case 0: // Cell is bottom left corner
                            cells[i].northNeighbor = cells[i + xSize];
                            cells[i].eastNeighbor = cells[i + 1];
                            break;
                        default: // Cell somewhere in middle of bottom row
                            cells[i].northNeighbor = cells[i + xSize];
                            cells[i].westNeighbor = cells[i - 1];
                            cells[i].eastNeighbor = cells[i + 1];
                            break;
                    }
                }
                continue;
            }

            // If cells are in the top row
            if (i >= (cells.Length - xSize))
            {
                // If cell is in top left corner
                if (i == cells.Length - xSize)
                {
                    cells[i].eastNeighbor = cells[i + 1];
                    cells[i].southNeighbor = cells[i - xSize];
                }
                else if (i == (cells.Length - 1)) // If cell is in top right corner
                {
                    cells[i].westNeighbor = cells[i - 1];
                    cells[i].southNeighbor = cells[i - xSize];
                }
                else // Cell somewhere in middle of top row
                {
                    cells[i].westNeighbor = cells[i - 1];
                    cells[i].eastNeighbor = cells[i + 1];
                    cells[i].southNeighbor = cells[i - xSize];
                }
                continue;
            }

            // If on left edge but not in corner
            if ((i % xSize) == 0 && i != 0 && i != (cells.Length - xSize))
            {
                cells[i].northNeighbor = cells[i + xSize];
                cells[i].eastNeighbor = cells[i + 1];
                cells[i].southNeighbor = cells[i - xSize];
                continue;
            }

            // If on right edge but not in corner
            if (((i + 1) % xSize) == 0 && i != (cells.Length - 1) && i != (xSize - 1))
            {
                cells[i].northNeighbor = cells[i + xSize];
                cells[i].eastNeighbor = cells[i + 1];
                cells[i].southNeighbor = cells[i - xSize];
                continue;
            }

            cells[i].northNeighbor = cells[i + xSize];
            cells[i].westNeighbor = cells[i - 1];
            cells[i].eastNeighbor = cells[i + 1];
            cells[i].southNeighbor = cells[i - xSize];

        }
    }

    void CreateMaze()
    {
        while (visitedCells < totalCells) // 'if' here to use Invoke below
        {
            if (startedBuilding)
            {
                GiveNeighbor();
                if (cells[currentNeighbor].visited == false && cells[currentCell].visited == true)
                {
                    BreakWall();
                    cells[currentNeighbor].visited = true;
                    visitedCells++;
                    lastCells.Add(currentCell); // Add to visited list
                    currentCell = currentNeighbor; // Move to neighbor
                    if (lastCells.Count > 0)
                    {
                        backingUp = lastCells.Count - 1; // Reset top value of cell stack
                    }

                }
            }
            else
            {
                currentCell = Random.Range(0, totalCells); // Start the building process at random cell
                cells[currentCell].visited = true;
                visitedCells++;
                startedBuilding = true;
            }

            //Invoke("CreateMaze", 0.0f); 
        }
        Debug.Log("Completed");
    }

    void BreakWall()
    {
        switch (wallToBreak)
        {
            case 1:
                Destroy(cells[currentCell].north);
                cells[currentCell].north = null;
                cells[currentCell + xSize].south = null;
                break;
            case 2:
                Destroy(cells[currentCell].west);
                cells[currentCell].west = null;
                cells[currentCell - 1].east = null;
                break;
            case 3:
                Destroy(cells[currentCell].east);
                cells[currentCell].east = null;
                cells[currentCell + 1].west = null;
                break;
            case 4:
                Destroy(cells[currentCell].south);
                cells[currentCell].south = null;
                cells[currentCell - xSize].north = null;
                break;
        }
    }

    void GiveNeighbor()
    {

        int length = 0; // How many neighbors found
        int[] neighbors = new int[4]; // Max of 4 neighbors
        int check = 0; // Cornering cell
        int[] connectingWall = new int[4];

        check = ((currentCell + 1) / xSize); // Checking xAxis - are we on last element on x-axis? Means no wall to right
        check -= 1;
        check *= xSize;
        check += xSize;

        // east wall (right)
        if (currentCell + 1 < totalCells && (currentCell + 1) != check) // Check if exceeding total cells limit and if on edge
        {
            if (cells[currentCell + 1].visited == false)
            {
                neighbors[length] = currentCell + 1;
                connectingWall[length] = 3; // east
                length++;
            }
        }

        // Left side (west)
        if (currentCell - 1 >= 0 && currentCell != check) // Check if exceeding total cells limit and if on edge
        {
            if (cells[currentCell - 1].visited == false)
            {
                neighbors[length] = currentCell - 1;
                connectingWall[length] = 2; // west
                length++;
            }
        }

        // North wall (top z axis)
        if (currentCell + xSize < totalCells) // Check if exceeding total cells limit and if on edge
        {
            if (cells[currentCell + xSize].visited == false)
            {
                neighbors[length] = currentCell + xSize;
                connectingWall[length] = 1; // North 
                length++;
            }
        }

        // South wall (bottom z axis)
        if (currentCell - xSize >= 0) // Check if exceeding total cells limit and if on edge
        {
            if (cells[currentCell - xSize].visited == false)
            {
                neighbors[length] = currentCell - xSize;
                connectingWall[length] = 4; // South
                length++;
            }
        }

        // Debug log
        /*for (int i = 0; i < length; i++)
        {
            Debug.Log(neighbors[i]);
        } */

        // If neighbors still exist
        if (length != 0)
        {
            int randomCellChosen = Random.Range(0, length);
            currentNeighbor = neighbors[randomCellChosen];
            wallToBreak = connectingWall[randomCellChosen];
        }
        else // Otherwise back up
        {
            if (backingUp > 0) // Not defined any of neighbors, backup
            {
                currentCell = lastCells[backingUp];
                backingUp--;
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (ran_dijkstra == false && selected.Count == 1) {
            Text startEndText = GameObject.Find("Start_End_Text").GetComponent<Text>();
            startEndText.text = "Select finish location";

            if (textSet == false)
            {
                textSet = true;
                Text startText = GameObject.Find("startText").GetComponent<Text>();
                startText.text += " " + selected[0];
            }
        }
        if (ran_dijkstra == false && selected.Count == 2)
        {
            Text endText = GameObject.Find("endText").GetComponent<Text>();
            Text startEndText = GameObject.Find("Start_End_Text").GetComponent<Text>();
            endText.text += " " + selected[1];
            startEndText.text = "";

            ran_dijkstra = true;
            d_tree = dijkstra.Dijkstra_Solve(adjacencyMatrix, allNodes.Count, selected[0], selected[1]); // change for after testing purposes
            Vector3 pos = allNodes[d_tree[0]].cellFloor.transform.position;
            pos.y = pos.y + 0.1f;
            runner = Instantiate(sphere, pos, Quaternion.identity) as GameObject;
        }

        Scrollbar bar = GameObject.Find("Scrollbar").GetComponent<Scrollbar>();
        Maze_Runner.speed = (float)(bar.value * 10);
    }

    public void ChooseEndpoint(Transform clickedObject)
    {
        clickedObject.transform.gameObject.GetComponent<Renderer>().material.color = Color.yellow;
        clickedObject.transform.Translate(0.0f, -0.15f, 0.0f);
    }
}
