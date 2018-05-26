using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Running : MonoBehaviour {

	public Animator animator;
	public bool isRunning;

	// Use this for initialization
	void Start () {
		animator = this.gameObject.GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
