using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Unity.BossRoom.Gameplay.UI
{
    /// <summary>
    /// This controls the tooltip popup -- the little text blurb that appears when you hover your mouse
    /// over an ability icon.
    /// </summary>
    public class UITooltipPopup : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField]
        private Canvas m_Canvas;
        [SerializeField]
        [Tooltip("This transform is shown/hidden to show/hide the popup box")]
        private GameObject m_WindowRoot;
        [SerializeField]
        private TextMeshProUGUI m_TextField;
        [SerializeField]
        private UnityEngine.UI.Image m_fillImage;
        [SerializeField]
        private Vector3 m_CursorOffset;

        public bool IsPointerOverTooltip = false;
        public Canvas Canvas => m_Canvas;
        public TextMeshProUGUI TextField => m_TextField;

        private RectTransform m_RectTransform;
        private bool m_IsPersistent;

        public bool IsPersistent => m_IsPersistent;

        private void Awake()
        {
            Assert.IsNotNull(m_Canvas);
            Assert.IsNotNull(m_WindowRoot);
            m_RectTransform = m_WindowRoot.transform as RectTransform;
        }

        /// <summary>
        /// Shows a tooltip at the given mouse coordinates.
        /// </summary>
        public void ShowTooltip(string text, Vector3 screenXy)
        {
            m_TextField.text = text;
            m_WindowRoot.SetActive(true);
            RepositionTooltip(screenXy);
        }

        public void SetFillImage(float amount)
        {
            if (!m_IsPersistent)
            {
                m_fillImage.fillAmount = amount;
            }
        }

        public void SetPersistent(bool on)
        {
            m_IsPersistent = on;
            m_fillImage.raycastTarget = on;
        }

        private void RepositionTooltip(Vector3 screenXy)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(m_RectTransform);
            screenXy += m_CursorOffset;

            // Get the tooltip dimensions
            Vector2 sizeDelta = m_RectTransform.sizeDelta;
            Vector3 finalScale = new Vector3(sizeDelta.x * transform.lossyScale.x, sizeDelta.y * transform.lossyScale.y);

            // Adjust the position if the tooltip exceeds the screen boundaries
            if (screenXy.x - finalScale.x < 0)
            {
                screenXy.x = finalScale.x;
            }
            if (screenXy.y - finalScale.y < 0)
            {
                screenXy.y = finalScale.y;
            }

            m_WindowRoot.transform.position = GetCanvasCoords(screenXy);
        }

        /// <summary>
        /// Hides the current tooltip.
        /// </summary>
        public void HideTooltip()
        {
            SetPersistent(false);
            m_WindowRoot.SetActive(false);
        }

        /// <summary>
        /// Maps screen coordinates (e.g. Input.mousePosition) to coordinates on our Canvas.
        /// </summary>
        private Vector3 GetCanvasCoords(Vector3 screenCoords)
        {
            Vector2 canvasCoords;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                m_Canvas.transform as RectTransform,
                screenCoords,
                m_Canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : m_Canvas.worldCamera,
                out canvasCoords);
            return m_Canvas.transform.TransformPoint(canvasCoords);
        }



#if UNITY_EDITOR
        private void OnValidate()
        {
            if (gameObject.scene.rootCount > 1) // Hacky way for checking if this is a scene object or a prefab instance and not a prefab definition.
            {
                if (!m_Canvas)
                {
                    // typically there's only one canvas in the scene, so pick that
                    m_Canvas = FindObjectOfType<Canvas>();
                }
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
           IsPointerOverTooltip = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            IsPointerOverTooltip = false;
        }
#endif

    }
}
