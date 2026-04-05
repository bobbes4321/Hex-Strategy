using Reflex.Core;
using Reflex.Extensions;
using UnityEngine.SceneManagement;

namespace _Strategy.Runtime.Board
{
    public static class DI
    {
        public static T Resolve<T>() => GetSceneContainer().Resolve<T>();
        public static Container GetSceneContainer() => SceneManager.GetActiveScene().GetSceneContainer();
    }
}