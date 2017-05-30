using System.Collections.Generic;
using UnityEngine;
using InControl;


[RequireComponent( typeof(InControlInputModule) )]
public class InputModuleActionAdapter : MonoBehaviour
{
    StandardMenuAction menuAction;

    #region delegate events
    public delegate void MenuCancelAction();
    public static event MenuCancelAction OnCancel;
    #endregion

    void OnEnable()
    {
        menuAction = StandardMenuAction.CreateStandardBinding();

        InControlInputModule inputModule = GetComponent<InControlInputModule>();
        if (inputModule != null)
        {
            inputModule.SubmitAction = menuAction.Submit;
            inputModule.CancelAction = menuAction.Cancel;
            inputModule.MoveAction = menuAction.Move;
        }
    }

    void Update(){
        if(menuAction.Cancel.WasPressed)
            OnCancel();
    }

    void OnDisable()
    {
         menuAction.Destroy();
    }
}
