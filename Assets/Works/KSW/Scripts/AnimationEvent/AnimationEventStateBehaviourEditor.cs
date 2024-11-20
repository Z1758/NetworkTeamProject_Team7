#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

[CustomEditor(typeof(AnimationEventStateBehaviour))]
public class AnimationEventStateBehaviourEditor : Editor
{
    AnimationClip previewClip;
    float previewTime;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        AnimationEventStateBehaviour stateBehaviour = (AnimationEventStateBehaviour)target;
        if(Validate(stateBehaviour,out string errorMessage))
        {
            GUILayout.Space(10);

          
          
            PreviewAnimationClip(stateBehaviour);

            GUILayout.Label($"Previewing at {previewTime:F2}s", EditorStyles.helpBox);
        }
        else
        {
            EditorGUILayout.HelpBox(errorMessage, MessageType.Info);
        }
    }

    private void PreviewAnimationClip(AnimationEventStateBehaviour stateBehaviour)
    {
        if (previewClip == null) return;

        previewTime = stateBehaviour.triggerTime * previewClip.length;

        AnimationMode.StartAnimationMode();
        AnimationMode.SampleAnimationClip(Selection.activeGameObject,previewClip,previewTime);
        AnimationMode.StopAnimationMode();
    }

    bool Validate(AnimationEventStateBehaviour stateBehaviour, out string errorMessage)
    {
        AnimatorController animatorController = GetValidAnimatorController(out errorMessage);
        if (animatorController == null) return false;

        ChildAnimatorState matchingState = animatorController.layers.
            SelectMany(layer => layer.stateMachine.states)
            .FirstOrDefault(state => state.state.behaviours.Contains(stateBehaviour));
        
        previewClip = matchingState.state?.motion as AnimationClip;
        if(previewClip == null){
            errorMessage = "유효하지 않은 클립";
            return false;
        }

        return true;
        
    }

    AnimatorController GetValidAnimatorController(out string errorMessage)
        {
            errorMessage = string.Empty;

            GameObject targetGameObject = Selection.activeGameObject;
            if(targetGameObject == null)
            {
                errorMessage = "게임 오브젝트를 선택해야 함";
                return null;
            }

            Animator animator = targetGameObject.GetComponent<Animator>();
        if (animator == null)
        {
            errorMessage = "선택한 게임 오브젝트에 애니메이터 컴포넌트가 없음";
            return null;
        }

        AnimatorController animatorController = animator.runtimeAnimatorController as AnimatorController;
        if(animatorController == null)
        {
            errorMessage = "선택한 애니메이터가 유효하지 않음";
            return null;
        }

        return animatorController;

    }
    

}
#endif