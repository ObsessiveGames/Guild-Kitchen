using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotCounterVisual : MonoBehaviour
{
    [SerializeField] private PotCounter potCounter;
    [SerializeField] private Transform counterTopPoint;
    [SerializeField] private Transform potVisualPrefab;


    private List<GameObject> potVisualGameObjectList;

    private void Awake()
    {
        potVisualGameObjectList = new List<GameObject>();
    }

    private void Start()
    {
        potCounter.OnPotSpawned += PotCounter_OnPotSpawned;
        potCounter.OnPotRemoved += PotCounter_OnPotRemoved;
    }

    private void PotCounter_OnPotRemoved(object sender, System.EventArgs e)
    {
        GameObject potGameObject = potVisualGameObjectList[potVisualGameObjectList.Count - 1];
        potVisualGameObjectList.Remove(potGameObject);
        Destroy(potGameObject);
    }

    private void PotCounter_OnPotSpawned(object sender, System.EventArgs e)
    {
        Transform potVisualTransform = Instantiate(potVisualPrefab, counterTopPoint);

        potVisualTransform.localPosition = Vector3.zero;

        potVisualGameObjectList.Add(potVisualTransform.gameObject);
    }
}
