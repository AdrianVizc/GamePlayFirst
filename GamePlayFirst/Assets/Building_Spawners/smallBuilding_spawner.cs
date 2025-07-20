using UnityEngine;

[ExecuteInEditMode]
public class smallBuilding_Spawner : MonoBehaviour
{
    [Header("Building Parameters")]
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

        GameObject[] allBasePrefabs = Resources.LoadAll<GameObject>("Building_Assets/small_building_base");
        GameObject[] allMiddlePrefabs = Resources.LoadAll<GameObject>("Building_Assets/small_building_middle");
        GameObject[] allRoofPrefabs = Resources.LoadAll<GameObject>("Building_Assets/small_building_roof");

        GameObject randomBasePrefab = allBasePrefabs[Random.Range(0, allBasePrefabs.Length)];
        GameObject randomRoofPrefab = allRoofPrefabs[Random.Range(0, allRoofPrefabs.Length)];

        // Base
        if (flip)
        {
            GameObject baseInstance = Instantiate(allBasePrefabs[0], nextPos, Quaternion.identity, buildingParent.transform);
            height = GetHeight(baseInstance);
        }
        else
        {
            GameObject baseInstance = Instantiate(randomBasePrefab, nextPos, Quaternion.identity, buildingParent.transform);
            height = GetHeight(baseInstance);
        }
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
