using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorChange : MonoBehaviour {
    //basic script that creates  new materials and flips the color every 10 seconds

    public float timeToChange = 3f;
    private float timer = 0f;
    private Material material1;
    private Material material2;
    private bool mat1InUse = false;
    private Renderer ourRenderer;

    private void OnEnable()
    {
        material1 = new Material(Shader.Find("Standard"))
        {
            color = new Color(255, 0, 0, 255)
        };
        material2 = new Material(Shader.Find("Standard"))
        {
            color = new Color(0, 255, 0, 255)
        };

        ourRenderer = gameObject.GetComponent<Renderer>();
    }


    // Update is called once per frame
    void Update () {
        timer += Time.deltaTime;
        //for every frame we increase the timer.
        if(timer >= timeToChange)
        {
            timer = 0f; //reset timer.
            SwitchMaterials();
        }
	}

    void SwitchMaterials()
    {
        if (ourRenderer != null)
        {
            if (mat1InUse)
            {
                ourRenderer.material = material2;
                mat1InUse = false;
            }
            else
            {
                ourRenderer.material = material1;
                mat1InUse = true;
            }
        }
        else
        {
            Debug.Log("Cannot find renderer on gameobject");
        }
    }


}
