using UnityEngine;

public class PlayerManager : MonoBehaviour
{

    [HideInInspector] public bool isMale;
    [HideInInspector] public int[,,] MAP_INT;
    [HideInInspector] public int lastSkinUsed;
    [HideInInspector] public int lastMapUsed;
    [HideInInspector] public int lastMapDivisionUsed;
    [HideInInspector] public int lastMapRoundStepUsed;
    [HideInInspector] public int unlockedSkins;
    [HideInInspector] public int unlockedMaps;

    public void NewPlayer(int[,,] _mAP_INT)
    {

        isMale = false;
        MAP_INT = _mAP_INT;
        lastSkinUsed = 0;
        lastMapUsed = 0;
        lastMapDivisionUsed = 0;
        lastMapRoundStepUsed = 0;
        unlockedSkins = 0;
        unlockedMaps = 0;

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
        lastMapDivisionUsed = playerManager.lastMapDivisionUsed;
        lastMapRoundStepUsed = playerManager.lastMapRoundStepUsed;
        unlockedSkins = playerManager.unlockedSkins;
        unlockedMaps = playerManager.unlockedMaps;

    }

}
