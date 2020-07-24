using UnityEngine;
public abstract class BlockType : MonoBehaviour {
	public int Type;
	public Vector4 unknownVector;
	//public IBlocktype component;
	[HideInInspector] public B3DScript script;
	public virtual void ClosingEvent(){	} //should be abstract?
	public void ComaEvent(){}

}
