using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.UI;
using System.IO;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;
using UnityEngine.AI;
using System.Linq;

public class Draw : MonoBehaviour
{
    
    public OVRInput.Controller controller;

    [SerializeField] private GameObject thePathMover; //prefab for player
    private LineRenderer lineRenderer; //linerenderer for creatline() function
    [SerializeField] private GameObject linePrefab; // the prefab of a playerpath/line
    private GameObject currentLine; // the currentline getting created whenever creatline() is called
    [SerializeField] private GameObject parentOfLines;

    private List<GameObject> lineObjects; //current list of paths/lines

    [SerializeField] private Text text; //debugpurposes
    [SerializeField] private Text text2;//debugpurposes
    [SerializeField] private Text text3;//debugpurposes
    [SerializeField] private Text text4;//debugpurposes

    [SerializeField] private GameObject olinePrefab;
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button resetButton;
    private Vector3[][] lines; // current positions of all paths/lines

    private bool started;
    private bool paused;

    private Vector3 cameraPos; //the vr camera rig position. Has to be placed a
                               //ccording to what offensive line positions has been selected.

    [SerializeField] private GameObject maincam; //the vr camera rig object

    private List<GameObject> pathMovers; // the list of playerobjects (defensive and offensive players)

    /*
     * <summary>
     * Initializing JSON and Lists
     * </summary>
     */
    private void Awake()
    {
        pathMovers = new List<GameObject>();


        lineObjects = new List<GameObject>();


        linePrefab.gameObject.SetActive(false);
        GameObject json = GlobalVariables.json; //The json for the player paths are saved in a static class named GlobalVariables. All the GameObjects in there are marked with DontDestroyOnLoad() so that they are still available after the transition from the Menu-Scene
        

        string jsonString = json.GetComponent<Text>().text;
        var settings = new Newtonsoft.Json.JsonSerializerSettings();
        // This tells your serializer that multiple references are okay.
        settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        lines = JsonConvert.DeserializeObject<Vector3[][]>(jsonString, settings);
        //backButton.GetComponent<Button>().onClick.AddListener(back);

        CreateLines();
        started = false;
        paused = false;
    }
    /*
     * <summary>
     * Sets position of Camera according to Football Position selected
     * </summary>
     */
    void Start()
    {
        text4.text = GlobalVariables.camPosition.transform.position.ToString(); 
        maincam.transform.position = GlobalVariables.camPosition.transform.position;
        text4.text = text4.text + " " + maincam.transform.position;
        pauseButton.onClick.AddListener(pauseGame);
        resetButton.onClick.AddListener(reset);
        placeOline();
        CreatePathMovers();
        StartCoroutine(CountdownToStart());
    }

    /*
    * <summary>
    * Timer so that the play starts after 5 seconds
    * </summary>
    */
    IEnumerator CountdownToStart()
    {
        int time = 5;
        while(time > 0)
        {
            text4.text = time.ToString();
            yield return new WaitForSeconds(1f);
            time--;
        }
        text4.text = "0";
        movePathMovers();
        started = true;
    }

    /*
     * <summary>
     * Function to get Back to the Menu Scene
     * </summary>
     */
    void back()
    {
        SceneManager.LoadScene(sceneName: "Select");
    }
    /*
     * <returns>
     * All Path/Line objects currently in the lists
     * </returns>
     */
    public List<GameObject> getLineObjects()
    {
        return this.lineObjects;
    }



    void placeOline()
    {

        GameObject lt = Instantiate(olinePrefab, new Vector3(0, 0, 2), Quaternion.identity);
        GameObject lg = Instantiate(olinePrefab, new Vector3(0, 0, 0), Quaternion.identity);
        GameObject c = Instantiate(olinePrefab, new Vector3(0, 0, -2), Quaternion.identity);
        GameObject rg = Instantiate(olinePrefab, new Vector3(0, 0, -4), Quaternion.identity);
        GameObject rt = Instantiate(olinePrefab, new Vector3(0, 0, -6), Quaternion.identity);

        lt.SetActive(true);
        lg.SetActive(true);

        c.SetActive(true);

        rg.SetActive(true);

        rt.SetActive(true);

    }

