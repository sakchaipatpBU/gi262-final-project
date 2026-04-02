using UnityEngine;

public class HighOrcMelee : MeleeEnemy
{
    private bool isRecordAnalytics = false;
    public override void Dead()
    {
        base.Dead();

        if(isRecordAnalytics) return;

        // high orc dead
        GameAnalyticsService.Instance.LogBossBattleEndGame(new BossBattleData
        {
            BossName = characterName,
            Result = BattleResult.Win,
            PlayerLevel = playerCharacter.Level,
            PlayerAtkPoint = playerCharacter.AtkPoint,
            PlayerHpPoint = playerCharacter.HpPoint,
            PlayerMoveSpeedPoint = playerCharacter.MovementPoint,
            AttemptCount = PlayerPrefs.GetInt("attemptCount")
        });
        isRecordAnalytics = true;
        // reset attemptCount
        PlayerPrefs.SetInt("attemptCount", 0);
        
    }

    public override void PerformAttack(Character target)
    {
        if (isDead) return;
        if (target == null) return;

        float distance = Vector2.Distance(transform.position, target.transform.position);
        if (distance <= attackRange)
        {
            Debug.Log($"{characterName} attack {target.characterName} with {atk}");
            target.gameObject.GetComponent<PlayerController>().SetHitDirection(transform.position);
            target.TakeDamage(atk);

            // player dead
            if (target.IsDead && !isRecordAnalytics)
            {
                // add attemptCount
                int attemptCount = PlayerPrefs.GetInt("attemptCount");
                attemptCount++;
                PlayerPrefs.SetInt("attemptCount", attemptCount);

                // send analytics >> Lose
                GameAnalyticsService.Instance.LogBossBattleEndGame(new BossBattleData
                {
                    BossName = characterName,
                    Result = BattleResult.Lose,
                    PlayerLevel = playerCharacter.Level,
                    PlayerAtkPoint = playerCharacter.AtkPoint,
                    PlayerHpPoint = playerCharacter.HpPoint,
                    PlayerMoveSpeedPoint = playerCharacter.MovementPoint,
                    AttemptCount = PlayerPrefs.GetInt("attemptCount")
                });
                isRecordAnalytics = true;
            }
        }
        else
        {
            Debug.Log($"{characterName} attack {target.characterName} , But Not Hit");
        }
        SoundManager.Instance.PlaySFX("Sword_Hit", 0.3f);
    }
}
