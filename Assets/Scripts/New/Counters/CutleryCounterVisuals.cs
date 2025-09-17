using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutleryCounterVisual : MonoBehaviour
{
    [SerializeField] private CutleryCounter cutleryCounter;
    [SerializeField] private Transform counterTopPoint;
    [SerializeField] private Transform cutleryVisualPrefab;


    private List<GameObject> cutleryVisualGameObjectList;

    private void Awake()
    {
        cutleryVisualGameObjectList = new List<GameObject>();
    }

    private void Start()
    {
        cutleryCounter.OnCutlerySpawned += CutleryCounter_OnCutlerySpawned;
        cutleryCounter.OnCutleryRemoved += CutleryCounter_OnCutleryRemoved;
    }

    private void CutleryCounter_OnCutleryRemoved(object sender, System.EventArgs e)
    {
        GameObject cutleryGameObject = cutleryVisualGameObjectList[cutleryVisualGameObjectList.Count - 1];
        cutleryVisualGameObjectList.Remove(cutleryGameObject);
        Destroy(cutleryGameObject);
    }

    private void CutleryCounter_OnCutlerySpawned(object sender, System.EventArgs e)
    {
        Transform cutleryVisualTransform = Instantiate(cutleryVisualPrefab, counterTopPoint);

        float cutleryoffsetY = .1f;
        cutleryVisualTransform.localPosition = new Vector3(0, cutleryoffsetY * cutleryVisualGameObjectList.Count, 0);

        cutleryVisualGameObjectList.Add(cutleryVisualTransform.gameObject);
    }
}
