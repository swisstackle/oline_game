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
    private Queue<Vector3> pathPoints;
    public Text debugtext;
    public Text debugtext2;
    void Awake()
    {
        
        pathPoints = new Queue<Vector3>();
        nav = GetComponent<NavMeshAgent>();
        obj = GameObject.Find("Draw");
        draw = obj.GetComponent<Draw>();
        lineObjects = draw.getLineObjects();

    }
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

    void UpdatePathing()
    {
        Vector3 dest = pathPoints.Dequeue();
            nav.SetDestination(dest);
        

    }
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
