using UnityEngine;
using UnityEngine.InputSystem;

#if UNITY_EDITOR
using UnityEditor;
[InitializeOnLoad]
#endif

public class DeltaTimeProcessor : InputProcessor<Vector2>
{
#if UNITY_EDITOR
    static DeltaTimeProcessor()
    {
        Initialise();
    }
#endif
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Initialise()
    {
        InputSystem.RegisterProcessor<DeltaTimeProcessor>();
    }
    
    public override Vector2 Process(Vector2 value, InputControl control)
    {
        return value * Time.deltaTime;
    }


}
