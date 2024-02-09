using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Paperial.EditorUtils
{

    [DisallowMultipleComponent]
    public class Notes : EditorMonoBehaviour
    {

        [TextArea] public string note;

#if UNITY_EDITOR
        [NonSerialized] public bool e;
        [NonSerialized] public List<string> todos;
        [NonSerialized] public List<string> todosMidPrio;
        [NonSerialized] public List<string> todosHighPrio;
        public MessageType type;

        [ContextMenu("Edit Note")] private void EditNote() => e = true;
#endif

    }


    public class EditorMonoBehaviour : MonoBehaviour
    {
#if UNITY_STANDALONE && !UNITY_EDITOR
        protected virtual void OnValidate()
        {
            Destroy(this);
        }
#endif
    }



#if UNITY_EDITOR


    [CustomEditor(typeof(Notes))]
    public class NoteEditor : Editor
    {
        Notes linked;

        private void OnEnable()
        {
            linked = (Notes)target;
            linked.todos = new List<string>();
            linked.todosMidPrio = new List<string>();
            linked.todosHighPrio = new List<string>();
            MonoScript[] scripts = MonoImporter.GetAllRuntimeMonoScripts();
            var scriptsOnThisObj = linked.GetComponents<Component>();

            foreach (var item in scriptsOnThisObj)
            {
                MonoScript s = scripts.FirstOrDefault(x =>
                {
                    var a = x.GetClass();
                    if (a == null)
                        return false;
                    var b = item.GetType().Name;

                    if (b == "Notes")
                        return false;

                    return a.Name == b;
                });

                if (s == null)
                    continue;
                // TODO: replace replace with regex exp 
                // TODO: add throws as crit/ get contents of crit message using regex
                // TODO: make a criticality level
                IEnumerable<string> todoLines = s.text.Split('\n').Where(L => L.Contains("TODO: ") && !L.Contains("IEnumerable<string> todoLines")).Select(y => "(" + s.GetClass().Name + ")" + " " + y.Replace("//", "").TrimStart());
                IEnumerable<string> critLines = s.text.Split('\n').Where(L => L.Contains("NotImplemented")).Select(y => y.TrimStart());
                linked.todos.AddRange(todoLines);
                linked.todos.AddRange(critLines);
            }
        }

        private void OnDisable()
        {
            linked.e = false;
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox(linked.note, linked.type);

            foreach (var item in linked.todosHighPrio)
                EditorGUILayout.HelpBox(item, MessageType.Error);

            foreach (var item in linked.todos)
                EditorGUILayout.HelpBox(item, MessageType.Warning);

            if (linked.e)
                DrawDefaultInspector();
        }
    }
#endif
}
