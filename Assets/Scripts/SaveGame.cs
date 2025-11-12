using UnityEngine;

public static class SaveGame
{
    public static void SavePlayerData(PlayerCharacter player)
    {
        PlayerPrefs.SetInt("Player_Level", player.Level);
        PlayerPrefs.SetInt("Player_Exp", player.CurrentExp);
        PlayerPrefs.SetInt("Player_ExpToNextLevel", player.ExpToNextLevel);
        PlayerPrefs.SetInt("Player_Gold", player.Gold);
        PlayerPrefs.SetInt("Player_StatusPoint", player.StatusPoint);
        PlayerPrefs.SetInt("Player_StatusPointLeft", player.StatusPointLeft);
        PlayerPrefs.SetInt("Player_HpPoint", player.HpPoint);
        PlayerPrefs.SetInt("Player_AtkPoint", player.AtkPoint);
        PlayerPrefs.SetInt("Player_MovementPoint", player.MovementPoint);
        PlayerPrefs.SetFloat("Player_MoveSpeedMultiplier", player.MoveSpeedMultiplier);

        PlayerPrefs.Save();
        Debug.Log("Player data saved via SaveGame.");
    }

    public static void LoadPlayerData(PlayerCharacter player)
    {
        if (!PlayerPrefs.HasKey("Player_Level"))
        {
            Debug.Log("No save data found.");
            return;
        }

        player.Level = PlayerPrefs.GetInt("Player_Level");
        player.CurrentExp = PlayerPrefs.GetInt("Player_Exp");
        player.ExpToNextLevel = PlayerPrefs.GetInt("Player_ExpToNextLevel");
        player.Gold = PlayerPrefs.GetInt("Player_Gold");
        player.StatusPoint = PlayerPrefs.GetInt("Player_StatusPoint");
        player.StatusPointLeft = PlayerPrefs.GetInt("Player_StatusPointLeft");
        player.HpPoint = PlayerPrefs.GetInt("Player_HpPoint");
        player.AtkPoint = PlayerPrefs.GetInt("Player_AtkPoint");
        player.MovementPoint = PlayerPrefs.GetInt("Player_MovementPoint");
        player.MoveSpeedMultiplier = PlayerPrefs.GetFloat("Player_MoveSpeedMultiplier");

        Debug.Log("Player data loaded via SaveGame.");
    }

    public static void ClearAllData()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("All save data cleared.");
    }
}
