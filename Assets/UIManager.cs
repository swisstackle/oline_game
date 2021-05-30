using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public UnityEngine.UI.Button startButton;

    public string test;
    // Start is called before the first frame update
    void Start()
    {
        startButton.gameObject.SetActive(false);
        test = "whatsup";
    }


    public GameObject CreateButton(Button template, string name, string _text = "default text")
    {
        var dynamicButton = Instantiate<GameObject>(template.gameObject, template.transform.parent);
        dynamicButton.SetActive(true);
        var text = dynamicButton.GetComponentInChildren<Text>();
        text.text = _text;
        return dynamicButton;
    }
    
}
