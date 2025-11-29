using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;

public class bfs : MonoBehaviour
{
    Camera cam;
    public Canvas canvas;
    Vector3 origin, difference;
    RaycastHit2D hit1, hit2;
    public int mode;
    public float multiplier, sizeMin, sizeMax;
    public bool help, creating, choosing, continueNext, chosen;
    public GameObject node, edge, line, aNode;
    LineRenderer lineRenderer;
    indexScript script1, script2;
    private SpriteRenderer spriteRenderer;
    private int[] v = new int[100];
    private SortedSet<int> indexSet;
    private Queue<Vector2> Queue;
    public GameObject[] nodes;
    private GameObject[,] lines;
    void Start()
    {
        cam = Camera.main;
        nodes = new GameObject[100];
        indexSet = new SortedSet<int>();
        lines = new GameObject[100,100];
        Queue = new Queue<Vector2>();
        creating = false;
        help = false;
        canvas.enabled = false;
        sizeMin = 1f;
        sizeMax = 20f;
        for (int i = 0; i < 100; i++) indexSet.Add(i);
    }

    void RemoveEdge(GameObject line)
    {
        Destroy(line);
    }

    void RemoveNode(GameObject Node)
    {
        script2 = Node.GetComponent<indexScript>();
        for (int i = 0; i <= 99; i++)
            if (lines[script2.index, i] != null) Destroy(lines[script2.index, i]);
        indexSet.Add(script2.index);
        Destroy(Node);
    }

    void Reset()
    {
        for (int i = 0; i < 100; i++)
            Destroy(nodes[i]);
        for (int i = 0; i < 100; i++)
            for (int j = 0; j < 100; j++)
                Destroy(lines[i, j]);
        choosing = false;

    }

    void ResetColor()
    {
        for (int i = 0; i < 100; i++)
            if(nodes[i]!=null)
            {
                spriteRenderer = nodes[i].GetComponent<SpriteRenderer>();
                spriteRenderer.color = new Color(1f, 1f, 1f);
            }
        for (int i = 0; i < 100; i++)
            for (int j = 0; j < 100; j++)
                if (lines[i, j] != null)
                {
                    lineRenderer = lines[i, j].GetComponent<LineRenderer>();
                    lineRenderer.startColor = new Color(0.41f, 0f, 0.12f);
                    lineRenderer.endColor = new Color(0.61f, 0.05f, 0.33f); 
                }
    }

    void StartButton()
    {
        choosing = true;
        chosen = false;
        mode = 0;
        for (int i = 0; i < 100; i++)
            v[i] = 0;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !choosing) SceneManager.LoadScene(1);
        if (Input.GetKeyDown(KeyCode.Return)) StartButton();
        if (Input.GetKeyDown(KeyCode.Alpha1) && !choosing) mode = 0;
        if (Input.GetKeyDown(KeyCode.Alpha2) && !choosing) mode = 1;
        if (Input.GetKeyDown(KeyCode.Backspace)) Reset();

        if (Input.GetKeyDown(KeyCode.F1))
        {
            help = true;
            canvas.enabled = true;
        }
        if (Input.GetKeyUp(KeyCode.F1))
        {
            canvas.enabled = false;
            help = false;
        }

        if (mode == 0 && !help) // cam movement mode
        {
            if (Input.GetMouseButtonDown(0))
                origin = cam.ScreenToWorldPoint(Input.mousePosition);
            if (Input.GetMouseButton(0))
            {
                difference = origin - cam.ScreenToWorldPoint(Input.mousePosition);
                cam.transform.position += difference;
            }
            cam.orthographicSize -= Input.GetAxis("Mouse ScrollWheel") * multiplier;
            if (cam.orthographicSize < sizeMin) cam.orthographicSize = sizeMin;
            if (cam.orthographicSize > sizeMax) cam.orthographicSize = sizeMax;
        }