    /*
     * <summary>
     * Creates all paths for the players in the 3D world. Had to make some translations because the vectors in the 2D-Editor are alligned differently
     * </summary>
     */
    void CreateLines()
    {
        try
        {
            for (int i = 0; i < lines.GetLength(0); i++)
            {

                currentLine = Instantiate(linePrefab, Vector3.zero, Quaternion.identity, parentOfLines.transform);

                lineRenderer = currentLine.GetComponent<LineRenderer>();

                lineRenderer.positionCount = 0;
                int innerLength = lines[i].Length;

                for (int j = 0; j < innerLength; j++)
                {

                    var tempX = lines[i][j].x;
                    var tempY = lines[i][j].y;
                    var tempZ = lines[i][j].z;
                    //z = sin(theta), 
                    
                    lines[i][j].x = tempY;
                    lines[i][j].z = tempX;
                    lines[i][j].y = 0;

                    lines[i][j].x = lines[i][j].x * 0.1f; // because scalar is 10 for the plane object
                    lines[i][j].y = lines[i][j].y * 0.1f;
                    lines[i][j].z = lines[i][j].z * 0.1f;

                    lineRenderer.positionCount++;

                    lineRenderer.SetPosition(lineRenderer.positionCount - 1, lines[i][j]);

                }
                currentLine.GetComponent<LineRenderer>().Simplify(0.01f);
                currentLine.SetActive(true);
                lineObjects.Add(currentLine);
            }

        }

        catch (System.Exception e)
        {
            text.verticalOverflow = VerticalWrapMode.Overflow;
            text.text = e.ToString();
        }
        

    }

    List<Vector3[]> getPositions()
    {
        List<Vector3[]> linePositions = new List<Vector3[]>();
        foreach (var lineObject in this.lineObjects)
        {
            LineRenderer renderer = lineObject.GetComponent<LineRenderer>();
            int posCount = renderer.positionCount;
            Vector3[] positions = new Vector3[posCount];

            renderer.GetPositions(positions);
            for (int i = 0; i < positions.Length; i++)
            {
                positions[i] = Vector3.Scale(positions[i], new Vector3(10, 10, 10));
                positions[i].y = 1;
            }
            linePositions.Add(positions);

        }
        return linePositions;
    }
    /*
     * <summary>
     * Setting up a player at the start of each line.
     * </summary>
     */
    void CreatePathMovers()
    {
        List<Vector3[]> linePositions = getPositions();
        foreach (Vector3[] posis in linePositions)
        {
            GameObject mover = Instantiate(thePathMover, posis[0], Quaternion.identity);
            mover.SetActive(true);
            pathMovers.Add(mover);
        }
    }
    void movePathMovers()
    {
        int i = 0;
        List<Vector3[]> posis = getPositions();
        try
        {
            foreach (var mover in pathMovers)
            {
                mover.SetActive(true);
                PathMover pathMover = mover.GetComponent<PathMover>();
                pathMover.debugtext = text3;
                pathMover.debugtext2 = text4;
                
                pathMover.setPoints(posis.ElementAt(i));
                i++;
            }

        }
        catch (System.Exception e)
        {
            text.text = e.ToString();
        }

    }
   void pauseGame()
    {
        if (paused)
        {
            paused = false;
            foreach (var mover in pathMovers)
            {
                PathMover pathMover = mover.GetComponent<PathMover>();
                pathMover.paused = false;
            }
        }
        else
        {
            paused = true;
            foreach (var mover in pathMovers)
            {
                PathMover pathMover = mover.GetComponent<PathMover>();
                pathMover.paused = true;
            }
        }
       
        
    }
    void reset()
    {
        SceneManager.LoadScene(sceneName: "Game");
    }
   
    /*
     * <summary>
     * Checks for B button on Oculus Controller so that player can go back to Menu.
     * </summary>
     */
    void Update()
    {
       
        if (OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, controller) > 0.1f)
        {
            back();
        }
       

    }
}

