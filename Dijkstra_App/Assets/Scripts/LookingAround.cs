using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookingAround : MonoBehaviour {

	public Animator animator;
	public bool isLookingAround;

	// Use this for initialization
	void Start () {
		animator = this.GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
