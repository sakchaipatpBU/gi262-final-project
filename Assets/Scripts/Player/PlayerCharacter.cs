using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCharacter : Character
{
    private PlayerController playerController;

    [Header("Level & Exp")]
    [SerializeField] private int currentExp = 0;
    public int CurrentExp { get { return currentExp; } set { currentExp = value; } }
    [SerializeField] private int level = 1;
    public int Level { get { return level; } set { level = value; } }
    [SerializeField] private int expToNextLevel = 100;
    public int ExpToNextLevel { get { return expToNextLevel; } set { expToNextLevel = value; } }

    [Header("Gold")]
    [SerializeField] private int gold = 0;
    public int Gold { get { return gold; } set { gold = value; } }
    
    [Header("Status Point")]
    [SerializeField] private int basePrice = 5;
    [SerializeField] private int combatScore;
    public int CombatScore {  get { return combatScore; } set { combatScore = value; } }
    [SerializeField] private int statusPoint;
    public int StatusPoint { get { return statusPoint; } set { statusPoint = value; } }
    [SerializeField] private int statusPointLeft;
    public int StatusPointLeft {  get { return statusPointLeft; } set { statusPointLeft = value; } }
    [SerializeField] private int hpPoint;
    public int HpPoint {  get { return hpPoint; } set { hpPoint = value; } }

    [SerializeField] private int atkPoint;
    public int AtkPoint {  get { return atkPoint; } set { atkPoint = value; } }

    [SerializeField] private int movementPoint;
    public int MovementPoint { get { return movementPoint; } set { movementPoint = value; } }

    [SerializeField] private float moveSpeedMultiplier = 1;
    public float MoveSpeedMultiplier
    {
        get { return moveSpeedMultiplier; }
        set
        {
            moveSpeedMultiplier = value; // ex. 1.1 , 1.25 , 2.5
        }
    }

    [SerializeField] private float baseMaxHp;
    [SerializeField] private float baseAtk;
    [SerializeField] private float baseMovement;

    [Header("Fury Mode")]
    [SerializeField]
    private int fury;
    public int Fury { get { return fury; } set { fury = value; } }
    [SerializeField]
    private SpriteRenderer spriteRenderer;
    [SerializeField]
    private Image furyFillBar;
    [SerializeField]
    private TMP_Text furyText;
    [SerializeField]
    private float furyDuration = 5f;
    [SerializeField]
    private float rainbowSpeed = 2f;

    private Coroutine furyCoroutine;
    private Color originalColor;
    private bool isFuryActive = false;

    private void Awake()
    {
        playerController = gameObject.GetComponent<PlayerController>();
        if (spriteRenderer == null)
            spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
        furyFillBar.fillAmount = 0f;
        fury = (int)(furyFillBar.fillAmount * 100);
        furyText.text = $"{fury} %";
    }
    public override void Start()
    {
        base.Start();

        SaveGame.LoadPlayerData(this);
        expToNextLevel = CalculateExpForLevel(level + 1);
        UpdateAllPlayerStatus();

    }
    public override bool TakeDamage(float damage)
    {
        if (isDead) return true;

        hp -= damage;
        Debug.Log($"{characterName} got {damage} damage. Now {hp} / {maxHp} hp.");

        if (hp <= 0)
        {
            Dead();
            return true;
        }
        playerController.GetHitAnimation();

        return false;
    }
    public override void Dead()
    {
        isDead = true;
        hp = 0;
        Debug.Log($"{characterName} is dead!");
        playerController.DeadAnimation();
        GameManager.Instance.GaveOver();

        SoundManager.Instance.PlaySFX("Dead_Player", 0.3f);
    }
    public void AddGold(int g)
    {
        gold += g;
    }

    #region Exp & Level
    private int CalculateExpForLevel(int targetLevel)
    {
        return (int)(10 * Mathf.Pow(targetLevel, 2));
    }
    public void AddExperience(int amount)
    {
        currentExp += amount;
        CheckForLevelUp();
    }
    private void CheckForLevelUp()
    {
        while (currentExp >= expToNextLevel)
        {
            SoundManager.Instance.PlaySFX("LevelUp", 0.3f);

            level++;
            statusPoint++;
            statusPointLeft++;
            currentExp -= expToNextLevel;
            expToNextLevel = CalculateExpForLevel(level + 1);
            Console.WriteLine($"Player leveled up to Level {level}!");
        }
    }
    #endregion

    #region Player Status System
    public bool TryBuyUpgrade(string upgrade, int value)
    {
        if(StatusPointLeft < value) return false;

        int price;
        if(upgrade == "hp")
        {
            price = CalculateUpgradePrice(HpPoint, HpPoint + value);
            if(gold > price)
            {
                BuyUpgrade(price);
                UpdateHpStatus(value);
                return true;
            }
        }
        else if(upgrade == "atk")
        {
            price = CalculateUpgradePrice(AtkPoint, AtkPoint + value);
            if (gold > price)
            {
                BuyUpgrade(price);
                UpdateAtkStatus(value);
                return true;
            }
        }
        else if (upgrade == "movement")
        {
            price = CalculateUpgradePrice(movementPoint, movementPoint + value);
            if (gold > price)
            {
                BuyUpgrade(price);
                UpdataMovementStatus(value);
                return true;
            }
        }
        return false;
    }
    public int CalculateUpgradePrice(int _baseValue, int targetValue)
    {
        if(targetValue <= _baseValue)
        {
            return 0;
        }
        return (int)(basePrice * math.pow(targetValue - 1, 2)) + 
            CalculateUpgradePrice(_baseValue, targetValue - 1);
    }
    // upgrade status point form 1 to 3
    // 3 is not unclude -> current status -> not upgrade yet
    // baseValue = 1 , target = 3 , basePrice = 5  
    // Price = (5 * 2^2) + (5 * 1^2) + 0
    // Price =    20    +    10      + 0
    // Price =  30
    void BuyUpgrade(int price)
    {
        gold -= price;
    }
    void UpdateHpStatus(int value)
    {
        HpPoint += value;
        statusPointLeft -= value;
        UpdateAllPlayerStatus();
    }
    void UpdateAtkStatus(int value)
    {
        AtkPoint += value;
        statusPointLeft -= value;
        UpdateAllPlayerStatus();
    }
    void UpdataMovementStatus(int value)
    {
        MovementPoint += value;
        statusPointLeft -= value;
        UpdateAllPlayerStatus();
    }
    void UpdateAllPlayerStatus()
    {
        maxHp = baseMaxHp + HpPoint * 10;
        if(hp > maxHp) hp = maxHp;
        atk = baseAtk + AtkPoint * 10;
        MoveSpeedMultiplier = 1 + ((float)MovementPoint / 10);
        moveSpeed = baseMovement * moveSpeedMultiplier;
        playerController.UpdateMoveSpeed();
        combatScore = (HpPoint * 10) + (AtkPoint * 10) + (MovementPoint * 2);
    }

    public bool ResetStatus()
    {
        if (gold < CalculateResetPrice()) return false;

        gold -= CalculateResetPrice();
        StatusPointLeft = StatusPoint;
        HpPoint = 0;
        AtkPoint = 0;
        MovementPoint = 0;

        UpdateAllPlayerStatus();

        return true;
    }
    public int CalculateResetPrice()
    {
        int value = StatusPoint - StatusPointLeft;
        int cal = value * 50;
        return cal;
    }
    public void GainHp(int _hp)
    {
        hp += _hp;
        if(hp > maxHp) hp = maxHp;
    }
    #endregion

    #region Fury Mode
    private bool CheckFury()
    {
        if (fury >= 100)
        {
            fury = 100;
            return true;
        }
        else return false;
    }

    private void ActivateFuryMode()
    {
        if (isFuryActive)
            return;

        furyCoroutine = StartCoroutine(FuryModeRoutine());
    }

    private IEnumerator FuryModeRoutine()
    {
        isFuryActive = true;

        // Apply Buff
        ApplyBuff(true);
        SoundManager.Instance.PlaySFX("buff", 0.3f);


        float timer = furyDuration;

        while (timer > 0)
        {
            timer -= Time.deltaTime;

            // Rainbow Color Effect
            float hue = Mathf.PingPong(Time.time * rainbowSpeed, 1f);
            spriteRenderer.color = Color.HSVToRGB(hue, 1f, 1f);

            // UI Fill Bar Countdown
            furyFillBar.fillAmount = timer / furyDuration;

            fury = (int)(furyFillBar.fillAmount * 100);
            furyText.text = $"{fury} %";

            yield return null;
        }

        // Reset everything
        spriteRenderer.color = originalColor;
        furyFillBar.fillAmount = 0f;

        ApplyBuff(false);

        isFuryActive = false;
    }
    private void ApplyBuff(bool enable)
    {
        if (enable)
        {
            atk *= 1.5f;
            moveSpeed *= 1.5f;
            playerController.UpdateMoveSpeed();
        }
        else
        {
            atk /= 1.5f;
            moveSpeed /= 1.5f;
            playerController.UpdateMoveSpeed();
        }
    }

    public void AddFury(int f)
    {
        if (isFuryActive)
            return;

        fury += f;
        if (CheckFury()) ActivateFuryMode();
        furyFillBar.fillAmount = (float)fury / 100f;
        furyText.text = $"{fury} %";
    }

    #endregion Fury Mode
}