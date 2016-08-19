using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChangeShape : MonoBehaviour {

    public int CurrentShape;
    private List<Material> materials = new List<Material>();

	// Use this for initialization
	void Start () {

        foreach (var item in Resources.LoadAll("Shapes")) materials.Add((Material)item);

        transform.GetComponent<Renderer>().material = materials[CurrentShape];
	}
	
	// Update is called once per frame
	void Update () {

        if (Input.GetButtonDown("ChangeShape"))
        {
            CurrentShape++;
            if (CurrentShape > materials.Count - 1) CurrentShape = 0;
            transform.GetComponent<Renderer>().material = materials[CurrentShape];

        }
	
	}
}
