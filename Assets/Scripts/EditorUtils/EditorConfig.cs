#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Paperial.EditorUtils
{
    public static class EditorConfig
    {

        public static bool cheaterScrappie;


        [MenuItem("Tools/ funcName")]
        private static void ScrappieCheater()
        {
            cheaterScrappie = !cheaterScrappie;
            Debug.Log(cheaterScrappie);
        }
    }
}
#endif