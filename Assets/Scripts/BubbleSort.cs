using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class BubbleSort : MonoBehaviour
{
    Camera cam;
    public bool running, typing, continueNext, swapped;
    public int index1, index2;
    private SpriteRenderer spriteRenderer;
    private ElementIndex elementIndex;
    private TextMeshPro textMesh;
    private TMP_InputField inputField1, inputField2;
    RaycastHit2D hit;
    public int[] v;
    public GameObject highlighted;
    public GameObject[] fields;
    public GameObject text;
    void Start()
    {
        running = false;
        typing = false;
        v = new int[10];
        cam = Camera.main;
        continueNext = false;
    }

    void UpdateText()
    {
        TMP_InputField inputField;
        int result;
        for (int i = 0; i < 10; i++)
        {
            inputField = fields[i].GetComponent<TMP_InputField>();
            if (inputField.text != null)
            {
                int.TryParse(inputField.text, out result);
                v[i] = result;
                //Debug.Log(result);
            }
        }
    }

    void SwapTexts(TMP_InputField inputField1, TMP_InputField inputField2)
    {
        string aux;
        aux = inputField1.text;
        inputField1.text = inputField2.text;
        inputField2.text = aux;
    }

    void Run()
    {
        running = true;
        swapped = false;
        spriteRenderer.color = new Color(0f, 0f, 0f);
        TMP_InputField inputField;
        for (int i = 0; i < 10; i++)
        {
            inputField = fields[i].GetComponent<TMP_InputField>();
            inputField.interactable = false;
        }
        index1 = 0;
    }

    void Reset()
    {
        TMP_InputField inputField;
        for (int i = 0; i < 10; i++)
        {
            inputField = fields[i].GetComponent<TMP_InputField>();
            if (inputField.text != null)
                inputField.text = "";
        }
        for (int i = 0; i < 10; i++)
        {
            inputField = fields[i].GetComponent<TMP_InputField>();
            inputField.interactable = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) SceneManager.LoadScene(4);
        UpdateText();
        if (!running)
        {
            if (Input.GetKeyDown(KeyCode.Backspace)) Reset();
            if (Input.GetKeyDown(KeyCode.Space)) Run();
            if (Input.GetMouseButtonDown(0))
            {
                hit = Physics2D.GetRayIntersection(cam.ScreenPointToRay(Input.mousePosition)); // check for 2d collision
                if (hit.collider != null)
                {
                    if (highlighted != null)
                    {
                        spriteRenderer = highlighted.GetComponent<SpriteRenderer>();
                        spriteRenderer.color = new Color(0f, 0f, 0f);
                    }
                    spriteRenderer = hit.collider.gameObject.GetComponent<SpriteRenderer>();
                    spriteRenderer.color = new Color(1f, 0.71f, 0f);
                    highlighted = hit.collider.gameObject;
                    elementIndex = hit.collider.gameObject.GetComponent<ElementIndex>();
                }
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.RightArrow)) continueNext = true;

            if (continueNext)
            {
                inputField1 = fields[index1].GetComponent<TMP_InputField>();
                inputField2 = fields[index1+1].GetComponent<TMP_InputField>();
                if (v[index1] > v[index1+1]) { SwapTexts(inputField1, inputField2); swapped = true; }
                index1++;
                if (index1 == 9)
                {
                    index1 = 0;
                    running = swapped;
                    swapped = false;
                }
                continueNext = false;
            }
        }
    }
}
