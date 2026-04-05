using UnityEngine;

namespace _Strategy.Runtime.Piece
{
    public static class TransformUtils
    {
        /// <summary>
        /// Gets a component of type T from children by searching for a GameObject with the specified name
        /// </summary>
        /// <typeparam name="T">The type of component to retrieve</typeparam>
        /// <param name="parent">The parent GameObject to search in</param>
        /// <param name="childName">The name of the child GameObject to find</param>
        /// <returns>The component of type T if found, null otherwise</returns>
        public static T GetComponentInChildrenByName<T>(this MonoBehaviour parent, string childName) where T : Component
        {
            T[] children = parent.GetComponentsInChildren<T>(true);
    
            foreach (var child in children)
            {
                if (child != parent.transform && child.name == childName)
                {
                    return child.GetComponent<T>();
                }
            }
    
            return null;
        }

    }
}