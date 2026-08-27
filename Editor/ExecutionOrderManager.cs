using UnityEditor;

[InitializeOnLoad]
public class ExecutionOrderManager : Editor
{
    static ExecutionOrderManager()
    {
        EditorApplication.delayCall += () =>
        {
            try
            {
                System.Type fileSaverType = typeof(UXF.FileSaver);

                // Iterate through all scripts (Might be a better way to do this?)
                foreach (MonoScript monoScript in MonoImporter.GetAllRuntimeMonoScripts())
                {
                    if (monoScript.GetClass() == fileSaverType)
                    {
                        // And it's not at the execution time we want already
                        // (Without this we will get stuck in an infinite loop)
                        if (MonoImporter.GetExecutionOrder(monoScript) != 1000)
                        {
                            MonoImporter.SetExecutionOrder(monoScript, 1000);
                        }
                        break;
                    }
                }
            }
            catch (System.Exception exception)
            {
                UnityEngine.Debug.LogError($"ExecutionOrderManager failed to set execution order: {exception.Message}");
            }
        };
    }
}
