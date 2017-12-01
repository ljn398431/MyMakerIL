using UnityEngine;
using System.Collections;
using System;
using System.Text;

public class LogTest : MonoBehaviour
{
    // public Renderer render;
    public Material mat;
    int PowerupCount = 23;
    private testColor _testColor = testColor.red;

   
    void Awake()
    {
        for (int i = 0; i <= 5; i++)
        {
            // Registration Log.
            Debug.Log(10 % 2.1153f);
            UnityEngine.Debug.Log("1111111111");

            Debug.Log("suifeng", Color.cyan);
            Debug.Log("1238908", "test", Color.red);
            Debug.Log("1238908", "ljn", Color.red);
            //Debug.Log(colorToHex(Color.red));
            Debug.Log("PowerupCount " + PowerupCount);

            // Registration Group Log.
            Debug.Log("Test 1324567980123456789", "LogTest1");
            Debug.Log("test", "LogTest2");
            Debug.Log("PowerupCount " + PowerupCount, "LogTest2");

            // Registration LogWarning. 
            Debug.LogWarning("test", "LogTest3");

            // Registration LogError.
            Debug.LogError("Hello ", "LogTest4");
        }
        // Watcher in the window register value of the variable.
        Debug.Watcher("PowerupCount", PowerupCount);

        // ShowFunc Command Register.
        DebugConsole.RegisterCommand("ShowFunc", ShowFuncCommand);
    }
    enum testColor
    {
        red,
        blue,
        white,
        green,
    }

    void Update()
    {
        PowerupCount++;
        for (int i = 0; i < 5; i++)
        {
            // Watcher in the window register value of the variable
            Debug.Watcher("PowerupCount " + i, PowerupCount);
        }
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            if ((int)_testColor > 3)
            {
                _testColor = testColor.red;
            }
            else
            {
                int a = (int)_testColor + 1;
                _testColor = (testColor)a;
            }
            switch (_testColor)
            {
                case testColor.red:
                    mat.SetColor("_EmissionColor", Color.red);
                    Camera.main.backgroundColor = Color.red;
                    break;
                case testColor.blue:
                    mat.SetColor("_EmissionColor", Color.blue);
                    Camera.main.backgroundColor = Color.blue;
                    break;
                case testColor.white:
                    mat.SetColor("_EmissionColor", Color.white);
                    Camera.main.backgroundColor = Color.white;
                    break;
                case testColor.green:
                    mat.SetColor("_EmissionColor", Color.green);
                    Camera.main.backgroundColor = Color.green;
                    break;
                default:
                    break;
            }
        }

    }

    // Function to be called by the command
    public object ShowFuncCommand(params string[] args)
    {
        return ("PowerupCount " + PowerupCount);
    }

    // Function to be called by the command
    public void ShowDate()
    {
        Debug.LogCommand(DateTime.Now);
    }

}
