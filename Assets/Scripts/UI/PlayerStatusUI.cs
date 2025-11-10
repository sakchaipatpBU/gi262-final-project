using System.Collections;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerStatusUI : MonoBehaviour
{
    public GameObject playerStatusPanel;
    public GameObject resetPanel;
    public GameObject failPanel;

    [Header("Main")]
    public Image expBar;
    public TMP_Text expText;
    public TMP_Text playerLevelText;
    public Image hpBar;
    public TMP_Text hpText;

    [Header("Text")]
    public TMP_Text coinText;
    public TMP_Text hpStatusText;
    public TMP_Text hpPriceText;
    public TMP_Text atkStatusText;
    public TMP_Text atkPriceText;
    public TMP_Text movementStatusText;
    public TMP_Text movementPriceText;
    public TMP_Text pointLeftText;
    public TMP_Text spendingPointText;
    public int spendingPoint;
    public TMP_Text failText;
    public TMP_Text resetPriceText;

    /*[Header("Button")]
    public Button hpUpgradeButton;
    public Button atkUpgradeButton;
    public Button movementUpgradeButton;
    public Button resetButton;*/



    private PlayerCharacter playerCharacter;
    private PlayerController playerController;
    private InputAction statusAction;
    private bool isDisplay = false;
    private Coroutine failCoroutine;


    private void Start()
    {
        playerCharacter = GameObject.Find("Player").GetComponent<PlayerCharacter>();
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        statusAction = InputSystem.actions.FindAction("Status");
        CloseDisplayUI();
        failPanel.SetActive(false);
        resetPanel.SetActive(false);


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
        spendingPoint = 1;
        UpdateCoinTextUI();
        UpdateHpTextUI();
        UpdateAtkTextUI();
        UpdateMovementTextUI();
        UpdatePointLeftUI();
        UpDateSpendingPointUI();

        playerStatusPanel.SetActive(true);
        playerController.enabled = false;
    }
    void CloseDisplayUI()
    {
        isDisplay = false;
        playerStatusPanel.SetActive(false);
        playerController.enabled = true;
    }
    #region Main
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
    #endregion

    #region Tab - Status
    void UpDateSpendingPointUI()
    {
        spendingPointText.text = spendingPoint.ToString();
        UpdateHpPriceTextUI();
        UpdateAtkPriceTextUI();
        UpdateMovementPriceTextUI();
    }

    void UpdateCoinTextUI()
    {
        coinText.text = playerCharacter.Gold.ToString();
    }
    void UpdateHpTextUI()
    {
        hpStatusText.text = playerCharacter.HpPoint.ToString();
    }
    void UpdateHpPriceTextUI()
    {
        hpPriceText.text = playerCharacter
            .CalculateUpgradePrice(playerCharacter.HpPoint,
            playerCharacter.HpPoint + spendingPoint)
            .ToString();
    }
    void UpdateAtkTextUI()
    {
        atkStatusText.text = playerCharacter.AtkPoint.ToString();
    }
    void UpdateAtkPriceTextUI()
    {
        atkPriceText.text = playerCharacter
            .CalculateUpgradePrice(playerCharacter.AtkPoint,
            playerCharacter.AtkPoint + spendingPoint)
            .ToString();
    }
    void UpdateMovementTextUI()
    {
        movementStatusText.text = playerCharacter.MovementPoint.ToString();
    }
    void UpdateMovementPriceTextUI()
    {
        movementPriceText.text = playerCharacter
            .CalculateUpgradePrice(playerCharacter.MovementPoint,
            playerCharacter.MovementPoint + spendingPoint)
            .ToString();
    }
    void UpdatePointLeftUI()
    {
        pointLeftText.text = playerCharacter.StatusPointLeft.ToString();
    }
    

    #region Button
    public void OnAddSpendingPointClicked()
    {
        spendingPoint++;
        UpDateSpendingPointUI();
    }
    public void OnReduceSpendingPointClicked()
    {
        spendingPoint--;
        if (spendingPoint < 1) spendingPoint = 1;
        UpDateSpendingPointUI();
    }
    public void OnHpUpgradeButtonClicked()
    {
        if (playerCharacter.TryBuyUpgrade("hp", spendingPoint))
        {
            spendingPoint = 1;
            UpdateCoinTextUI();
            UpdateHpTextUI();
            UpdatePointLeftUI();
            UpDateSpendingPointUI();
        }
        else
        {
            StopAllCoroutines();
            failCoroutine = StartCoroutine(ShowFailPanel("Something wrong, Can NOT upgrade."));
        }
    }
    public void OnAtkUpgradeButtonClicked()
    {
        if (playerCharacter.TryBuyUpgrade("atk", spendingPoint))
        {
            spendingPoint = 1;
            UpdateCoinTextUI();
            UpdateAtkTextUI();
            UpdatePointLeftUI();
            UpDateSpendingPointUI();
        }
        else
        {
            StopAllCoroutines();
            failCoroutine = StartCoroutine(ShowFailPanel("Something wrong, Can NOT upgrade."));
        }
    }
    public void OnMovementUpgradeButtonClicked()
    {
        if (playerCharacter.TryBuyUpgrade("movement", spendingPoint))
        {
            spendingPoint = 1;
            UpdateCoinTextUI();
            UpdateMovementTextUI();
            UpdatePointLeftUI();
            UpDateSpendingPointUI();
        }
        else
        {
            StopAllCoroutines();
            failCoroutine = StartCoroutine(ShowFailPanel("Something wrong, Can NOT upgrade."));
        }
    }

    public void OnComfirmResetButtonClicked()
    {
        if (playerCharacter.ResetStatus())
        {
            spendingPoint = 1;
            UpdateCoinTextUI();
            UpdateHpTextUI();
            UpdateAtkTextUI();
            UpdateMovementTextUI();
            UpdatePointLeftUI();
            UpDateSpendingPointUI();
        }
        else
        {
            StopAllCoroutines();
            failCoroutine = StartCoroutine(ShowFailPanel("Something wrong, Can NOT reset your upgrade."));
        }
    }
    public void OnResetButtonClick()
    {
        resetPriceText.text = playerCharacter.CalculateResetPrice().ToString();
    }

    IEnumerator ShowFailPanel(string text)
    {
        failText.text = text;
        failPanel.SetActive(true);
        yield return new WaitForSeconds(2f);
        failPanel.SetActive(false);
    }

    #endregion
    #endregion
}
