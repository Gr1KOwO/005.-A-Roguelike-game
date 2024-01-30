using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tileItem : Item
{
    [SerializeField] private int level=1;
    protected override void CustomOnTriggerEnter(Collider other)
    {
       if(other.TryGetComponent<PlayerController>(out PlayerController player))
        {
            player.changeTile(gameObject);
            Destroy(gameObject);
        }
    }

    public int GetLevel()
    {
        return level;
    }

    public void setLevel(int level)
    {
        this.level = level;
    }
}
