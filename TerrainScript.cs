using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainScript : MonoBehaviour
{
    Vector3[,] matrix;
    [SerializeField, Range(1, 50)] int xSize = 10;
    [SerializeField, Range(1, 50)] int zSize = 10;
    [SerializeField] float distanceUnit = 1f;
    Vector3 oldSize = new Vector3(10, 10);
    
    Vector3[] vertices;
    int[] triangles;
    

    Mesh mesh;

    [Header("Wave Properties")]
    [SerializeField] float wave1Freq = 1f;
    [SerializeField] float wave1Height = 1f;

    private void Awake()
    {
        //------Initialize Mesh--------//
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        oldSize = new Vector3(xSize, zSize);
    }
    private void Start()
    {
        matrix = new Vector3[xSize,zSize];
        UpdateMatrixSize();
        UpdateMatrixPosition();
        UpdateVerticesAndTriangles();


        StartCoroutine(Wave1(wave1Freq, wave1Height));
    }
    private void Update()
    {
        if (oldSize.x != xSize || oldSize.z != zSize)
        {
            UpdateMatrixSize();
            UpdateMatrixPosition();
            UpdateVerticesAndTriangles();
        }
        oldSize = new Vector3(xSize, zSize);//Update old sizes
        
    }

    /// <summary>The Matrix----------------------------------------------------------
    /// 
    ///z^     x - x - x
    /// |     |   |   |
    /// |     x - x - x
    /// |     |   |   |
    /// |     x - x - x 
    /// |
    /// |
    /// 0-------------->
    ///                x
    ///                
    /// The parsing is made from 0 to z and then from 0 to x
    /// </summary>
    void UpdateMatrixSize()
    {
        Vector3[,] newMatrix = new Vector3[xSize, zSize];

        //Copy first matrix in this one if Possible
        for (int x = 0; x < xSize; x++)
        {
                for (int z = 0; z < zSize; z++)
                {
                    try
                    {
                        newMatrix[x, z] = matrix[x, z];
                    }
                    catch { }
                
                }
        }
        //Add other vertices to the matrix if Possible
        for (int x = 0; x < xSize; x++)
        {
            for (int z = 0; z < zSize; z++)
            {
                if (newMatrix[x,z] == null)
                {
                    newMatrix[x, z] = new Vector3();
                }
            }
        }
        matrix = null;
        matrix = (Vector3[,])newMatrix.Clone();
    }
    void UpdateMatrixPosition()
    {
        Vector3 pos = transform.position;
        for (int x = 0; x < xSize; x++)
        {
            for (int z = 0; z < zSize; z++)
            {
                matrix[x, z] = pos + new Vector3(x, 0, z);
            }
        }
    }
    void UpdateVerticesAndTriangles()
    {
        vertices = new Vector3[xSize * zSize];
        triangles = new int[xSize*zSize*6]; //its X3 because triangles stores the 3 vert of a triangle

        for (int counter = 0, x = 0; x < xSize; x++)
        {
            for (int z = 0; z < zSize; z++)
            {
                vertices[counter++] = matrix[x, z];
            }
        }
        triangles[0] = 0;
        triangles[1] = xSize + 1;
        triangles[2] = 1;

        int tris = 0;
         for (int x = 0; x < xSize-1; x++)
         {
             for (int z = 0; z < zSize-1; z++)
             {
                triangles[tris++] = z     + xSize*x;
                triangles[tris++] = z + 1 + xSize * x;
                triangles[tris++] = z     + zSize*(x+1);

                triangles[tris++] = z + 1 + xSize * x;
                triangles[tris++] = z + 1 + zSize * (x+1);
                triangles[tris++] = z     + zSize * (x+1);

                //for Looking up add 1 to z, for looking right add 1 to x when multiply
             }
         }
 
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }
    IEnumerator Wave1(float frequency, float height)
    {
        
        while (true)
        {
            for (int x = 0; x < xSize; x++)
            {

                //it waves on the x line
                for (int z = 0; z < zSize; z++)
                {
                    matrix[x, z].y *= Mathf.Sin(Time.time) * height;




                }
                //yield return new WaitForSeconds(frequency/100);
                
            }
            UpdateVerticesAndTriangles();
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (matrix == null)
            return;
        for (int x = 0; x < xSize; x++)
        {
            for (int z = 0; z < zSize; z++)
            {
                if (matrix[x, z] != null)
                {
                    Gizmos.DrawSphere(matrix[x, z], 0.1f);
                }
            }
        }
    }




    //---------------Getters------------------//
    public int GetXSize()
    {
        return xSize;
    }
    public int GetZSize()
    {
        return zSize;
    }
    public float GetDistanceUnit()
    {
        return distanceUnit;
    }
}
