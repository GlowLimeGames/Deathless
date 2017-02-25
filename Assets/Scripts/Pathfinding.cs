using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding {
    public static List<Vector3> GetPath(Vector3 currentPos, Vector2 targetPos) {
        return GetPath(currentPos, GetZPos(targetPos));
    }

    private static List<Vector3> GetPath(Vector3 currentPos, Vector3 targetPos) {
        List<Vector3> waypoints = new List<Vector3>();
        waypoints.Add(targetPos); //Placeholder for more complex pathfinding
        return waypoints;
    }

    private static Vector3 GetZPos(Vector2 point) {
        float zPos = point.y; //Placeholder
        return new Vector3(point.x, point.y, zPos);
    }
}