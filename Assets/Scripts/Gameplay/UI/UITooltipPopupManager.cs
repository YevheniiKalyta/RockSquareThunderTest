using System.Collections.Generic;
using Unity.BossRoom.Gameplay.UI;
using UnityEngine;
using UnityEngine.Pool;

public class UITooltipPopupManager : MonoBehaviour
{
    [SerializeField] private UITooltipPopup m_tooltipPrefab;
    private ObjectPool<UITooltipPopup> m_tooltipsPool;
    private Stack<(UITooltipDetector, UITooltipPopup)> m_popupStack = new Stack<(UITooltipDetector, UITooltipPopup)>();

    private void Awake()
    {
        m_tooltipsPool = new ObjectPool<UITooltipPopup>(
            createFunc: CreateTooltip,
            actionOnGet: OnGetTooltip,
            actionOnRelease: OnReleaseTooltip,
            defaultCapacity: 10,
            maxSize: 20
        );
    }

    private void Update()
    {
        if (m_popupStack.Count > 0)
        {
            var a = m_popupStack.Peek();
            a.Item1.TryHideTooltip();
        }
    }

    private UITooltipPopup CreateTooltip()
    {
        return Instantiate(m_tooltipPrefab, transform);
    }

    private void OnGetTooltip(UITooltipPopup tooltip)
    {
        tooltip.gameObject.SetActive(true);
    }

    private void OnReleaseTooltip(UITooltipPopup tooltip)
    {
        tooltip.gameObject.SetActive(false);
    }

    public UITooltipPopup GetTooltip()
    {
        return m_tooltipsPool.Get();
    }

    public void ReturnTooltip(UITooltipPopup tooltip)
    {
        m_tooltipsPool.Release(tooltip);
    }

    internal void ShowTooltip(UITooltipDetector uITooltipDetector, string m_TooltipText, Vector3 mousePosition, out UITooltipPopup uITooltipPopup)
    {
        uITooltipPopup = GetTooltip();
        uITooltipPopup.ShowTooltip(m_TooltipText, Input.mousePosition);
        m_popupStack.Push((uITooltipDetector, uITooltipPopup));
    }

    internal bool HideTooltip(UITooltipPopup uITooltipPopup)
    {
        if (m_popupStack.Count > 0)
        {
            (UITooltipDetector, UITooltipPopup) lastPopup = m_popupStack.Peek();
            if (lastPopup.Item2 == uITooltipPopup)
            {
                if ((!lastPopup.Item2.IsPersistent || !lastPopup.Item2.IsPointerOverTooltip) && !lastPopup.Item1.IsPointerOverElement)
                {
                    lastPopup.Item2.HideTooltip();
                    m_tooltipsPool.Release(lastPopup.Item2);
                    m_popupStack.Pop();
                    return true;
                }
            }
        }
        return false;
    }
}
