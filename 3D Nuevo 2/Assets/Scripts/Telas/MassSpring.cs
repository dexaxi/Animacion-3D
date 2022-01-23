using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
/// <summary>
/// Sample code for accessing MeshFilter data.
/// </summary>
public class MassSpring : MonoBehaviour
{
    /// <summary>
    /// Default constructor. Zero all. 
    /// </summary>
    public MassSpring()
    {
        this.paused = true;
		this.TimeStep = 0.01f;
		this.Gravity = new Vector3 (0.0f, -9.81f, 0.0f);
		this.IntegrationMethod = Integration.Symplectic;
    }

    
    /// <summary>
	/// Integration method.
	/// </summary>
	public enum Integration
	{
		Explicit = 0,
		Symplectic = 1,
        
	};
    public struct Triangle{
        public Node[] nodes;

        public Triangle(Node[] nodes){
            this.nodes = nodes; 
        }

    }

    #region InEditorVariables
    [Header("Simulation Settings")]
    [SerializeField] public bool paused;
    [SerializeField] public bool damping;
    [SerializeField] public bool wind;
    [SerializeField] private Integration IntegrationMethod;
    [SerializeField]   [Range(1f, 35f)] public int subSteps;

    [Header("Physics Attributes")]
    [SerializeField]  [Range(0.01f, 10f)]  private float nodeMass;
    [SerializeField] [Range(0.01f, 10f)]  private float nodeDamping;
    [SerializeField] [Range(0.01f, 1000f)]  private float tractionStiffness;
    [SerializeField] [Range(0.01f, 1000f)]  private float tensionStiffness;
    [SerializeField] [Range(0.01f, 50f)]  private float springDamping;
	[SerializeField]  private float TimeStep;
    [SerializeField]  public Vector3 Gravity;
    [SerializeField] [Range(0.01f, 5f)]public float windChange;
    [SerializeField]  public Vector3 windVelocity;



    [Header("Scene Components")]
    [SerializeField] private Fixer[] fixers;

    #endregion

    #region OtherVariables
    private List<Node> nodes;
    private List<Spring> tractionSprings;
    private List<Spring> tensionSprings;
    private List<Spring> removedSprings;
    private List<Edge> allEdges;
    private Mesh mesh;
    private FixerManager myFixerManager;
    private float currentWindTime = 0;
    #endregion

    #region MonoBehaviour

    private void Start()
    {

        //mesh = mymesh
        mesh = this.GetComponent<MeshFilter>().mesh;
        //VertexArray & triangleArray
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;
        //FindAllFixers
        myFixerManager = GetComponentInParent<FixerManager>();
        //InitializeStuff
        InitializeVertices(vertices);
        InitializeSprings(triangles);  
        InitializeTriangles(triangles);    

    }

