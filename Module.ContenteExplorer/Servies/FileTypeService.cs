using System.ComponentModel.Composition;
using Module.ContentExplorer.Core.FileType;

namespace Module.ContentExplorer.Servies;

public  interface IFileTypeService
{
    string? GetFileTypePath(IFileType fileItemType);

    string? GetFileTypeBrushName(IFileType fileItemType);

    IFileType GetFileTypeByExtension(string extension);
}

[Export(typeof(IFileTypeService))]
public class FileTypeService: IFileTypeService
{
    private FileTypeDefinition[] _fileTypeDefinitions;
    private IFileType[] _fileTypes;
    [ImportingConstructor]
    public FileTypeService([ImportMany] FileTypeDefinition[] fileTypeDefinitions, [ImportMany]IFileType[] fileTypes)
    {
        _fileTypeDefinitions = fileTypeDefinitions;
        _fileTypes = fileTypes;
    }
    public string? GetFileTypePath(IFileType fileItemType)
    {
        return _fileTypeDefinitions.LastOrDefault(e => e.TargetFileType == fileItemType.GetType())?.PathData;
    }

    public string? GetFileTypeBrushName(IFileType fileItemType)
    {
        return _fileTypeDefinitions.LastOrDefault(e => e.TargetFileType == fileItemType.GetType())?.DynamicBrushName;
    }

    public IFileType GetFileTypeByExtension(string extension)
    {
        return _fileTypes.LastOrDefault(e => e.Extensions.Any(s=>s == extension))??new UnKnowItemType();
    }
}