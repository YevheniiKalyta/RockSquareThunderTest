using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Unity.BossRoom.Gameplay.UI
{
    /// <summary>
    /// Attach to any UI element that should have a tooltip popup. If the mouse hovers over this element
    /// long enough, the tooltip will appear and show the specified text.
    /// </summary>
    /// <remarks>
    /// Having trouble getting the tooltips to show up? The event-handlers use physics raycasting, so make sure:
    /// - the main camera in the scene has a PhysicsRaycaster component
    /// - if you're attaching this to a UI element such as an Image, make sure you check the "Raycast Target" checkbox
    /// </remarks>
    public class UITooltipDetector : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [SerializeField]
        private UITooltipPopupManager m_TooltipPopupManager;

        [Tooltip("The actual Tooltip that should be triggered")]
        private UITooltipPopup m_TooltipPopup;

        [SerializeField]
        [Multiline]
        [Tooltip("The text of the tooltip (this is the default text; it can also be changed in code)")]
        private string m_TooltipText;

        [SerializeField]
        [Tooltip("Should the tooltip appear instantly if the player clicks this UI element?")]
        private bool m_ActivateOnClick = true;

        [SerializeField]
        [Tooltip("The length of time the mouse needs to hover over this element before the tooltip appears (in seconds)")]
        private float m_TooltipDelay = 0.5f;

        [SerializeField]
        [Tooltip("The length of time the mouse needs to hover over this element before the tooltip appears (in seconds)")]
        private float m_PersistentTooltipDelay = -1f;

        private float m_PointerEnterTime = 0;
        private bool m_IsShowingTooltip;


        public bool IsPointerOverElement => m_PointerEnterTime > 0;

        public void SetText(string text)
        {
            m_TooltipText = text; 
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            OnPointerEnterProcessing();
        }

        public void OnPointerEnterProcessing()
        {
            m_PointerEnterTime = Time.time;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            m_PointerEnterTime = 0;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (m_ActivateOnClick)
            {
                ShowTooltip();
            }
        }

        private void OnDisable()
        {
            m_PointerEnterTime = 0;
        }

        private void Update()
        {
            if (m_PointerEnterTime != 0)
            {
                float timeSincePointerEnter = Time.time - m_PointerEnterTime;

                if (m_PersistentTooltipDelay > 0 && timeSincePointerEnter > m_TooltipDelay + m_PersistentTooltipDelay)
                {
                    m_TooltipPopup.SetPersistent(true);
                }
                else if (timeSincePointerEnter > m_TooltipDelay)
                {
                    ShowTooltip();
                    m_TooltipPopup.SetFillImage(timeSincePointerEnter / (m_TooltipDelay + m_PersistentTooltipDelay));
                }
            }
        }

        public void SetPersistent(bool on)
        {
            if (m_IsShowingTooltip)
            {
                m_TooltipPopup.SetPersistent(on);
            }
        }

        public void ShowTooltip()
        {
            if (!m_IsShowingTooltip)
            {
                m_TooltipPopupManager.ShowTooltip(this, m_TooltipText, Input.mousePosition, out m_TooltipPopup);
                m_IsShowingTooltip = true;
            }
        }

        public void Hide()
        {
            if (m_IsShowingTooltip)
            {
                if (m_TooltipPopupManager.HideTooltip(m_TooltipPopup))
                {
                    m_IsShowingTooltip = false;
                }
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (gameObject.scene.rootCount > 1) // Hacky way for checking if this is a scene object or a prefab instance and not a prefab definition.
            {
                if (!m_TooltipPopupManager)
                {
                    // typically there's only one tooltip popup in the scene, so pick that
                    m_TooltipPopupManager = FindObjectOfType<UITooltipPopupManager>();
                }
            }
        }
#endif
    }
}
