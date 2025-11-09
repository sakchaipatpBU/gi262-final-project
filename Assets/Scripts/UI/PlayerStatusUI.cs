using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerStatusUI : MonoBehaviour
{
    public GameObject playerStatusPanel;
    public Image expBar;
    public TMP_Text expText;
    public TMP_Text playerLevelText;

    public Image hpBar;
    public TMP_Text hpText;

    public TMP_Text coinText;
    public TMP_Text hpStatus;
    public TMP_Text hpPrice;
    public TMP_Text atkStatus;
    public TMP_Text atkPrice;
    public TMP_Text movementStatus;
    public TMP_Text movementPrice;

    public TMP_Text pointLeft;
    public TMP_Text spendingPointText;
    public int spendingPoint;

    private PlayerCharacter playerCharacter;
    private PlayerController playerController;
    private InputAction statusAction;
    private bool isDisplay = false;


    private void Start()
    {
        playerCharacter = GameObject.Find("Player").GetComponent<PlayerCharacter>();
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        statusAction = InputSystem.actions.FindAction("Status");
        CloseDisplayUI();
        UpDateSpendingPointUI();

    }

    private void Update()
    {
        if (statusAction.triggered)
        {
            if (!isDisplay) 
            {
                DisplayUI();
            } 
            else
            {
                CloseDisplayUI();
            }
        }

        UpdateEpxUI();
        UpdateHpUI();
    }
    void DisplayUI()
    {
        isDisplay = true;
        spendingPoint = 0;
        UpDateCoinUI();
        playerStatusPanel.SetActive(true);
        playerController.enabled = false;
    }
    void CloseDisplayUI()
    {
        isDisplay = false;
        playerStatusPanel.SetActive(false);
        playerController.enabled = true;
    }
    void UpdateEpxUI()
    {
        playerLevelText.text = $"Level {playerCharacter.Level}"; 
        expText.text = $"{playerCharacter.CurrentExp}/{playerCharacter.ExpToNextLevel}";
        expBar.fillAmount = (float)playerCharacter.CurrentExp / (float)playerCharacter.ExpToNextLevel;
    }
    void UpdateHpUI()
    {
        hpText.text = playerCharacter.Hp.ToString();
        hpBar.fillAmount = (float)playerCharacter.Hp / (float)playerCharacter.MaxHp;
    }

    public void AddSpendingPoint()
    {
        spendingPoint++;
        UpDateSpendingPointUI();
    }
    public void ReduceSpendingPoint()
    {
        spendingPoint--;
        if(spendingPoint < 0) spendingPoint = 0;
        UpDateSpendingPointUI();
    }

    void UpDateSpendingPointUI()
    {
        spendingPointText.text = spendingPoint.ToString();
    }

    void UpDateCoinUI()
    {
        coinText.text = playerCharacter.Gold.ToString();
    }
}
