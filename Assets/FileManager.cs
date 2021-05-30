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
    private UIManager uimanager;
    public Text debugtext;
    public Text debugtext2;

    private string path;


    void Start()
    {
        path = Application.persistentDataPath;
        startButton.gameObject.SetActive(false);
        uimanager = new UIManager();
        
        debugtext.verticalOverflow = VerticalWrapMode.Overflow;
        debugtext.text=Application.persistentDataPath;
        ShowFiles();

       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

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

    private void Load(string filepath)
    {


        GameObject obj = new GameObject();
        Text t = obj.AddComponent<Text>();
        t.text = File.ReadAllText(filepath);
        

        GlobalVariables.json = obj;
        DontDestroyOnLoad(GlobalVariables.json);

        SceneManager.LoadScene(sceneName: "Game");
    }
    void Cancel()
    {

    }
}
