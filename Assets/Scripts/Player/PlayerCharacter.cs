using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PlayerCharacter : Character
{
    [Header("Level & Exp")]
    [SerializeField] private int currentExp = 0;
    public int CurrentExp { get { return currentExp; } }
    [SerializeField] private int level = 1;
    public int Level { get { return level; } }

    [SerializeField] private int expToNextLevel = 100;
    public int ExpToNextLevel { get { return expToNextLevel; } }
    


    private float levelUpMultiplier = 1.5f; // if want to scale lvl. with linear function

    [Header("Gold")]
    [SerializeField] private int gold = 0;
    public int Gold { get { return gold; } }


    [Header("Attack")]
    private PlayerController playerController;

    [Header("Status")]
    [SerializeField] private int statusPoint;
    public int StatusPoint { get { return statusPoint; } }
    [SerializeField] private int statusPointLeft;
    public int StatusPointLeft 
    { 
        get { return statusPointLeft; }
        set { statusPointLeft = value; }
    }
    [SerializeField] private int basePrice = 5;
    [SerializeField] private int hpPoint;
    public int HpPoint 
    { 
        get 
        { return hpPoint; }
        set
        {
            hp = value;
        }
    }
    [SerializeField] private int atkPoint;
    public int AtkPoint
    {
        get
        { return atkPoint; }
        set
        {
            atkPoint = value;
        }
    }
    [SerializeField] private int movementPoint;
    public int MovementPoint
    {
        get
        { return movementPoint; }
        set
        {
            movementPoint = value;
        }
    }



    public override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody2D>();
        playerController = gameObject.GetComponent<PlayerController>();
        if (PlayerPrefs.HasKey("statusPointLeft"))
        {
            statusPointLeft = PlayerPrefs.GetInt("statusPointLeft");
        }
        else
        {
            statusPointLeft = statusPoint;
            PlayerPrefs.SetInt("statusPointLeft", statusPointLeft);
        }
    }
    private void Update()
    {
        
    }

    [SerializeField] private float moveSpeedMultiplier = 1;
    public float MoveSpeedMultiplier
    {
        get { return moveSpeedMultiplier; }
        set
        {
            moveSpeedMultiplier = value; // ex. 1.1 , 1.25 , 2.5
            playerController.UpdateMoveSpeedMultiplier();
        }
    }




    #region Exp & Level
    private int CalculateExpForLevel(int targetLevel)
    {
        return (int)(100 * Mathf.Pow(targetLevel, 2));
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
            level++;
            currentExp -= expToNextLevel;
            expToNextLevel = CalculateExpForLevel(level + 1);
            Console.WriteLine($"Player leveled up to Level {level}!");

            // TO-DO Trigger level-up effects, stat increases, etc.
        }
    }

    #endregion

    public int GetGold()
    {
        return gold;
    }
    public void AddGold(int g)
    {
        gold += g;
    }

    public override void Dead()
    {
        isDead = true;
        hp = 0;
        Debug.Log($"{characterName} is dead!");
        // to-do add effect
        playerController.DeadAnimation();
    }

    #region Player Status System
    public bool TryBuyUpgrade(string upgrade, int value)
    {
        if(StatusPointLeft < value) return false;

        int price;
        if(upgrade == "hp")
        {
            //baseValue = HpPoint - value;
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
        maxHp += HpPoint * 10;
    }
    void UpdateAtkStatus(int value)
    {
        AtkPoint += value;
        statusPointLeft -= value;
        atk += AtkPoint * 10;
    }
    void UpdataMovementStatus(int value)
    {
        MovementPoint += value;
        statusPointLeft -= value;
        moveSpeed *= moveSpeedMultiplier + value / 10;
    }

    public bool ResetStatus()
    {
        if (gold < CalculateResetPrice()) return false;

        StatusPointLeft = StatusPoint;
        int hpRefund = CalculateUpgradePrice(1, HpPoint);
        int atkRefund = CalculateUpgradePrice(1, AtkPoint);
        int movementRefund = CalculateUpgradePrice(1, MovementPoint);
        int allRefund = hpRefund + atkRefund + movementRefund;
        gold += allRefund;
        HpPoint = 0;
        AtkPoint = 0;
        MovementPoint = 0;

        return true;
    }
    public int CalculateResetPrice()
    {
        int value = StatusPoint - StatusPointLeft;
        int cal = value * 50;
        return cal;
    }
    #endregion

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
}
