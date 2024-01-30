using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Openings : MonoBehaviour
{

    [System.Serializable]
    public class Opening
    {
        /// <summary>
        /// Расположение проема относительно центра
        /// </summary>
        public Vector2Int Pos;

        /// <summary>
        /// Объект проемы (стена, дверь, шторы и т.д.)
        /// </summary>
        public GameObject GameObject;
    }

    [SerializeField]
    private Opening top;

    [SerializeField]
    private Opening right;

    [SerializeField]
    private Opening bottom;

    [SerializeField]
    private Opening left;

    private List<Opening> list;

    public List<Opening> GetOpenings()
    {
        if (list == null)
        {
            list = new List<Opening>()
            {
                top,
                right,
                bottom,
                left
            };
        }

        return list;
    }
}
