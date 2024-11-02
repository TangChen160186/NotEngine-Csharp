namespace NotEngine.Editor.Modules.ContentExplorer.Core.FileType;

public class FileTypeDefinition
{
    public Type TargetFileType { get; }
    public string DynamicBrushName { get; }
    public string? PathData { get; }


    public FileTypeDefinition(Type targetFileType, string dynamicBrushName, string? pathData)
    {
        PathData = pathData;
        DynamicBrushName = dynamicBrushName;
        TargetFileType = targetFileType;

    }
}