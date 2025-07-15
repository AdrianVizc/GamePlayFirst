using UnityEngine;

public enum mediumBuildingVersion
{
    one,
    two,
    three
}

[ExecuteInEditMode]
public class mediumBuilding_Spawner : MonoBehaviour
{
    [Header("Building Parameters")]
    public mediumBuildingVersion buildingVersion;
    public int floors = 2;
    public bool flip = false;

    [Space(10)]
    public bool spawnBuilding;

    void Update()
    {
        if (spawnBuilding)
        {
            GenerateBuilding();
            spawnBuilding = false;
        }
    }

    void GenerateBuilding()
    {
        GameObject buildingParent = new GameObject("Building");
        buildingParent.transform.position = transform.position;
        Vector3 nextPos = transform.position;
        float height = 0f;

        if (floors < 2)
            floors = 2;

        GameObject[] allBasePrefabs = Resources.LoadAll<GameObject>("Building_Assets/medium_building_base");
        GameObject[] allMiddlePrefabs = null;
        GameObject[] allRoofPrefabs = null;

        switch (buildingVersion)
        {
            case mediumBuildingVersion.one:
                allMiddlePrefabs = Resources.LoadAll<GameObject>("Building_Assets/medium_building_middle/1");
                allRoofPrefabs = Resources.LoadAll<GameObject>("Building_Assets/medium_building_roof/1");
                break;
            case mediumBuildingVersion.two:
                allMiddlePrefabs = Resources.LoadAll<GameObject>("Building_Assets/medium_building_middle/2");
                allRoofPrefabs = Resources.LoadAll<GameObject>("Building_Assets/medium_building_roof/2");
                break;
            case mediumBuildingVersion.three:
                allMiddlePrefabs = Resources.LoadAll<GameObject>("Building_Assets/medium_building_middle/3");
                allRoofPrefabs = Resources.LoadAll<GameObject>("Building_Assets/medium_building_roof/3");
                break;
            default:
                allMiddlePrefabs = Resources.LoadAll<GameObject>("Building_Assets/medium_building_middle/1");
                allRoofPrefabs = Resources.LoadAll<GameObject>("Building_Assets/medium_building_roof/1");
                break;
        }

        GameObject randomBasePrefab = allBasePrefabs[Random.Range(0, allBasePrefabs.Length)];
        GameObject randomRoofPrefab = allRoofPrefabs[Random.Range(0, allRoofPrefabs.Length)];

        // Base
        GameObject baseInstance = Instantiate(randomBasePrefab, nextPos, Quaternion.identity, buildingParent.transform);
        height = GetHeight(baseInstance);
        nextPos += new Vector3(0, height, 0);

        // Middles
        if (floors > 2 && allMiddlePrefabs.Length > 0)
        {
            for (int i = 0; i < floors - 2; i++)
            {
                GameObject randomMiddle = allMiddlePrefabs[Random.Range(0, allMiddlePrefabs.Length)];
                if (flip)
                {
                    GameObject mid = Instantiate(randomMiddle, nextPos, Quaternion.Euler(180, 0, 0), buildingParent.transform);
                    height = GetHeight(mid);
                    mid.transform.position += new Vector3(0, height, 0);
                }
                else
                {
                    GameObject mid = Instantiate(randomMiddle, nextPos, Quaternion.identity, buildingParent.transform);
                    height = GetHeight(mid);
                }
                nextPos += new Vector3(0, height, 0);
            }
        }

        // Roof
        if (flip)
        {
            GameObject roof = Instantiate(randomRoofPrefab, nextPos, Quaternion.Euler(180, 0, 0), buildingParent.transform);
            height = GetHeight(roof);
            roof.transform.position += new Vector3(0, height, 0);
        }
        else
        {
            Instantiate(randomRoofPrefab, nextPos, Quaternion.identity, buildingParent.transform);
        }
    }

    float GetHeight(GameObject obj)
    {
        float maxHeight = 0f;
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renderers)
        {
            maxHeight = Mathf.Max(maxHeight, r.bounds.size.y);
        }
        return maxHeight;
    }
}
