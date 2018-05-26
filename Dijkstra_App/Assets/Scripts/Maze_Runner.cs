using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Maze_Runner : MonoBehaviour {
    public bool d_r;
    public bool hasnt_run;
    public static float speed = 2.0f;

    IEnumerator run_maze() {
        GameObject maze = GameObject.Find("Maze_Gen");
        Maze maze_script = maze.GetComponent<Maze>();
        var d_tree = maze_script.d_tree;
        var all_nodes = maze_script.allNodes;
        Vector3 initial_position = new Vector3(all_nodes[d_tree[0]].cellFloor.transform.position.x,
                                                all_nodes[d_tree[0]].cellFloor.transform.position.y,
                                                all_nodes[d_tree[0]].cellFloor.transform.position.z);
        float debugFlashWaitTime = 0.0005f;
        // follow path
        foreach (int i in d_tree) {
            Vector3 next_position = new Vector3(all_nodes[i].cellFloor.transform.position.x, all_nodes[i].cellFloor.transform.position.y, all_nodes[i].cellFloor.transform.position.z);
            
            if (transform.position.x < next_position.x)
            {
                while (transform.position.x < next_position.x)
                {
                    transform.Translate(speed * Time.deltaTime, 0, 0);
                    high_light_path();
                    yield return new WaitForSeconds(debugFlashWaitTime);
                }
            }
            if (transform.position.x > next_position.x)
            {
                while (transform.position.x > next_position.x)
                {
                    transform.Translate(-1 * speed * Time.deltaTime, 0, 0);
                    high_light_path();
                    yield return new WaitForSeconds(debugFlashWaitTime);
                }
            }
            if (transform.position.z < next_position.z)
            {
                while (transform.position.z < next_position.z)
                {
                    transform.Translate(0, 0, speed * Time.deltaTime);
                    high_light_path();
                    yield return new WaitForSeconds(debugFlashWaitTime);
                }
            }
            if (transform.position.z > next_position.z)
            {
                while (transform.position.z > next_position.z)
                {
                    transform.Translate(0, 0, -1 * speed * Time.deltaTime);
                    high_light_path();
                    yield return new WaitForSeconds(debugFlashWaitTime);
                }
            }
        }
    }

    public void high_light_path() {
        Collider[] colliders;
        if ((colliders = Physics.OverlapSphere(transform.position, .25f /* Radius */)).Length > 1) //Presuming the object you are testing also has a collider 0 otherwise
        {
            foreach (var collider in colliders)
            {
                var go = collider.gameObject; //This is the game object you collided with
                if (go == gameObject) continue; //Skip the object itself
                go.transform.GetComponent<Renderer>().material.color = Color.yellow;                                //Do something
            }
        }
    }

	// Use this for initialization
	void Start () {
        GameObject maze = GameObject.Find("Maze_Gen");
        Maze maze_script = maze.GetComponent<Maze>();
        d_r = maze_script.ran_dijkstra;
        hasnt_run = true;
    }
	
	// Update is called once per frame
	void Update () {
        
        if (d_r && hasnt_run)
        {
            hasnt_run = false;
            StartCoroutine( run_maze());
        }
    }
}
