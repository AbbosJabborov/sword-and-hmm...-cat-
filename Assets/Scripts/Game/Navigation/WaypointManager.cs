using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointManager : MonoBehaviour
{
    public GameObject markerPrefab;
    public Camera cam;

    public void CreateMarker(Transform target)
    {
        GameObject marker = Instantiate(markerPrefab, transform);
        var ui = marker.GetComponent<WaypointMarker>();
        ui.target = target;
        ui.cam = cam;
    }
}

