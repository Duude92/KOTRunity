using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;





class InvokeMe : MonoBehaviour
{
    public string space;
    public string blocks;
    //public Vector3 tempSpace;
    //public float tempScale;
    public Transform GO;
    //public GameObject SpcOb;
    bool notfound = false;
    GameObject ind;

    void OnRenderObject()
    {
        if ((transform.childCount == 0) && (!notfound))
        {
            Invoke();
        }
    }


    public void Destroy()
    {
        DestroyImmediate(ind);

    }
    public void Invoke()
    {

        GameObject me = null;
        Transform meT = null;

        meT = GameManager.common.transform.Find(blocks);

        if (meT)
        {
            me = meT.gameObject;
        }
        else
        {
            meT = GO?.Find(blocks);
            if (meT)
                me = meT?.gameObject;
            else
            {
                Debug.LogError(blocks + " NOT FOUND");
                me = null;
            }

        }




        GameObject SpcOb = null;




        if (space != "$$world")
        {
            Transform tr = GO?.Find(space);
            SpcOb = tr?.gameObject;
        }
        else
        {
            notfound = true;
        }




        if (me)
        {
            me.SetActive(false);
            if (space != "")
            {
                if (SpcOb != null)
                {
                    SpcOb.SetActive(false);
                    Block24 Spc = SpcOb.GetComponent<Block24>();
                    ind = Instantiate(me);
                    //ind.transform.localPosition = Spc.position;

                    ind.transform.SetParent(gameObject.transform);
                    Vector3 rotation = new Vector3();
                    float sy = Mathf.Sqrt(Spc.matrix[0][0] * Spc.matrix[0][0] + Spc.matrix[1][0] * Spc.matrix[1][0]);
                    bool singular = sy < 1e-6;
                    float rad = 180 / Mathf.PI;
                    if (!singular)
                    {
                        rotation.x = Mathf.Atan2(Spc.matrix[2][1], Spc.matrix[2][2]) * rad;
                        rotation.z = Mathf.Atan2(-Spc.matrix[2][0], sy) * rad;
                        rotation.y = Mathf.Atan2(Spc.matrix[1][0], Spc.matrix[0][0]) * rad;
                    }
                    else
                    {
                        rotation.x = Mathf.Atan2(-Spc.matrix[1][2], Spc.matrix[1][1]) * rad;
                        rotation.z = Mathf.Atan2(-Spc.matrix[2][0], sy) * rad;
                        rotation.y = 0;
                    }

                    ind.transform.localPosition = Spc.position;
                    ind.transform.eulerAngles = rotation;
                    ind.SetActive(true);

                }
                else
                {
                    Debug.LogError(blocks + " space not found");
                }
            }
            else
            {
                //Debug.Log("Space is empty for blocks: "+blocks);
                ind = Instantiate(me);
                //GameObject ind = Instantiate(me,tempSpace,new Quaternion());
                //ind.transform.localPosition = tempSpace;
                ind.transform.SetParent(gameObject.transform);
                ind.transform.localPosition = new Vector3(0, 0, 0);
                ind.SetActive(true);

            }
        }
        else
        {
            Debug.LogError(blocks + " block not found");
        }
    }
}
