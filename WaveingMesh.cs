using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class WaveingMesh : MonoBehaviour
{

    public uint width = 256;
    public uint height = 256;
    public float size = 1f;
    public float noiseScale = 10f;

    [Header("Modifiable at RunTime")]
    public float altitude = 5f;
    public float stress = 5f;

    private Vector3[][] matrix;
    private float[][] matrixOffset;
    private Mesh mesh;
    private void Awake()
    {
        matrix = new Vector3[height][];
        matrixOffset = new float[height][];
        for (int i = 0; i < height; i++)
        {
            matrix[i] = new Vector3[width];
            matrixOffset[i] = new float[width];
        }
        mesh = new Mesh();
        mesh = GetComponent<MeshFilter>().mesh;
    }
    private void Start()
    {
        InitMatrix(matrix, width, height, noiseScale,altitude); 
    }
    private void Update()
    {
        WaveMatrix();
        UpdateMesh();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        try
        {
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    Gizmos.DrawSphere(matrix[i][j], .25f);
                }
            }

        }
        catch { }
       
    }

    private void InitMatrix(Vector3[][] m, uint w, uint h, float noiseScale = 10f, float altitude = 5f)
    {
        for (int i = 0; i < h; i++)
        {
            for (int j = 0; j < w; j++)
            {
                float yValue = Mathf.PerlinNoise((float)i / height * noiseScale, (float)j / width * noiseScale);
                m[i][j] = new Vector3(i * size, yValue * altitude, j * size);
                matrixOffset[i][j] = yValue;
            }
        }
    }
    private void WaveMatrix()
    {
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                matrix[i][j].y = Mathf.Sin(Time.time + matrixOffset[i][j]*stress) * altitude;
   
            }
        }
    }
    private void UpdateMesh()
    {
        Vector3[] vertices = new Vector3[height * width];
        for (int i = 0, counter = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                vertices[counter++] = matrix[i][j];
            }
        }


        int tris = 0;
        int[] triangles = new int[height*width*6];



        for (int i = 0; i < height - 1; i++)
        {
            for (int j = 0; j < width -1; j++)
            {
                triangles[tris++] = j + (int)height * i;
                triangles[tris++] = j + 1 + (int)height * i;
                triangles[tris++] = j + (int)width * (i + 1);
                triangles[tris++] = j + 1 + (int)height*i;
                triangles[tris++] = j + 1 + (int)width * (i + 1);
                triangles[tris++] = j + +(int)width * (i + 1);
            }
        }

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        
    }
}
