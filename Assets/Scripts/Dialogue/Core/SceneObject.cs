using UnityEngine;

public abstract class SceneObject : MonoBehaviour {
    private SceneObject instance;

    /// <summary>
    /// The instance of the object in the current scene.
    /// </summary>
    protected SceneObject Instance {
        get {
            if (instance == null || instance.Equals(null)) {
                instance = GetInstance();
            }
            return instance;
        }
    }
    
    /// <summary>
    /// Whether this object has a valid instance in the current scene.
    /// </summary>
    public bool hasInstance { get { return Instance != null; } }

    /// <summary>
    /// Finds a SceneObject in the currently running scene with the
    /// same name. NOTE: Currently unable to find disabled objects.
    /// </summary>
    protected virtual SceneObject GetInstance() {
        SceneObject[] objs = FindObjectsOfType<SceneObject>();
        foreach (SceneObject obj in objs) {
            if (obj.Equals(this)) {
                return obj;
            }
        }
        return null;
    }

    /// <summary>
    /// Equality of game items will be based on their gameobject's name.
    /// Items must be of the same runtime type to be considered equal.
    /// </summary>
    public override bool Equals(object other) {
        if (other == null) { return base.Equals(other); }
        else {
            return (other.GetType() == GetType() &&
            ((SceneObject)other).gameObject.name == gameObject.name);
        }
    }

    /// <summary>
    /// Returns the hashcode of this object. (Hashcode override required
    /// to correspod to Equals() override.)
    /// </summary>
    public override int GetHashCode() {
        return gameObject.name.GetHashCode() + GetType().GetHashCode();
    }
}