using UnityEngine;
public abstract class BlockType : MonoBehaviour
{
    public int Type;
    public Vector4 unknownVector;
    //public IBlocktype component;
    [HideInInspector] public B3DScript script;
    public virtual void ClosingEvent() { } //should be abstract?
    public virtual void ComaEvent() { }
    public void RecursivelyInvoke()
    {
        foreach (Transform item in transform)
        {
            var block = item.GetComponent<BlockType>();
			if(block)
			{
            if (block.Type == 18)
            {
                ((Block18)block).Enable();
            }
			block.RecursivelyInvoke();
			}
        }
    }

}
