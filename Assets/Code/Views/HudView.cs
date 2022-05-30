using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Snowboard.Views
{
    public class HudView : MonoBehaviour
    {
        [SerializeField] private GameObject progressBlock;

        [SerializeField] private Image progressFill;

        [SerializeField] private GameObject title;

        [SerializeField] private TMP_Text positionText;

        [SerializeField] private TMP_Text finalText;


        private int previousPosition = 0;

        private const string winMessage = "You Win!";
        private const string looseMessage = "Better luck next time!";


        public float Progress { set { progressFill.fillAmount = value; } }

        public void ShowProgress(bool show)
        {
            progressBlock.SetActive(show);
        }

        public void ShowTitle(bool show)
        {
            title.SetActive(show);
        }

        public void SetPosition(int position, int total)
        {
            if (position != previousPosition)
            {
                previousPosition = position;
                positionText.text = $"{position} | {total}";
            }
        }

        public void ShowFinalMessage(bool show)
        {
            finalText.gameObject.SetActive(show);
            finalText.text = previousPosition == 1 ? winMessage : looseMessage;
        }
    }
}