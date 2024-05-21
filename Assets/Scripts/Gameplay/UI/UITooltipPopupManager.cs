using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.BossRoom.Gameplay.UI;
using UnityEngine;
using UnityEngine.Pool;

public class UITooltipPopupManager : MonoBehaviour
{
    [SerializeField] private UITooltipPopup tooltipPrefab;
    private ObjectPool<UITooltipPopup> tooltipsPool;
    private Stack<(UITooltipDetector, UITooltipPopup)> popupStack = new Stack<(UITooltipDetector, UITooltipPopup)>();

    private void Awake()
    {
        tooltipsPool = new ObjectPool<UITooltipPopup>(
            createFunc: CreateTooltip,
            actionOnGet: OnGetTooltip,
            actionOnRelease: OnReleaseTooltip,
            defaultCapacity: 10,
            maxSize: 20
        );
        StartCoroutine(TryHideTooltips());
    }

    private IEnumerator TryHideTooltips()
    {
        while (true)
        {
            yield return new WaitForEndOfFrame();
            if (popupStack.Count > 0)
            {
                var a = popupStack.Peek();
                a.Item1.Hide();
            }
        }

    }

    private UITooltipPopup CreateTooltip()
    {
        return Instantiate(tooltipPrefab, transform);
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
        return tooltipsPool.Get();
    }

    public void ReturnTooltip(UITooltipPopup tooltip)
    {
        tooltipsPool.Release(tooltip);
    }

    internal void ShowTooltip(UITooltipDetector uITooltipDetector, string m_TooltipText, Vector3 mousePosition, out UITooltipPopup uITooltipPopup)
    {
        uITooltipPopup = GetTooltip();
        uITooltipPopup.ShowTooltip(m_TooltipText, Input.mousePosition);
        popupStack.Push((uITooltipDetector, uITooltipPopup));
    }

    internal bool HideTooltip(UITooltipPopup uITooltipPopup)
    {
        if (popupStack.Count > 0)
        {
            (UITooltipDetector, UITooltipPopup) lastPopup = popupStack.Peek();
            if (lastPopup.Item2 == uITooltipPopup)
            {
                if ((!lastPopup.Item2.IsPersistent || !lastPopup.Item2.IsPointerOverTooltip) && !lastPopup.Item1.IsPointerOverElement)
                {
                    lastPopup.Item2.HideTooltip();
                    tooltipsPool.Release(lastPopup.Item2);
                    popupStack.Pop();
                    return true;
                }
            }
        }
        return false;
    }
}