    void InitializeVertices(Vector3[] vertices){
        //New node list
        nodes = new List<Node>();
        for (int i = 0; i < vertices.Length; i++){
        
        //Initialize each vertex
        Vector3 vertexPos = transform.InverseTransformPoint(vertices[i]);
        nodes.Add(new Node(vertexPos, nodeMass, this));
        
        //Fix first vertex (Useful for quads)
        //if (i == 1) nodes[1].isFixed = true;
            //FixerCode
            fixers = myFixerManager.fixersFromManager;
            if(fixers!=null){foreach (Fixer fixer in fixers)
                    {
                        //If there's a fixer in the bounding box then->
                        if (fixer.IsContained(transform.TransformPoint(vertices[i])))
                        {
                            //Fix respective node and add node to THAT fixer's list
                            nodes[i].isFixed = true;
                            fixer.AddNodeToFixerList(nodes[i]);
                        }
                    }
            }
        }

    }
    void InitializeSprings(int[] triangles){
        
        List<int> vertices= new List<int>();
        allEdges = new List<Edge>();
        tractionSprings = new List<Spring>();
        tensionSprings = new List<Spring>();

        for(int i = 0; i<triangles.Length; i+=3){
            

            int vertex1 = triangles[i];
            int vertex2 = triangles[i+1];
            int vertex3 = triangles[i+2];
           
           //All six combinations for each three vertices in the first two positions ordered, smallest index first always 
            if(vertex1<vertex2){
                allEdges.Add(new Edge(vertex1,vertex2,vertex3));
            } else{
                allEdges.Add(new Edge(vertex2,vertex1,vertex3));

            }
            if(vertex1<vertex3){
                allEdges.Add(new Edge(vertex1,vertex3,vertex2));
            } else{
                allEdges.Add(new Edge(vertex3,vertex1,vertex2));

            }
            if(vertex2<vertex3){
                allEdges.Add(new Edge(vertex2,vertex3,vertex1));
            } else{
                allEdges.Add(new Edge(vertex3,vertex2,vertex1));

            }

        }    
        

        //Initialize removed edges + springs lists
        List<Edge> removedEdges = new List<Edge>();
        removedSprings = new List<Spring>();


        //Sort edges on Vertex A then Vertex B using Linq
        List<Edge> edges = allEdges.OrderBy(x => x.VertexA).ThenBy(x=>x.VertexB).ToList();
        
       for(int r = 0; r < edges.Count-1 ; r++){
            if(edges[r].VertexB == edges[r+1].VertexB && edges[r].VertexA == edges[r+1].VertexA){
                //Add tension springs
                tensionSprings.Add(new Spring( nodes[edges[r].VertexO], nodes[edges[r+1].VertexO] , tensionStiffness));
            }
        
        }


        for(int j = 0; j < edges.Count-1 ; j++){
            
            if(edges[j].VertexB == edges[j+1].VertexB){
                //removed duplicates + add to removed edges list
                removedEdges.Add(edges[j]);
                edges.Remove(edges[j]);
            }
        }

        

        for (int k = 0; k < edges.Count; k++){
            //Debug Traction Springs
            //Debug.Log(edges[k].VertexA + " ," + edges[k].VertexB + ", " + edges[k].VertexO);
            tractionSprings.Add(new Spring(nodes[edges[k].VertexA], nodes[edges[k].VertexB], tractionStiffness));
        }


      /*for (int g = 0; g < removedEdges.Count; g++){
            //Debug Removed Edges
            Debug.Log(removedEdges[g].VertexA + " ," + removedEdges[g].VertexB + ", " + removedEdges[g].VertexO);
            
            //RemovedSprings Visual Aid
            removedSprings.Add(new Spring(nodes[removedEdges[g].VertexA], nodes[removedEdges[g].VertexB], tractionStiffness));

        }*/


        //Debugging Nonsense
        Debug.Log("Created " + mesh.vertices.Length + " Vertices.");
        Debug.Log("Created " + nodes.Count + " Nodes.");
        Debug.Log("There are " + triangles.Length + " triangles");
        Debug.Log("Created " + edges.Count + " Edges.");
        Debug.Log("Removed " + removedEdges.Count + " Edges.");
        Debug.Log("Created " + tractionSprings.Count + " Traction Springs.");
        Debug.Log("Created " + tensionSprings.Count + " Tension Springs.");
    }
    void InitializeTriangles(int[] triangles){


        for(int i = 0; i < triangles.Length / 3; i ++){
                
            Node[] triangleNodes = new Node[3];

            triangleNodes[0] = nodes[triangles[i*3]];
            triangleNodes[1] = nodes[triangles[i*3 + 1]];
            triangleNodes[2] = nodes[triangles[i*3 + 2]];

            Triangle triangle = new Triangle(triangleNodes);

            nodes[triangles[i*3]].triangles.Add(triangle);
            nodes[triangles[i*3+1]].triangles.Add(triangle);
            nodes[triangles[i*3+2]].triangles.Add(triangle);

        }
    }
  
    
    private void Update() {

        //Controls
        //PauseSimulation
        //if(Input.GetKeyDown(KeyCode.P))paused = !paused;
        //if(Input.GetKeyDown(KeyCode.D))damping = !damping;
        //if(Input.GetKeyDown(KeyCode.W))wind = !wind;
      //  if(Input.GetKeyDown(KeyCode.M) && subSteps < 35)subSteps++;
      //  if(Input.GetKeyDown(KeyCode.N) && subSteps > 1)subSteps--;
        


        //Visual Aid -> traction springs white, tension springs blue, duplicates red
        /*for (int i = 0; i < tractionSprings.Count; i++){
            Debug.DrawLine(tractionSprings[i].nodeA.pos, tractionSprings[i].nodeB.pos);
        }
        for (int i = 0; i < removedSprings.Count; i++){
            Debug.DrawLine(removedSprings[i].nodeA.pos, removedSprings[i].nodeB.pos, Color.red);
        }
        for (int i = 0; i < tensionSprings.Count; i++){
            Debug.DrawLine(tensionSprings[i].nodeA.pos, tensionSprings[i].nodeB.pos, Color.blue);
        }*/

    }
    public void FixedUpdate()
    {

        if (this.paused)
            return; // Not simulating
        
        //substeps
        for(int i = 0; i<subSteps; i++){
        
            //Compute Nodes + Spring Forces independently of integration method
            ComputeForces();

            // Select integration method
            switch (this.IntegrationMethod)
            {
                case Integration.Explicit: this.StepExplicit(); break;
                case Integration.Symplectic: this.StepSymplectic(); break;
                default:
                throw new System.Exception("[ERROR] Should never happen!");
            }

            //alters wind
            if(wind) currentWindTime += TimeStep * windChange;

            //Update spring lengths each FixedUpdate (just in case, could be done in Spring.ComputeForces)
            foreach (Spring spring in tractionSprings)
            {
                spring.UpdateLength();
            }
            foreach (Spring spring in tensionSprings)
            {
                spring.UpdateLength();
            }

            //Update vertex positions -> local to global
            UpdateVertices();

        }
       
    }

