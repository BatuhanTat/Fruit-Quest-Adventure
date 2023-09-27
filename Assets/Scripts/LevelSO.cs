using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class LevelSO : ScriptableObject
{
    public List<GemSO> gemList;
    public int width;
    public int height;
    public List<LevelGridPosition> levelGridPositionList;
    public GoalType goalType;
    public int moveAmount;
    public int targetScore;

    public enum GoalType
    {
        Score,
        Glass,    
    }

    [System.Serializable]
    public class LevelGridPosition
    {
        public GemSO gemSO;
        public int x;
        public int y;
        public bool hasGlass;
    }
}
