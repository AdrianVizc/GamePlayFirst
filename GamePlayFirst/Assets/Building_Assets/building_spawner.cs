using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BuildingType {
    smallBuilding,
    largeBuilding
}

[ExecuteInEditMode]
public class BuildingSpawner : MonoBehaviour
{
    [Header("Building Parameters")]
    public BuildingType buildingType;
    public int floors = 2;

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

    void GenerateBuilding() {
        Vector3 nextPos = transform.position;
        float height = 0f;

        if (floors < 2)
            floors = 2;

        if (buildingType == BuildingType.smallBuilding)
        {
            GameObject[] allBasePrefabs = Resources.LoadAll<GameObject>("Building_Assets/small_building_base");
            GameObject[] allMiddlePrefabs = Resources.LoadAll<GameObject>("Building_Assets/small_building_middle");
            GameObject[] allRoofPrefabs = Resources.LoadAll<GameObject>("Building_Assets/small_building_roof");

            GameObject randomBasePrefab = allBasePrefabs[Random.Range(0, allBasePrefabs.Length)];
            GameObject randomRoofPrefab = allRoofPrefabs[Random.Range(0, allRoofPrefabs.Length)];

            // Base
            GameObject baseInstance = Instantiate(randomBasePrefab, nextPos, Quaternion.identity);
            height = GetHeight(baseInstance);
            nextPos += new Vector3(0, height, 0);

            // Middles
            if (floors > 2 && allMiddlePrefabs.Length > 0)
            {
                for (int i = 0; i < floors - 2; i++)
                {
                    GameObject randomMiddle = allMiddlePrefabs[Random.Range(0, allMiddlePrefabs.Length)];
                    GameObject mid = Instantiate(randomMiddle, nextPos, Quaternion.identity);
                    height = GetHeight(mid);
                    nextPos += new Vector3(0, height, 0);
                }
            }

            // Roof
            Instantiate(randomRoofPrefab, nextPos, Quaternion.identity);
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
