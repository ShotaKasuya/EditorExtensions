using UnityEngine;

namespace Module.EditorExtension.Runtime.Attribute
{
    public enum CoordinateSpace
    {
        Local,
        World
    }

    /// <summary>
    /// Apply to a Vector2 or Vector3 field to display a position handle in the Scene view.
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Field)]
    public class SceneVectorAttribute : PropertyAttribute
    {
        public readonly CoordinateSpace Space;

        public SceneVectorAttribute(CoordinateSpace space = CoordinateSpace.Local)
        {
            Space = space;
        }
    }
}
