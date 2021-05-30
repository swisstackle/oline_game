using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FileManager : MonoBehaviour
{
    // Start is called before the first frame update

    
    
    public UnityEngine.UI.Button startButton;
    /*
     * <summary>
     * UI Manager.
     * </summary>
     */
    private UIManager uimanager;
    public Text debugtext; 
    public Text debugtext2;

    /*
     * <summary>
     * Path to JSON file
     * </summary>
     */
    private string path;

    [SerializeField] private GameObject mainCam;

    /*
     * <summary>
     * Buttons to select football position
     * </summary>
     */
    [SerializeField] private Button ltbutton;
    [SerializeField] private Button lgbutton;
    [SerializeField] private Button cbutton;
    [SerializeField] private Button rgbutton;
    [SerializeField] private Button rtbutton;

    /*
     * <summary>
     * Camera position for ingame
     * </summary>
     */
    private Vector3 camPosition;


    /*
     * <summary>
     * Initializing variables
     * </summary>
     */
    void Start()
    {
        camPosition = new Vector3();
        path = Application.persistentDataPath;
        startButton.gameObject.SetActive(false);
        uimanager = new UIManager();
        
        debugtext.verticalOverflow = VerticalWrapMode.Overflow;
        debugtext.text=Application.persistentDataPath;
        ShowFiles();

        ltbutton.onClick.AddListener(() => setCamPosition("lt"));
        lgbutton.onClick.AddListener(() => setCamPosition("lg"));
        cbutton.onClick.AddListener(() => setCamPosition("c"));
        rgbutton.onClick.AddListener(() => setCamPosition("rg"));
        rtbutton.onClick.AddListener(() => setCamPosition("rt"));




    }


    // Update is called once per frame
    void Update()
    {
        
    }

    /*
     * <summary>
     * Sets value of Vector for Camera according to Football Position selected
     * </summary>
     */
    void setCamPosition(string footballposition)
    {
        switch (footballposition)
        {
            case "lt": camPosition = new Vector3(0, 0, 4); break;
            case "lg": camPosition = new Vector3(0, 0, 2); break;
            case "c":  camPosition = new Vector3(0, 0, 0); break;
            case "rg": camPosition = new Vector3(0, 0, -2); break;
            case "rt": camPosition = new Vector3(0, 0, -4); break;

            default:
                break;
        }

    }


    /*
     * <summary>
     * Displays all files in Scrollview
     * </summary>
     */
    void ShowFiles()
    {
        string[] files = Directory.GetFiles(path);
        debugtext.text = files[0];
        debugtext2.text = files.Length.ToString();
        
        foreach (string file in files)
        {
            string filename = Path.GetFileName(file);

            debugtext.text = filename;
            var newButton = uimanager.CreateButton(startButton, filename, filename);
            
            newButton.GetComponent<Button>().onClick.AddListener(()=>Load(file));


        }
    }
    /*
     * <summary>
     * Loads, puts JSON and cam position into the global variables (so that the 
     * </summary>
     */
    private void Load(string filepath)
    {


        GameObject obj = new GameObject();
        Text t = obj.AddComponent<Text>();
        t.text = File.ReadAllText(filepath);

        GameObject obj2 = new GameObject();
        obj2.transform.position = camPosition;
        

        GlobalVariables.json = obj;
        GlobalVariables.camPosition = obj2;
        
        DontDestroyOnLoad(GlobalVariables.json);
        DontDestroyOnLoad(GlobalVariables.camPosition);

        SceneManager.LoadScene(sceneName: "Game");
    }
    void Cancel()
    {

    }
}
