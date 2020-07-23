using UnityEngine;
public abstract class BlockType : MonoBehaviour {
	public int Type;
	public Vector4 unknownVector;
	public IBlocktype component;

	public virtual void ClosingEvent(){} //should be abstract?
	public void ComaEvent(){}

}
