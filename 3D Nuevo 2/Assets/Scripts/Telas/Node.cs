using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node {

    public Vector3 pos;
    public Vector3 vel;
    public Vector3 force;
    public float mass;
    public bool isFixed;
    public Vector3 fixPos;
    MassSpring myMassSpring;
    public List<MassSpring.Triangle> triangles;

    //Constructor
    public Node(Vector3 p,float m, MassSpring ms){
        pos = p;
        vel = Vector3.zero;
        mass = m;
        myMassSpring = ms;
        triangles = new List<MassSpring.Triangle>();
    }
    

    //Transform myMassSpring to local coords pos = that
    public void SetGlobalCoords(Vector3 p){
        pos = myMassSpring.transform.InverseTransformPoint(p);
    }


    //get myMassSpring in global coords
      public Vector3 GetGlobalCoords()
    {
        return myMassSpring.transform.TransformPoint(pos);
    }
	
    //f = ma
    public void ComputeForces(float m, float d, bool wind, bool damping)
    {
        //force += mass * gravity
        force += m * myMassSpring.Gravity;
        //damping
        if(damping) {
            force -= d * vel;
        }

        

       if(triangles != null && wind){
           //foreach triangle apply formula
            foreach(MassSpring.Triangle triangle in triangles){
                
                Vector3 n = Vector3.Cross(triangle.nodes[1].pos -triangle.nodes[0].pos, triangle.nodes[2].pos - triangle.nodes[0].pos);
                n.Normalize();
                
                Vector3 triangleVelocity = triangle.nodes[0].vel + triangle.nodes[1].vel + triangle.nodes[2].vel;
                triangleVelocity /= 3;

                Vector3 windForce = GetTriangleArea(triangle.nodes[0].pos, triangle.nodes[1].pos, triangle.nodes[2].pos) * (Vector3.Dot(n, myMassSpring.GetCurrentWind() - triangleVelocity)) * n;
                windForce /=triangles.Count;
                force += windForce;
             }
        }
    }

    float GetTriangleArea(Vector3 nodeA, Vector3 nodeB, Vector3 nodeC){
            
            //heron triangle area formula
            float a = (nodeB - nodeA).magnitude;
            float b = (nodeC - nodeB).magnitude;
            float c = (nodeA - nodeC).magnitude;

            float s = 0.5f*(a + b + c);

            return s;


    }
}
