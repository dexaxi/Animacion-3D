using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spring {

    //Adjacent nodes
    public Node nodeA, nodeB;
    //Starting length
    public float L0;
    //Currentlength
    public float L;
    //SpringStiffness
    public float stiffness;


    //Constructor
    public Spring(Node a, Node b, float s){
        nodeA = a;
        nodeB = b;
        stiffness = s;
        L = GetAdjacentNodeDistance();
        L0 = L;
    }


    //Hooke's law, add forces to one node, add opposing forces to the adjacent
    public void ComputeForces(float s, float d, bool damping)
    {
        Vector3 u = GetAdjacentNodeSub();
        u.Normalize();

        //hooke
        Vector3 force = - s * (L - L0) * u;
        //dampingforce
        if(damping){
            force -= d* GetAdjacentNodeVelDif();
            force -= d * Vector3.Dot(u,GetAdjacentNodeVelDif()) * u;
        }
        //add forces
        nodeA.force += force;
        nodeB.force -= force;
    }

   public void UpdateLength(){

       L = GetAdjacentNodeDistance();

   }
    float GetAdjacentNodeDistance(){
        //Length
        return(GetAdjacentNodeSub()).magnitude;
    }

    Vector3 GetAdjacentNodeSub(){
        return (nodeA.pos - nodeB.pos);
    }

     Vector3 GetAdjacentNodeVelDif(){
        return (nodeA.vel - nodeB.vel);
    }
}
