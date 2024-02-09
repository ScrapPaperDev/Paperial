
using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using static Paperial.EditorUtils.EditorConfig;
using UnityEditor;
#endif

namespace Paperial.EditorUtils
{
#if UNITY_EDITOR
    [ExecuteAlways]
#endif
    public class DesignHider : EditorMonoBehaviour
    {
#if UNITY_EDITOR
        private static List<Type> hiders = new List<Type>()
        {
            typeof(Renderer),
            typeof(MeshFilter),
            typeof(Collider),
            typeof(Rigidbody),
            typeof(DesignHider),
        };
        private Component[] comps;
#endif

        public bool hideTransform;
        public bool hideGo;


        private void OnEnable()
        {
#if UNITY_EDITOR
            comps = GetComponents<Component>();
            EditorApplication.playModeStateChanged += Doit;
            if (!Application.isPlaying)
                Doit(PlayModeStateChange.EnteredEditMode);
#else
        Destroy(this);
#endif


        }
#if UNITY_EDITOR
        private void Doit(PlayModeStateChange change)
        {
            if (hideGo)
            {
                if (!cheaterScrappie)
                    gameObject.hideFlags = HideFlags.HideInHierarchy;
                else
                    gameObject.hideFlags = HideFlags.None;

                return;
            }

            if (hideTransform)
                transform.hideFlags = HideFlags.HideInInspector;
            else
                transform.hideFlags = HideFlags.None;


            foreach (var item in comps)
            {
                if (cheaterScrappie)
                {
                    item.hideFlags = HideFlags.None;
                    continue;
                }

                if (!hiders.Exists(x => x.IsAssignableFrom(item.GetType()) || item.GetType().IsAssignableFrom(typeof(Behaviour))))
                    continue;


                if (change == PlayModeStateChange.EnteredEditMode)
                    item.hideFlags = HideFlags.HideInInspector;
                else if (change == PlayModeStateChange.EnteredPlayMode)
                    item.hideFlags = HideFlags.None;
            }
        }

        private void OnDisable()
        {
            EditorApplication.playModeStateChanged -= Doit;
        }
#endif

    }
#if UNITY_EDITOR

    [CustomEditor(typeof(DesignHider)), CanEditMultipleObjects]
    public class DesignHider_Editor : Editor
    {
        private DesignHider linked;
        private Component[] components;


        private void OnEnable()
        {
            linked = (DesignHider)target;
        }

        public override void OnInspectorGUI() { }
    }
#endif
}