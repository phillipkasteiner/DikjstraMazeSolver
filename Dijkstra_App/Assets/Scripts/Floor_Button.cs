using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HoloToolkit.Unity.InputModule;

public class Floor_Button : MonoBehaviour {

    // change to holo lens click
    private void OnMouseDown()
    {
        GameObject maze = GameObject.Find("Maze_Gen");
        Maze maze_script = maze.GetComponent<Maze>();
        var canvas = gameObject.transform.GetComponentInChildren<Canvas>();
        var text = canvas.transform.GetComponentInChildren<Text>();
        if (text.text != "-")
        {
            var index = 0;
            var success = int.TryParse(text.text, out index);
            if (success && maze_script.selected.Count < 2)
            {
                maze_script.selected.Add(index);
                maze_script.allNodes[index].cellFloor.transform.GetComponent<Renderer>().material.color = Color.yellow;
            }
        }
    }

    public void GetFloorIndex() {
        GameObject maze = GameObject.Find("Maze_Gen");
        Maze maze_script = maze.GetComponent<Maze>();
        var canvas = gameObject.transform.GetComponentInChildren<TextMesh>();
        var text = canvas.text;
        if (text != "-")
        {
            var index = 0;
            var success = int.TryParse(text, out index);
            if (success && maze_script.selected.Count < 2)
            {
                maze_script.selected.Add(index);
                maze_script.allNodes[index].cellFloor.transform.GetComponent<Renderer>().material.color = Color.yellow;
            }
        }
    }

    // Use this for initialization
    void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
       
    }
}
