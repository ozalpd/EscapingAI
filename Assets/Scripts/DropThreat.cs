using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropThreat : MonoBehaviour
{
    public GameObject threatPrefab;
    AIControl[] agents;
    Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
            Debug.LogError("No main camera found!");
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hitInfo;
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray.origin, ray.direction, out hitInfo))
            {
                agents = FindObjectsOfType<AIControl>();
                Instantiate(threatPrefab, hitInfo.point, threatPrefab.transform.rotation);
                foreach (var a in agents)
                {
                    a.DetectThreatAndFlee(hitInfo.point);
                }
            }
        }
    }
}
