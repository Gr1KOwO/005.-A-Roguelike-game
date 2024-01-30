using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomVariant : MonoBehaviour
{
    [SerializeField]
    private Openings doors;

    [SerializeField]
    private Openings walls;

    public void Setup(HashSet<Vector2Int> config)
    {
        foreach (var wall in walls.GetOpenings())
        {
            bool contains = config.Contains(wall.Pos);
            wall.GameObject.SetActive(!contains);
        }

        foreach (var wall in doors.GetOpenings())
        {
            bool contains = config.Contains(wall.Pos);
            wall.GameObject.SetActive(contains);
        }
    }
}
