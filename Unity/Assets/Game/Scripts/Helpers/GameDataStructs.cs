using Farm.Managers;

public class GameDataStructs
{
    
}

[System.Serializable]
public class PlantInfo
{
    public string contentId;
    public long instanceId;
    public int sellingPrice;
    public int seedsToPlant;
    public int yieldAmount;
    public CropsData cropData;
    
    public bool IsReadyToPlant => seedsToPlant > 0;
    public bool IsReadyToHarvest => yieldAmount > 0;
    public bool IsOwned => instanceId != 0;
}
