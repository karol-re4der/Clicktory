using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{
    public int direction;
    public bool outputing;
    public Machine owner;

    public FlowingResource res;

    public void SetupGate(bool outputing, int direction)
    {
        this.outputing = outputing;
        this.direction = direction;
    }

    public Gate GetLink()
    {
        Gate gate = null;
        if (GetComponent<MachinePart>().tile.links[(direction+owner.rotation)%4] != null)
        {
            if (GetComponent<MachinePart>().tile.links[(direction + owner.rotation) % 4].machine != null)
            {
                //omg so many things are wrong with this one line of code 
                gate = GetComponent<MachinePart>().tile.links[(direction + owner.rotation) % 4].machine.GetComponent<Machine>().GetGates().Where((x) => x.outputing != outputing).Where((x) => (x.direction+x.owner.rotation)%4 == (direction +owner.rotation + 2) % 4).ElementAt(0);
            }
        }

        return gate;
    }

    public int DirectionRotated()
    {
        return (direction+owner.rotation)% 4;
    }

    public Vector3 GetPosition()
    {
        Gate link = GetLink();
        Vector3 endPos;
        Vector3 posA;
        Vector3 posB;

        posA = GetComponent<MachinePart>().transform.position;
        if (link)
        {
            posB = link.GetComponent<MachinePart>().transform.position;
        }
        else
        {
            posB = GetComponent<MachinePart>().tile.links[(direction + owner.rotation) % 4].transform.position;
            posB.y += transform.localScale.y/2;
        }


        endPos = Vector2.Lerp(posA, posB, 0.5f);

        return endPos;
    }
    public int GetOrder()
    {
        if (outputing)
        {
            if (DirectionRotated() == 0 || DirectionRotated() == 3)
            {
                if (owner.obstructing)
                {
                    return GetComponent<SpriteRenderer>().sortingOrder-1;
                }
                else
                {
                    return GetComponent<SpriteRenderer>().sortingOrder + 1;
                }
            }
            else if (GetLink())
            {
                if (GetLink().owner.obstructing)
                {
                    return GetLink().GetComponent<SpriteRenderer>().sortingOrder;
                }
                else
                {
                    return GetLink().GetComponent<SpriteRenderer>().sortingOrder + 1;

                }
            }
        }
        else if(GetLink())
        {
            return GetLink().GetOrder();
        }
        return 0;
    }

    public bool IsOccupied()
    {
        if (res != null)
        {
            return true;
        }
        return false;
    }

    public void Activate()
    {
        if (outputing) 
        { 
            if (res != null)
            {
                Gate link = GetLink();
                if (link != null)
                {
                    if (link.res == null)
                    {
                        link.res = res;
                        res = null;
                    }
                }
                else
                {
                    //res.Dispose();
                    //res = null;
                }
            }
        }
    }
}
