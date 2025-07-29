using TMPro;
using UnityEngine;

namespace Project.Scripts.SaveSystem.SaveSystemLogic
{
    public class SaveSystemView
    {
        [SerializeField] private TextMeshPro moneyText;
        public void View(GameData gameData)
        {
            moneyText.text = gameData.Moneys.ToString();
        }
    }
}