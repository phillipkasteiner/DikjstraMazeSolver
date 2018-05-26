using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow_Dijkstra_Path : MonoBehaviour {

    public enum Direction { NORTH, WEST, EAST, SOUTH };

    public Maze.Cell currCell;
    private List<Maze.Cell> allNodes;
    private List<Maze.Cell> potentialDijkstraPath = new List<Maze.Cell>();
    private Maze.Cell[] cells;
    private List<int> d_tree;
    private int tempIndex;
    private Maze.Cell tempCell = new Maze.Cell();
    private Maze.Cell currrentCell = new Maze.Cell();
   
    public Follow_Dijkstra_Path(List<Maze.Cell> nodes, Maze.Cell[] cellArr, List<int> tree)
    {
        this.allNodes = nodes;
        this.cells = cellArr;
        this.d_tree = tree;
    }

	// Use this for initialization
	public void Start () {
	}
	
	// Update is called once per frame
	void Update () {
    }
    
    public void FollowDijkstraPath(Maze.Cell currentCell, int index, Direction direction)
    {
        switch(direction)
        {
            case Direction.NORTH:
                // If no north neighbor
                Debug.Log("-------- GOING NORTH ------------");
                Debug.Log("Current Cell Index in allNodes: " + allNodes.IndexOf(currentCell));
                Debug.Log("If path, cells index: " + System.Array.IndexOf(cells, currentCell));
                Debug.Log("Using this index: " + index);
                Debug.Log("--------------");
                if(currentCell.north != null || currentCell.northNeighbor.visited)
                {
                    FollowDijkstraPath(currentCell, index, Direction.WEST);
                }
                Maze.Cell tempCell = new Maze.Cell();
                tempCell = currentCell.northNeighbor;
                if (tempCell.isNode)
                {
                    tempCell.visited = true;
                    Debug.Log("Tempcell index: dtree next index -- " + allNodes.IndexOf(tempCell) + " : " + d_tree[index + 1]);
                    // If this is our next Node in path
                    if (allNodes.IndexOf(tempCell) == d_tree[index + 1])
                    {
                        // color all path behind Green, remove until potential path empty
                        for (int i = 0; i < potentialDijkstraPath.Count; i++)
                        {
                            potentialDijkstraPath[i].cellFloor.GetComponent<Renderer>().material.color = Color.green;
                            potentialDijkstraPath.Remove(potentialDijkstraPath[i]);
                        }
                        currentCell.cellFloor.GetComponent<Renderer>().material.color = Color.green;
                        tempCell.cellFloor.GetComponent<Renderer>().material.color = Color.green;
                        // If final node
                        if(allNodes.IndexOf(tempCell) == d_tree[d_tree.Count - 1]) { Debug.Log("Path complete"); break; }
                        index++;
                        FollowDijkstraPath(tempCell, index, Direction.NORTH);
                    }
                    else // Not next node in path, backtrack and go West
                    {
                        tempCell.cellFloor.GetComponent<Renderer>().material.color = Color.gray;
                        tempIndex = potentialDijkstraPath.Count; 

                        //for (int i = 0; i < tempIndex; i++)
                        //{
                        //    potentialDijkstraPath[tempIndex].cellFloor.GetComponent<Renderer>().material.color = Color.gray;
                        //    //yield return new WaitForSeconds(0.1f);
                        //    potentialDijkstraPath.Remove(potentialDijkstraPath[tempIndex]);
                        //    tempIndex--;
                        //}
                        Debug.Log("Dtree at index: " + d_tree[index]);
                        var llc = d_tree[index];
                        FollowDijkstraPath(allNodes[d_tree[index]], index, Direction.WEST);
                    }
                }
                else // Not a node
                {
                    currentCell.visited = true;
                    potentialDijkstraPath.Add(currentCell);
                    FollowDijkstraPath(tempCell, index, Direction.NORTH);
                }
                break;

            case Direction.WEST:
                // If no west neighbor
                // If no north neighbor
                Debug.Log("-------- GOING WEST ------------");
                Debug.Log("Current Cell Index in allNodes: " + allNodes.IndexOf(currentCell));
                Debug.Log("If path, cells index: " + System.Array.IndexOf(cells, currentCell));
                Debug.Log("Using this index: " + index);
                Debug.Log("--------------");
                if (currentCell.west != null || currentCell.northNeighbor.visited)
                {
                    FollowDijkstraPath(currentCell, index, Direction.EAST);
                }
                tempCell = currentCell.westNeighbor;
                if (tempCell.isNode)
                {
                    tempCell.visited = true;
                    // If this is our next Node in path
                    if (allNodes.IndexOf(tempCell) == d_tree[index + 1])
                    {
                        // color all path behind Green, remove until potential path empty
                        for (int i = 0; i < potentialDijkstraPath.Count; i++)
                        {
                            potentialDijkstraPath[i].cellFloor.GetComponent<Renderer>().material.color = Color.green;
                            potentialDijkstraPath.Remove(potentialDijkstraPath[i]);
                        }
                        tempCell.cellFloor.GetComponent<Renderer>().material.color = Color.green;
                        currentCell.cellFloor.GetComponent<Renderer>().material.color = Color.green;
                        // If final node
                        if (allNodes.IndexOf(tempCell) == d_tree[d_tree.Count - 1]) { Debug.Log("Path complete"); break; }
                        index++;
                        FollowDijkstraPath(tempCell, index, Direction.NORTH);
                    }
                    else // Not next node in path, backtrack and go East
                    {
                        tempCell.cellFloor.GetComponent<Renderer>().material.color = Color.gray;
                        tempIndex = potentialDijkstraPath.Count; //-1

                        //if (tempIndex > 0)
                        //{
                        //    while (potentialDijkstraPath.Count > 0)
                        //    {
                        //        potentialDijkstraPath[tempIndex].cellFloor.GetComponent<Renderer>().material.color = Color.gray;
                        //        //yield return new WaitForSeconds(0.1f);
                        //        potentialDijkstraPath.Remove(potentialDijkstraPath[tempIndex]);
                        //        tempIndex--;
                        //    }
                        //}
                        Debug.Log("Dtree at index: " + d_tree[index]);
                        var llc = d_tree[index];
                        FollowDijkstraPath(allNodes[d_tree[index]], index, Direction.EAST);
                    }
                }
                else // Not a node
                {
                    currentCell.visited = true;
                    potentialDijkstraPath.Add(currentCell);
                    FollowDijkstraPath(tempCell, index, Direction.WEST);
                }
                break;

            case Direction.EAST:
                // If no east neighbor
                if (currentCell.east != null || currentCell.northNeighbor.visited)
                {
                    FollowDijkstraPath(currentCell, index, Direction.SOUTH);
                }
                tempCell = currentCell.eastNeighbor;
                if (tempCell.isNode)
                {
                    tempCell.visited = true;
                    // If this is our next Node in path
                    if (allNodes.IndexOf(tempCell) == d_tree[index + 1])
                    {
                        // color all path behind Green, remove until potential path empty
                        for (int i = 0; i < potentialDijkstraPath.Count; i++)
                        {
                            potentialDijkstraPath[i].cellFloor.GetComponent<Renderer>().material.color = Color.green;
                            potentialDijkstraPath.Remove(potentialDijkstraPath[i]);
                        }
                        tempCell.cellFloor.GetComponent<Renderer>().material.color = Color.green;
                        currentCell.cellFloor.GetComponent<Renderer>().material.color = Color.green;
                        // If final node
                        if (allNodes.IndexOf(tempCell) == d_tree[d_tree.Count - 1]) { Debug.Log("Path complete"); break; }
                        index++;
                        FollowDijkstraPath(tempCell, index, Direction.NORTH);
                    }
                    else // Not next node in path, backtrack and go South
                    {
                        tempCell.cellFloor.GetComponent<Renderer>().material.color = Color.gray;
                        tempIndex = potentialDijkstraPath.Count; //-1

                        //if (tempIndex > 0)
                        //{
                        //    while (potentialDijkstraPath.Count > 0)
                        //    {
                        //        potentialDijkstraPath[tempIndex].cellFloor.GetComponent<Renderer>().material.color = Color.gray;
                        //        //yield return new WaitForSeconds(0.1f);
                        //        potentialDijkstraPath.Remove(potentialDijkstraPath[tempIndex]);
                        //        tempIndex--;
                        //    }
                        //}
                        Debug.Log("Dtree at index: " + d_tree[index]);
                        var llc = d_tree[index];
                        FollowDijkstraPath(allNodes[d_tree[index]], index, Direction.SOUTH);
                    }
                }
                else // Not a node
                {
                    currentCell.visited = true;
                    potentialDijkstraPath.Add(currentCell);
                    FollowDijkstraPath(tempCell, index, Direction.EAST);
                }
                break;

            case Direction.SOUTH:
                // If no south neighbor
                if (currentCell.south != null || currentCell.northNeighbor.visited)
                {
                    FollowDijkstraPath(currentCell, index, Direction.NORTH);
                }
                tempCell = currentCell.southNeighbor;
                if (tempCell.isNode)
                {
                    tempCell.visited = true;
                    // If this is our next Node in path
                    if (allNodes.IndexOf(tempCell) == d_tree[index + 1])
                    {
                        // color all path behind Green, remove until potential path empty
                        for (int i = 0; i < potentialDijkstraPath.Count; i++)
                        {
                            potentialDijkstraPath[i].cellFloor.GetComponent<Renderer>().material.color = Color.green;
                            potentialDijkstraPath.Remove(potentialDijkstraPath[i]);
                        }
                        tempCell.cellFloor.GetComponent<Renderer>().material.color = Color.green;
                        currentCell.cellFloor.GetComponent<Renderer>().material.color = Color.green;
                        // If final node
                        if (allNodes.IndexOf(tempCell) == d_tree[d_tree.Count - 1]) { Debug.Log("Path complete"); break; }
                        index++;
                        FollowDijkstraPath(tempCell, index, Direction.NORTH);
                    }
                    else // Not next node in path, backtrack and go North
                    {
                        tempCell.cellFloor.GetComponent<Renderer>().material.color = Color.gray;
                        tempIndex = potentialDijkstraPath.Count; //-1

                        //if (tempIndex > 0)
                        //{
                        //    while (potentialDijkstraPath.Count > 0)
                        //    {
                        //        potentialDijkstraPath[tempIndex].cellFloor.GetComponent<Renderer>().material.color = Color.gray;
                        //        //yield return new WaitForSeconds(0.1f);
                        //        potentialDijkstraPath.Remove(potentialDijkstraPath[tempIndex]);
                        //        tempIndex--;
                        //    }
                        //}
                        Debug.Log("Dtree at index: " + d_tree[index]);
                        var llc = d_tree[index];
                        FollowDijkstraPath(allNodes[d_tree[index]], index, Direction.NORTH);
                    }
                }
                else // Not a node
                {
                    currentCell.visited = true;
                    potentialDijkstraPath.Add(currentCell);
                    FollowDijkstraPath(tempCell, index, Direction.SOUTH);
                }
                break;

            default:
                Debug.Log("Invalid path");
                break;
        }
    }
}
