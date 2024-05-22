using TMPro;
using UnityEngine;
using Unity.BossRoom.Gameplay.Actions;
using Unity.BossRoom.Gameplay.UI;
using Unity.BossRoom.Gameplay.GameplayObjects;

public class TooltipHoverHandler : MonoBehaviour
{

    [SerializeField]
    private UITooltipDetector m_uITooltipDetector;

    [SerializeField]
    private UITooltipPopup m_uiTooltipPopup;

    private ActionID m_actionId = new ActionID();


    private void Update()
    {
        int linkIndex = TMP_TextUtilities.FindIntersectingLink(m_uiTooltipPopup.TextField, Input.mousePosition,
            m_uiTooltipPopup.Canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : m_uiTooltipPopup.Canvas.worldCamera);
        if (linkIndex != -1)
        {
            if (!m_uITooltipDetector.IsPointerOverElement)
            {
                m_uITooltipDetector.enabled = true;
                TMP_LinkInfo linkInfo = m_uiTooltipPopup.TextField.textInfo.linkInfo[linkIndex];
                m_actionId.ID = int.Parse(linkInfo.GetLinkID());
                m_uITooltipDetector.SetText(GameDataSource.Instance.GetActionPrototypeByID(m_actionId).Config.ReturnDetailedTooltip());
                m_uITooltipDetector.OnPointerEnterProcessing();
            }
        }
        else if (m_uITooltipDetector.enabled == true)
        {
            m_uITooltipDetector.enabled = false;
        }

    }
}
