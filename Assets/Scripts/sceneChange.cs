using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class sceneChange : MonoBehaviour
{
    // Start is called before the first frame update
    public void menu()
    {
        SceneManager.LoadScene(0);
    }
    public void grafuri()
    {
        SceneManager.LoadScene(1);
    }
    public void DFS()
    {
        SceneManager.LoadScene(2);
    }

    public void BFS()
    {
        SceneManager.LoadScene(3);
    }

    public void Sortari()
    {
        SceneManager.LoadScene(4);
    }

    public void SelectSort()
    {
        SceneManager.LoadScene(5);
    }

    public void bubbleSort()
    {
        SceneManager.LoadScene(6);
    }

    public void Exit()
    {
        Application.Quit();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) menu();
    }
}
