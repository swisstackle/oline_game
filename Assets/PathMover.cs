using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class PathMover : MonoBehaviour
{
    // Start is called before the first frame update

    private GameObject obj;
    private Draw draw;
    private List<GameObject> lineObjects;

    private NavMeshAgent nav;
    /*
     * <summary>
     * Queue is used so that we cna deque each time the player needs a new position/moves.
     * </summary>
     */
    private Queue<Vector3> pathPoints;
    public Text debugtext;
    public Text debugtext2;

    /*
     * <summary>
     * Initializing
     * </summary>
     */
    void Awake()
    {
        
        pathPoints = new Queue<Vector3>();
        nav = GetComponent<NavMeshAgent>();
        obj = GameObject.Find("Draw");
        draw = obj.GetComponent<Draw>();
        lineObjects = draw.getLineObjects();

    }

    /*
     * <summary>
     * Sets the path for the player
     * </summary>
     */
    public void setPoints(Vector3[] points)
    {
        pathPoints = new Queue<Vector3>(points);
        
    }

    // Update is called once per frame
    void Update()
    {

        if (ShouldSetDestination())
        {
            UpdatePathing();
        }

    }
    /*
     * <summary>
     * Sets the next destination for the navmeshagent, so that the player can move there
     * </summary>
     */
    void UpdatePathing()
    {
        Vector3 dest = pathPoints.Dequeue();
            nav.SetDestination(dest);
        

    }
    /*
     * <summary>
     * Checks whether we should actually set the destination.
     * </summary>
     */
    private bool ShouldSetDestination()
    {
        

        
        if (pathPoints.Count < 1)
        {
   
            return false;
        }
        
        
        if (!nav.hasPath || nav.remainingDistance < 0.01f)
        {
            //debugtext2.text = debugtext2.text + "good";
            return true;
        }
        return false;
    }
}
