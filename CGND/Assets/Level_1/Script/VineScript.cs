using UnityEngine;
using UnityEngine.UIElements;

public class VineScript : MonoBehaviour
{
    [SerializeField] private GameObject vinePrefab;
    [SerializeField] private float vineSpacing = -0.983f;
    [SerializeField] private int maxGrowth = 10; 
    [SerializeField] private GameObject root; 

    private int growthCount = 0;
    private bool hasGrown = false;

    public void Grow() {
        if (hasGrown) return;
        if (growthCount >= maxGrowth) return;

        hasGrown = true;

        VineRoot.instance.ExtendColliderDown(Mathf.Abs(vineSpacing));

            //ExtendColliderDown(Mathf.Abs(vineSpacing));

        Vector3 spawnPos = new Vector3(0f, vineSpacing, 0f); 
        GameObject newVine = Instantiate(vinePrefab, transform);
        newVine.transform.localPosition = spawnPos;

        VineScript vineScript = newVine.GetComponent<VineScript>();
        if (vineScript != null) {
            vineScript.vinePrefab = vinePrefab;
            vineScript.growthCount = growthCount + 1;
            vineScript.maxGrowth = maxGrowth;
        }
    }

}
