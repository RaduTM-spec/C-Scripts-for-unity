using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CameraPosition
{
    TopDown,
    Angled45d,
    Flat
}
public class CameraScript : MonoBehaviour
{
    [SerializeField] GameObject target;
    Component targetScript;

    Vector3 position;
    [SerializeField] float speed = 10f;
    [SerializeField]float height = 20f;

    CameraPosition camState;



    private void Awake()
    {

        target = GameObject.Find("Terrain");
        targetScript = target.GetComponent<TerrainScript>();


        camState = CameraPosition.TopDown;
        transform.rotation = Quaternion.identity * Quaternion.Euler(90, 0, 0);

    }
    void Start()
    {
        
       if (targetScript && targetScript.GetType() == typeof(TerrainScript))
            StartCoroutine(UpdateThroughTerrain());
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            camState = CameraPosition.TopDown;
            transform.rotation = Quaternion.identity * Quaternion.Euler(90, 0, 0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            camState = CameraPosition.Angled45d;
            transform.rotation = Quaternion.identity * Quaternion.Euler(45, 0, 0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        { 
            camState = CameraPosition.Flat;
            transform.rotation = Quaternion.identity;
        }
    }
    IEnumerator UpdateThroughTerrain()
    {
        TerrainScript terrainScript = targetScript as TerrainScript;
        Vector3 startPos = transform.position;
        Vector3 endPos = FindEndPosByCamState();

        float xAdd = (endPos.x - startPos.x) / 100;
        float yAdd = (endPos.y - startPos.y) / 100;
        float zAdd = (endPos.z - startPos.z) / 100;
        int d = 0;
        while (d < 100)
        {
            transform.position += new Vector3(xAdd,yAdd,zAdd);
            yield return 1 / speed;
            d++;
        }
        StartCoroutine(UpdateThroughTerrain());
    }
    Vector3 FindEndPosByCamState()
    {
        TerrainScript terrainScript = targetScript as TerrainScript;
        if (camState == CameraPosition.TopDown)
            return target.transform.position + new Vector3((terrainScript.GetXSize() - 1) / 2f, Mathf.Max(terrainScript.GetXSize(), terrainScript.GetZSize()), (terrainScript.GetZSize() - 1) / 2f) * terrainScript.GetDistanceUnit();
        else if (camState == CameraPosition.Angled45d)
            return target.transform.position + new Vector3((terrainScript.GetXSize() - 1) / 2f, Mathf.Max(terrainScript.GetXSize(), terrainScript.GetZSize()), -terrainScript.GetXSize()/2) * terrainScript.GetDistanceUnit();
        else if (camState == CameraPosition.Flat)
            return target.transform.position + new Vector3((terrainScript.GetXSize() - 1) / 2f, 0f, -terrainScript.GetXSize()/2) * terrainScript.GetDistanceUnit();

        return Vector3.zero;
    }
}
