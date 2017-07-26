using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles in-game character and object movement through a scene.
/// </summary>
public class Navigation {
    /// <summary>
    /// Get a path to the given point, in the form of an ordered list of waypoints.
    /// </summary>
    public static List<Vector3> GetPath(Vector3 currentPos, Vector2 targetPos) {
        return GetPath(currentPos, GetZPos(targetPos));
    }

    /// <summary>
    /// Get a path to the given position, in the form of an ordered list of waypoints.
    /// </summary>
    private static List<Vector3> GetPath(Vector3 currentPos, Vector3 targetPos) {
        List<Vector3> waypoints = new List<Vector3>();

        //Placeholder for more complex pathfinding
        waypoints.Add(targetPos);

        return waypoints;
    }

    /// <summary>
    /// Find the z-position for the given x,y coordinates.
    /// </summary>
    private static Vector3 GetZPos(Vector2 point) {
        //Placeholder. Ultimately z-position should be
        //determined by waypoint objects.
        float zPos = point.y;
        return new Vector3(point.x, point.y, zPos);
    }
}