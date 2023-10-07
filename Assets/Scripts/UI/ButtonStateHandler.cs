using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonStateHandler : MonoBehaviour
{
    [SerializeField] List<GameObject> buttonList;
    [Header("Interactable")]
    [SerializeField] Sprite interactableButton_Sprite;
    //[SerializeField] Image interactableButton_PressedImage;
    [SerializeField] TMP_FontAsset interactableFont;
    //[Header("Uninteractable")]
    //[SerializeField] Image uninteractableButton_Image;
    //[SerializeField] Image uninteractableButton_Pressed;
    //[SerializeField] TMP_FontAsset uninteractableFont;

    public void SetLevelButtons(int levelProgress)
    {
        CheckProgress(levelProgress);
    }

    private void CheckProgress(int levelProgress)
    {
        // levelProgress variable starts from 0.
        for (int i = 0; i < buttonList.Count; i++)
        {
            if (i < levelProgress)
            {
                buttonList[i].GetComponent<Button>().interactable = true;
                SetButtonProperties(buttonList[i]);
            }
            else
            {
                buttonList[i].GetComponent<Button>().interactable = false;
            }
        }
    }

    private void SetButtonProperties(GameObject buttonGO)
    {
        TextMeshProUGUI tmPro = buttonGO.GetComponentInChildren<TextMeshProUGUI>();
        tmPro.font = interactableFont;

        Image image = buttonGO.GetComponent<Image>();
        image.sprite = interactableButton_Sprite;
    }
}
