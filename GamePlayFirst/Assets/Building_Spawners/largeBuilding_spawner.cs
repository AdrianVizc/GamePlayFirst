using UnityEngine;

public enum largeBuildingVersion
{
    one,
    two,
    three,
    four
}

[ExecuteInEditMode]
public class largeBuilding_Spawner : MonoBehaviour
{
    [Header("Building Parameters")]
    public largeBuildingVersion buildingVersion;
    public int floors = 2;
    public bool flip = false;

    [Header("Building Scale")]
    public float x = 1;
    public float y = 1;
    public float z = 1;

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

        GameObject[] allBasePrefabs = null;
        GameObject[] allMiddlePrefabs = null;
        GameObject[] allRoofPrefabs = null;

        switch (buildingVersion)
        {
            case largeBuildingVersion.one:
                allBasePrefabs = Resources.LoadAll<GameObject>("Building_Assets/large_building_base/1");
                allMiddlePrefabs = Resources.LoadAll<GameObject>("Building_Assets/large_building_middle/1");
                allRoofPrefabs = Resources.LoadAll<GameObject>("Building_Assets/large_building_roof/1");
                break;
            case largeBuildingVersion.two:
                allBasePrefabs = Resources.LoadAll<GameObject>("Building_Assets/large_building_base/1");
                allMiddlePrefabs = Resources.LoadAll<GameObject>("Building_Assets/large_building_middle/2");
                allRoofPrefabs = Resources.LoadAll<GameObject>("Building_Assets/large_building_roof/2");
                break;
            case largeBuildingVersion.three:
                allBasePrefabs = Resources.LoadAll<GameObject>("Building_Assets/large_building_base/1");
                allMiddlePrefabs = Resources.LoadAll<GameObject>("Building_Assets/large_building_middle/3");
                allRoofPrefabs = Resources.LoadAll<GameObject>("Building_Assets/large_building_roof/3");
                break;
            case largeBuildingVersion.four:
                allBasePrefabs = Resources.LoadAll<GameObject>("Building_Assets/large_building_base/2");
                allMiddlePrefabs = Resources.LoadAll<GameObject>("Building_Assets/large_building_middle/4");
                allRoofPrefabs = Resources.LoadAll<GameObject>("Building_Assets/large_building_roof/4");
                break;
        }

        GameObject randomBasePrefab = allBasePrefabs[Random.Range(0, allBasePrefabs.Length)];
        GameObject randomRoofPrefab = allRoofPrefabs[0];

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
            if (buildingVersion == largeBuildingVersion.two || buildingVersion == largeBuildingVersion.three)
            {
                randomRoofPrefab = allRoofPrefabs[1];

            }
        }

        Instantiate(randomRoofPrefab, nextPos, Quaternion.identity, buildingParent.transform);

        // Scale end result
        buildingParent.transform.localScale = new Vector3(x, y, z);
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
