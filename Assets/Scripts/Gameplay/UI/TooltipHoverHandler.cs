using System.Collections;
using System.Collections.Generic;
using TMPro;
using Action = Unity.BossRoom.Gameplay.Actions.Action;
using UnityEngine;
using UnityEngine.UIElements;
using Unity.BossRoom.Gameplay.Actions;
using Unity.BossRoom.Gameplay.UI;
using Unity.BossRoom.Gameplay.GameplayObjects;

public class TooltipHoverHandler : MonoBehaviour
{

    [SerializeField]
    private UITooltipDetector uITooltipDetector;

    [SerializeField]
    private UITooltipPopup uiTooltipPopup;

    private ActionID actionId = new ActionID();


    private void Update()
    {
        int linkIndex = TMP_TextUtilities.FindIntersectingLink(uiTooltipPopup.TextField, Input.mousePosition,
            uiTooltipPopup.Canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : uiTooltipPopup.Canvas.worldCamera);
        if (linkIndex != -1)
        {
            if (!uITooltipDetector.IsPointerOverElement)
            {
                uITooltipDetector.enabled = true;
                TMP_LinkInfo linkInfo = uiTooltipPopup.TextField.textInfo.linkInfo[linkIndex];
                actionId.ID = int.Parse(linkInfo.GetLinkID());
                uITooltipDetector.SetText(GameDataSource.Instance.GetActionPrototypeByID(actionId).Config.ReturnDetailedTooltip());
                uITooltipDetector.OnPointerEnterProcessing();
            }
        }
        else if (uITooltipDetector.enabled == true)
        {
            uITooltipDetector.enabled = false;
        }

    }
}
