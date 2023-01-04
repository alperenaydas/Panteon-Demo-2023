using System.Collections.Generic;
using UnityEngine;

public class ContentScroll : MonoBehaviour
{
    [SerializeField] private RectTransform m_RectTransform;
    [SerializeField] private List<RectTransform> m_TestImages;
    
    private int m_TestImagesLength;

    private int m_LastTopIndex;
    private int m_LastBottomIndex;

    private bool m_IsPositiveScroll;
    private int m_PositiveCheckYPos;

    private void Start()
    {
        m_TestImagesLength = m_TestImages.Count;
        m_LastTopIndex = 0;
        m_LastBottomIndex = m_TestImagesLength - 1;
        m_PositiveCheckYPos = (int)m_RectTransform.anchoredPosition.y;
    }

    private void Update()
    {
        var currentYPos = (int)m_RectTransform.anchoredPosition.y + 25; //25 is spacing value of vertical layout group
        if (currentYPos != m_PositiveCheckYPos)
        {
            ScrollCheck(currentYPos);
        }
    }

    private void ScrollCheck(int currentYPos)
    {
        m_IsPositiveScroll = currentYPos > m_PositiveCheckYPos;
        m_PositiveCheckYPos = currentYPos;
        var lastExpectedTopIndex = currentYPos / 2025; //2025 is sprite group height + spacing
        if (currentYPos < 0) lastExpectedTopIndex -= 1;
        lastExpectedTopIndex %= m_TestImagesLength;
        if (lastExpectedTopIndex < 0) lastExpectedTopIndex += m_TestImagesLength; //negative modulo test
        if (lastExpectedTopIndex != m_LastTopIndex)
        {
            RecycleItems();
        }
    }

    private void RecycleItems()
    {
        if (m_IsPositiveScroll)
        {
            m_TestImages[m_LastTopIndex].anchoredPosition += Vector2.up * (-2025 * m_TestImagesLength); //2025 is sprite group height + spacing
            m_LastBottomIndex = m_LastTopIndex;
            m_LastTopIndex = (m_LastTopIndex+1) % m_TestImagesLength;
        }
        else
        {
            m_TestImages[m_LastBottomIndex].anchoredPosition += Vector2.up * (2025 * m_TestImagesLength); //2025 is sprite group height + spacing
            m_LastTopIndex = m_LastBottomIndex;
            m_LastBottomIndex = (m_LastBottomIndex-1) % m_TestImagesLength;
            if (m_LastBottomIndex < 0) m_LastBottomIndex += m_TestImagesLength; //negative modulo test
        }
    }
    
}