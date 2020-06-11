using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AtmosphereTransition
{
    public class AtmosphereTransition : MonoBehaviour
    {

        [SerializeField] private AtmosphereTransitionTrigger trigger;

        #if UNITY_EDITOR
        [UnityEditor.MenuItem("GameObject/Create AtmosphereTransition", false, 10)]
        private static void CreateNewAtmosphereTransition(MenuCommand _command)
        {
            if(_command.context is GameObject)
            {
                GameObject holder = new GameObject("AtmosphereTransition");
                holder.transform.SetParent(((GameObject) _command.context).transform, false);
                holder.transform.localScale = Vector3.one;

                AtmosphereTransition transition = holder.AddComponent<AtmosphereTransition>();

                GameObject triggerHolder = new GameObject("AtmosphereTransitionTrigger");
                triggerHolder.transform.SetParent(holder.transform, false);
                transition.trigger = triggerHolder.AddComponent<AtmosphereTransitionTrigger>();
                transition.trigger.InitializationSetup();
                
                Selection.activeGameObject = holder;

                Undo.RegisterCreatedObjectUndo(holder, "Create AtmosphereTransition");
            }
        }
        #endif
    }
}
