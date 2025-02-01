/* 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathObjectParent : MonoBehaviour
{
    public PathPoint[] CommonPathPoint;
    public PathPoint[] BluePlayerPathPoint;
    public PathPoint[] RedPlayerPathPoint;
    public PathPoint[] GreenPlayerPathPoint;
    public PathPoint[] YellowPlayerPathPoint;
    public PathPoint[] BasePoint;

    [Header("Scale and Position Difference")]
    public float[] scales;
    public float[] positionDifference;
}


*/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathObjectParentFor2Player : MonoBehaviour
{
    public PathPointFor2Player[] CommonPathPoint;
    //public PathPoint[] BluePlayerPathPoint;
    public PathPointFor2Player[] RedPlayerPathPoint;
    //public PathPoint[] GreenPlayerPathPoint;
    public PathPointFor2Player[] YellowPlayerPathPoint;
    public PathPointFor2Player[] BasePoint;

    [Header("Scale and Position Difference")]
    public float[] scales;
    public float[] positionDifference;
    public AudioSource killSound;

    public PathPointFor2Player GetStartPathPoint(PlayerPiecesFor2Player playerPiece_)
    {
        //if (playerPiece_.name.Contains("Blue"))
        //{
        //    return BluePlayerPathPoint[0];
        //}
        if (playerPiece_.name.Contains("Red"))
        {
            return RedPlayerPathPoint[0];
        }
        //else if (playerPiece_.name.Contains("Green"))
        //{
        //    return GreenPlayerPathPoint[0];
        //}
        else if (playerPiece_.name.Contains("Yellow"))
        {
            return YellowPlayerPathPoint[0];
        }
        return null;
    }
}
