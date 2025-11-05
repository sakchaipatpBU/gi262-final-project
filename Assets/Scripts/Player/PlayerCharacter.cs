using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : Character
{
    [Header("Level & Exp")]
    [SerializeField] private int currentExp = 0;
    [SerializeField] private int level = 1;
    [SerializeField] private int expToNextLevel = 100;
    private float levelUpMultiplier = 1.5f; // if want to scale lvl. with linear function

    [Header("Gold")]
    [SerializeField] private int gold = 0;

    [Header("Attack")]
    private PlayerController playerController;



    public override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        
    }

    private float moveSpeedMultiplier;
    public float MoveSpeedMultiplier
    {
        get {  return moveSpeedMultiplier; }
        set { moveSpeedMultiplier = value; }
    }

    public float Atk
    {
        get { return atk; }
        set { atk = value; }
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
        base.Dead();
    }
}