    private void ComputeForces(){

          foreach (Node node in nodes)
        {
            node.force = Vector3.zero;
            node.ComputeForces(nodeMass, nodeDamping, wind, damping);
        }

        foreach (Spring spring in tractionSprings)
        {
            spring.ComputeForces(tractionStiffness, springDamping, damping);
        }

        foreach (Spring spring in tensionSprings)
        {
            spring.ComputeForces(tensionStiffness, springDamping, damping);
        }

    }
     /// <summary>
    /// Performs a simulation step in 1D using Explicit integration.
    /// </summary>
    private void StepExplicit()
	{
       
        foreach (Node node in nodes)
        {
            if (!node.isFixed)
            {
                //Explicit Euler -> calculate velocity from position
                node.pos += (TimeStep/subSteps) * node.vel;
                node.vel += ((TimeStep/subSteps) / node.mass) * node.force;
            }
        }
	}

	/// <summary>
	/// Performs a simulation step in 1D using Symplectic integration.
	/// </summary>
	private void StepSymplectic()
	{    
        foreach (Node node in nodes)
        {
            if (!node.isFixed)
            {
                //Simplectic Euler -> Calculate position from velocity (More Stable!!!!)
                node.vel += ((TimeStep/subSteps) / node.mass) * node.force;
                node.pos +=  (TimeStep/subSteps) * node.vel;
            }
        }
       
    }

    private void UpdateVertices()
    {
        //Procedure to update vertex positions
        Vector3[] vertices = new Vector3[mesh.vertexCount];
        for (int i = 0; i < nodes.Count; i++)
        {

            //VertexArray = nodes.inLocalSpace 
            vertices[i] = transform.TransformPoint(nodes[i].pos);
         }
        //Mesh vertices = local coords
        mesh.vertices= vertices;
    }


    public Vector3 GetCurrentWind(){

        return transform.InverseTransformDirection(Mathf.Abs(Mathf.Sin(currentWindTime)) * windVelocity);

    }

    #endregion


}
