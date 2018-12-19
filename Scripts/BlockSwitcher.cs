using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class BlockSwitcher : MonoBehaviour {
	//public static int states;
	//public List<Renderer>[] renderers = new List<Renderer>[]{};
	public List<List<GameObject>> Gobs = new List<List<GameObject>>();
	public bool rendered = false;
	public int groups;

	public void switchState(int state)
	{
		if (state<=Gobs.Count)
		{
			int i = 0;
			foreach(var rends in Gobs)
			{
				if (i != state)
				{
					foreach(var rend in rends)
					{
						rend.SetActive(false);
					}
				}
				i++;
			}

			foreach (GameObject rend in Gobs[state])
			{
				rend.SetActive(true);
			}

		}
	}
	public void Initialize(int state)
	{
		for (int i = 0; i<=state;i++)
		{
			Gobs.Add(new List<GameObject>());
		}
	}
	public void AddRenderer(int state, GameObject rend)
	{
		Gobs[state].Add(rend);
	}

	void OnWillRenderObject()
	{
		if (!rendered)
		{
			Initialize(groups);

			int childCount = transform.childCount;
			int state = 0;
			//addSwitch swObj = gameObject.AddComponent<addSwitch>();
			for (int i = 0; i<childCount;i++)
			{
				if (transform.GetChild(i).name != "444")
				{
					AddRenderer(state,transform.GetChild(i).gameObject);
					//swObj.rend = Gobs[state];
				}
				else
				{
					//swObj = gameObject.AddComponent<addSwitch>();
					state++;
				}
			}

			rendered = true;
			switchState(0);
		}
	}


}
public class addSwitch : MonoBehaviour
{
	public List<GameObject> rend;
}

#if UNITY_EDITOR
[ExecuteInEditMode]
[CustomEditor(typeof(BlockSwitcher))]
public class GizmosSwitcher21 : Editor {


	string aaa = "0";

	public override void OnInspectorGUI()
    {
		aaa = EditorGUILayout.TextField("Switch to state number:",aaa);
		BlockSwitcher mySwitcher = (BlockSwitcher)target;

        if(GUILayout.Button("Switch Object"))
        {
			//Debug.LogWarning(aaa);
            mySwitcher.switchState(int.Parse(aaa));
        }
    }

}
#endif
