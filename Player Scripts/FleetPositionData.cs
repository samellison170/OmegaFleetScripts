[System.Serializable]  // Makes the class visible in the Unity Inspector
public class FleetPositionData
{
    //position in fleet
    public int index;
    //is the postion being used
    public bool isUsed;

    // Constructor
    public FleetPositionData(int index, bool isUsed)
    {
        this.index = index;
        this.isUsed = isUsed;
    }
}