using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InformationTab : MonoBehaviour
{
    [SerializeField] private GameObject m_InformationParentObject;
    
    [SerializeField] private TMP_Text m_SelectedObjectNameText;
    [SerializeField] private Image m_SelectedObjectNameImage;
    [SerializeField] private TMP_Text m_SelectedObjectInfoText;
    
    public void SetObjectInformation(string objectName, Sprite objectImage, string objectInfo)
    {
        m_SelectedObjectNameText.text = objectName;
        m_SelectedObjectNameImage.sprite = objectImage;
        m_SelectedObjectInfoText.text = objectInfo;
        m_InformationParentObject.SetActive(true);
    }

    public void CloseInformationTab()
    {
        if(m_InformationParentObject.activeSelf) m_InformationParentObject.SetActive(false);
    }
}
