public interface IDisableable
{
    void Disable();
    void Enable();
    UnityEngine.Transform GetTransform { get; }
    UnityEngine.GameObject GetGameObject { get; }
}