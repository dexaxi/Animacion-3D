using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fixer : MonoBehaviour {

	// Possibilities of the Fixer
    public List<Node> nodes;
    private Bounds bounds;
	private void Awake () {
       nodes = new List<Node>();
       bounds = GetComponent<Collider>().bounds;
    }
    
    private void FixedUpdate() {
        
        //foreach node in fixernodes transform to globalcoords the nodes fixerposition
        foreach (Node n in nodes){
        Vector3 globalPos = transform.TransformPoint(n.fixPos);
        n.SetGlobalCoords(globalPos);
        }
    
    }


    //Add nodes in global coords
    public void AddNodeToFixerList(Node n){
        if(n != null && nodes != null) {
            nodes.Add(n);
            n.fixPos = transform.InverseTransformPoint(n.GetGlobalCoords());
        }
    }

    //bound.contains
    public bool IsContained(Vector3 p){
        return bounds.Contains(p);
    }
}
