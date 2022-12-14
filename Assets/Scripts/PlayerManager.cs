using UnityEngine;

public class PlayerManager : MonoBehaviour
{

    [HideInInspector] public int isMale;
    [HideInInspector] public int[,,,] MAP_INT;
    [HideInInspector] public int lastSkinUsed;
    [HideInInspector] public int lastMapUsed;
    [HideInInspector] public int lastDivisionUsed;
    [HideInInspector] public int lastRoundStepUsed;
    [HideInInspector] public int unlockedSkins;
    [HideInInspector] public int unlockedMaps;
    [HideInInspector] public int level;
    [HideInInspector] public float levelEXP;
    [HideInInspector] public float nextLevelEXP;

    public void NewPlayer(int[,,,] _mAP_INT)
    {

        isMale = 0;
        MAP_INT = _mAP_INT;
        lastSkinUsed = 0;
        lastMapUsed = 0;
        lastDivisionUsed = 0;
        lastRoundStepUsed = 0;
        unlockedSkins = 0;
        unlockedMaps = 0;
        level = 1;
        levelEXP = 0f;
        nextLevelEXP = 0f;

        SavePlayer();

    }

    public void SavePlayer()
    {

        Database.SavePlayer(this);

    }

    public void LoadPlayer()
    {

        PlayerModel playerManager = Database.LoadPlayer();

        isMale = playerManager.isMale;
        MAP_INT = playerManager.MAP_INT;
        lastSkinUsed = playerManager.lastSkinUsed;
        lastMapUsed = playerManager.lastMapUsed;
        lastDivisionUsed = playerManager.lastDivisionUsed;
        lastRoundStepUsed = playerManager.lastRoundStepUsed;
        unlockedSkins = playerManager.unlockedSkins;
        unlockedMaps = playerManager.unlockedMaps;
        level = playerManager.level;
        levelEXP = playerManager.levelEXP;
        nextLevelEXP = playerManager.nextLevelEXP;

    }

}
