using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Edge
{

    //VertexA and Vertex B must be ordered, Vertex B too, vertex O is useful for tension springs
        public int VertexA;
        public int VertexB;
        public int VertexO;

        public Edge(int a, int b, int o){
            VertexA = a;
            VertexB = b;
            VertexO = o;

        }
    
}
