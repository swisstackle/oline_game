using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.UI;
using System.IO;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

public class Draw : MonoBehaviour
{
    
    public OVRInput.Controller controller;

    public GameObject thePathMover;
    private LineRenderer lineRenderer;
    public GameObject linePrefab;
    private GameObject currentLine;
    public Material red;
    public Button jsonButton;
    public GameObject parentOfLines;

    private List<GameObject> lineObjects;

    public Text text;
    public Text text2;
    public Text text3;
    public Text text4;
    private Vector3[][] lines;

   

    private List<GameObject> pathMovers;
    //public Button backButton;


    private void Awake()
    {
        pathMovers = new List<GameObject>();


        lineObjects = new List<GameObject>();


        linePrefab.gameObject.SetActive(false);
        GameObject json = GlobalVariables.json;
        string jsonString = json.GetComponent<Text>().text;
        var settings = new Newtonsoft.Json.JsonSerializerSettings();
        // This tells your serializer that multiple references are okay.
        settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        lines = JsonConvert.DeserializeObject<Vector3[][]>(jsonString, settings);
        //backButton.GetComponent<Button>().onClick.AddListener(back);
        CreateLines();
    }
    void Start()
    {
        CreatePathMovers();
    }

    void back()
    {
        SceneManager.LoadScene(sceneName: "Select");
    }
    public List<GameObject> getLineObjects()
    {
        return this.lineObjects;
    }


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

                    lines[i][j].x = tempY;
                    lines[i][j].z = tempX;
                    lines[i][j].y = 0;

                    lines[i][j].x = lines[i][j].x * 0.1f; // because scalar is 10 for the plane object
                    lines[i][j].y = lines[i][j].y * 0.1f;
                    lines[i][j].z = lines[i][j].z * 0.1f;

                    lineRenderer.positionCount++;

                    lineRenderer.SetPosition(lineRenderer.positionCount - 1, lines[i][j]);

                }
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

    void CreatePathMovers()
    {
        try
        {
            foreach (var lineObject in this.lineObjects)
            {
                LineRenderer renderer = lineObject.GetComponent<LineRenderer>();
                int posCount = renderer.positionCount;
                Vector3[] positions = new Vector3[posCount];

                renderer.GetPositions(positions);
                for (int i = 0; i < positions.Length; i++)
                {
                   positions[i] =  Vector3.Scale(positions[i], new Vector3(10, 10, 10));
                   positions[i].y = 1;
                }

                

                GameObject mover = Instantiate(thePathMover, positions[0], Quaternion.identity);
                
                
                mover.SetActive(true);

                PathMover pathMover = mover.GetComponent<PathMover>();
                pathMover.debugtext = text3;
                pathMover.debugtext2 = text4;
                
                pathMover.setPoints(positions);
            }
            
        }
        catch(System.Exception e)
        {
            text.text = e.ToString();
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.Get(OVRInput.Button.Two))
        {
            back();
        }
    }

    void printJson()
    {

    }
}

