using UnityEngine;

public abstract class ABotBehaviour : MonoBehaviour
{
    public abstract bool ShouldProcessUpdate();
    public abstract void ProcessUpdate(UnitMover unitMover);
}
