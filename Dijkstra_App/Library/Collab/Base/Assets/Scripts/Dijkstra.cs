using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class Dijkstra : MonoBehaviour {

    public int[] dist;
    public int[] parent;
    public List<int> decision_tree;


   private int minDistance(int[] dist, bool[] sptSet, int n)
    {
        // Initialize min value
        int min = int.MaxValue;
        int min_index = 0;

        for (int v = 0; v < n; v++) { 
            if (sptSet[v] == false && dist[v] <= min) {
                min = dist[v];
                min_index = v;
            }
        }
        return min_index;
    }

    public List<int> Dijkstra_Solve(int[,] graph, int node_count, int src, int fin)
    {
        decision_tree = new List<int>();        
        // The output array. dist[i]
        // will hold the shortest
        // distance from src to i
        dist = new int[node_count];

        // sptSet[i] will true if vertex
        // i is included / in shortest
        // path tree or shortest distance 
        // from src to i is finalized
        bool[] sptSet = new bool[node_count];


        // Parent array to store
        // shortest path tree
        parent = new int[node_count];
        

        // Initialize all distances as 
        // INFINITE and stpSet[] as false
        for (int i = 0; i < node_count; i++)
        {
            parent[src] = -1;
            dist[i] = int.MaxValue;
            sptSet[i] = false;
        }

        // Distance of source vertex 
        // from itself is always 0
        dist[src] = 0;

        // Find shortest path
        // for all vertices
        for (int count = 0; count < node_count; count++)
        {
            // Pick the minimum distance
            // vertex from the set of
            // vertices not yet processed. 
            // u is always equal to src
            // in first iteration.
            int u = minDistance(dist, sptSet, node_count);

            // Mark the picked vertex 
            // as processed
            sptSet[u] = true;

            // Update dist value of the 
            // adjacent vertices of the
            // picked vertex.
            for (int v = 0; v < node_count; v++)
            {
                // Update dist[v] only if is
                // not in sptSet, there is
                // an edge from u to v, and 
                // total weight of path from
                // src to v through u is smaller
                // than current value of
                // dist[v]

                if (!sptSet[v] && graph[u, v] > 0 && dist[u] + graph[u, v] < dist[v])
                {
                    parent[v] = u;
                    dist[v] = dist[u] + graph[u, v];
                }
            }
            if (u == fin) {
                break;
            }
        }
        print_dijkstra(dist, src, fin, parent);
        Build_D_Tree(dist, src, fin, parent);
        return decision_tree;
    }

    private void Get_Path(int j) {
        if (parent[j] == -1) {
            return;
        }
        decision_tree.Add(j);
        Get_Path(parent[j]);
    }

    private void Build_D_Tree(int[] dist, int src, int fin, int[] parent) {
        for (int i = 0; i <= fin; i++) {
            if (dist[i] != int.MaxValue)
            {
                Get_Path(i);
            }
        }
    }

    private string printPath(int[] parent, int j)
    {
        string path = "";
        // Base Case : If j is source
        if (parent[j] == -1)
        {
            return "";
        }
        path += System.String.Concat("<-", parent[j].ToString());
        return path + printPath(parent, parent[j]);
    }

    // A utility function to print 
    // the constructed distance
    // array
    private void print_dijkstra(int[] dist, int src,int fin, int[] parent)
    {
        string log;
        for (int i = 1; i <= fin; i++)
        {
            if (dist[i] != int.MaxValue) { 
                log = "";
                log += System.String.Concat(src.ToString(), " -> ", i.ToString(), " ");
                log += System.String.Concat("Moves: ", dist[i].ToString(), " | ", i.ToString());
                log += printPath(parent, i);
                Debug.Log(log);
            }
        }
    }

    
}