        else if (mode == 1 && !help) // create mode
        {
            cam.orthographicSize -= Input.GetAxis("Mouse ScrollWheel") * multiplier;
            if (cam.orthographicSize < sizeMin) cam.orthographicSize = sizeMin;
            if (cam.orthographicSize > sizeMax) cam.orthographicSize = sizeMax;
            if (Input.GetMouseButtonDown(1)) // user removal
            {
                hit1 = Physics2D.GetRayIntersection(cam.ScreenPointToRay(Input.mousePosition)); // check for 2d collision
                if (hit1.collider != null)
                {
                    if (hit1.collider.name == "Node(Clone)")
                        RemoveNode(hit1.collider.gameObject);
                }
                else // check for 3d collision (mesh)
                {
                    if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out RaycastHit hit))
                        RemoveEdge(hit.collider.gameObject);
                }

            }
            if (Input.GetMouseButtonDown(0)) // placement
            {
                hit1 = Physics2D.GetRayIntersection(cam.ScreenPointToRay(Input.mousePosition));
                if (hit1.collider) // make line
                {
                    creating = true;
                    //create a line, destroy after if no contact
                    line = Instantiate(edge, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0));
                    lineRenderer = line.GetComponent<LineRenderer>();
                    lineRenderer.SetPosition(0, hit1.collider.transform.position);
                    // save script instances
                    script1 = hit1.collider.GetComponent<indexScript>();
                }
                else // make node
                {
                    Vector3 pos = cam.ScreenToWorldPoint(Input.mousePosition);
                    pos += new Vector3(0, 0, 10);
                    aNode = Instantiate(node, pos, quaternion.identity);
                    script1 = aNode.GetComponent<indexScript>();
                    script1.index = indexSet.Min; // assign new tag
                    nodes[indexSet.Min] = aNode; // add to list of nodes
                    indexSet.Remove(indexSet.Min);
                }
            }
            if (Input.GetMouseButton(0) && creating) // update edge to cursor position
            {
                Vector3 pos = cam.ScreenToWorldPoint(Input.mousePosition);
                pos += new Vector3(0, 0, 10);
                lineRenderer.SetPosition(1, pos);
            }
            if (Input.GetMouseButtonUp(0) && creating) // finalize edge placement
            {
                hit2 = Physics2D.GetRayIntersection(cam.ScreenPointToRay(Input.mousePosition));
                if (hit2.collider != null && hit1.collider != hit2.collider)
                {
                    lineRenderer.SetPosition(1, hit2.collider.transform.position);
                    script2 = hit2.collider.GetComponent<indexScript>();

                    if (lines[script1.index, script2.index] == null) // if edge doesn't exist
                    {
                        MeshCollider meshCollider = line.AddComponent<MeshCollider>();
                        Mesh mesh = new Mesh();
                        lineRenderer.BakeMesh(mesh, cam, true);
                        meshCollider.sharedMesh = mesh;
                        lines[script1.index, script2.index] = line;
                        lines[script2.index, script1.index] = line;
                        script2.edges.Add(line);
                    }
                    else // else destroy
                    {
                        script1.edges.Remove(line);
                        Destroy(line);
                    }
                }
                else
                {
                    script1.edges.Remove(line);
                    Destroy(line);
                    //destroy if we hit nothing
                }
                creating = false;
            }
        }

        if (choosing && !help)
        {
            if (!chosen)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    hit1 = Physics2D.GetRayIntersection(cam.ScreenPointToRay(Input.mousePosition)); // check for 2d collision
                    if (hit1.collider != null)
                    {
                        if (hit1.collider.name == "Node(Clone)")
                        {
                            script1 = hit1.collider.gameObject.GetComponent<indexScript>();
                            Queue.Enqueue(new Vector2(script1.index, script1.index));
                            v[script1.index] = 1;
                            chosen = true;
                            continueNext = true;
                        }
                    }
                }
                
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.RightArrow)) continueNext = true;

                if (continueNext == true)
                {
                    if (Queue.Count != 0)
                    {
                        int current = (int)Queue.Peek().x;
                        int second = (int)Queue.Peek().y;
                        if (current != second)
                        {
                            lineRenderer = lines[current, second].GetComponent<LineRenderer>();
                            lineRenderer.startColor = new Color(0f, 1f, 1f);
                            lineRenderer.endColor = new Color(0f, 1f, 0f);
                        }
                        Queue.Dequeue();
                        spriteRenderer = nodes[current].GetComponent<SpriteRenderer>();
                        spriteRenderer.color = new Color(0f, 0f, 0f);
                        for (int i = 0; i < 100; i++)
                            if (lines[current, i] != null && v[i] == 0)
                            {
                                Queue.Enqueue(new Vector2(i, current));
                                v[i] = 1;

                            }
                        continueNext = false;
                    }
                    else
                    {
                        choosing = false;
                        chosen = false;
                        continueNext = false;
                        ResetColor();
                    }
                }
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                choosing = false;
                chosen = false;
                continueNext = false;
                ResetColor();
            }
        }
    }


}
