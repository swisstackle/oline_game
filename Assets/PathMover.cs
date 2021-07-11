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

    private Animator anim;
    private CharacterController cc;
    public bool paused;

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
        anim = GetComponent<Animator>();
        nav.updatePosition = false;
        cc = GetComponent<CharacterController>();
        paused = false;
    }
    void start()
    {
        transform.eulerAngles = new Vector3(0, -90, 0);
        Vector3 movedir = transform.TransformDirection(new Vector3(0, -90, 0));
        cc.Move(movedir * Time.deltaTime);
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
        if (paused)
        {
            anim.SetBool("move", false);
        }
        else
        {
            Vector3 dest = pathPoints.Dequeue();
            nav.SetDestination(dest);
            Vector3 worldDeltaPosition = dest - transform.position;

            Vector3 groundDeltaPosition = Vector3.zero;

            groundDeltaPosition.x = Vector3.Dot(transform.right, worldDeltaPosition);
            groundDeltaPosition.y = Vector3.Dot(transform.forward, worldDeltaPosition);

            Vector3 velocity = Vector3.zero;

            if (Time.deltaTime > 1e-5f)
            {
                velocity = groundDeltaPosition / Time.deltaTime;
            }

            bool shouldMove = velocity.magnitude > 0.025f && nav.remainingDistance > nav.radius;

            anim.SetBool("move", true);
            anim.SetFloat("velx", velocity.x);
            anim.SetFloat("vely", velocity.y);
        }
        

    }
    private void OnAnimatorMove()
    {
        transform.position = nav.nextPosition;
        
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
