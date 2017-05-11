using System.Collections.Generic;
using UnityEngine;
using InControl;


[RequireComponent( typeof(InControlInputModule) )]
public class InputModuleActionAdapter : MonoBehaviour
{
    StandardMenuAction actions;

    void OnEnable()
    {
        actions = StandardMenuAction.CreateStandardBinding();

        var inputModule = GetComponent<InControlInputModule>();
        if (inputModule != null)
        {
            inputModule.SubmitAction = actions.Submit;
            inputModule.CancelAction = actions.Cancel;
            inputModule.MoveAction = actions.Move;
        }
    }

    void OnDisable()
    {
        DestroyActions();
    }

    void DestroyActions()
    {
        actions.Destroy();
    }
}
