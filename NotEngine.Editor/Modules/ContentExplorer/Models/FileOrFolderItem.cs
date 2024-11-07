using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using Caliburn.Micro;
using NotEngine.Editor.Models;
using NotEngine.Editor.Services;
using NotEngine.Editor.Utils;

namespace NotEngine.Editor.Modules.ContentExplorer.Models;
public class FileOrFolderItem(
    string name,
    BindableCollection<FileOrFolderItem>? children,
    FileOrFolderItem? parent,
    string extension,
    bool acceptDrop,
    bool canRename,
    bool isFolder,
    BitmapImage? icon,
    bool isExpanded = true,
    bool isEditing = false)
    : PropertyChangedBase
{
    private string _name = name;

    public string Name
    {
        get => _name;
        set => SetName(value);
    }

    public string NameWithExtension => Name + Extension;

    private bool _isExpanded = isExpanded;

    public bool IsExpanded
    {
        get => _isExpanded;
        set => Set(ref _isExpanded, value);
    }

    public string Extension { get; set; } = extension;
    public bool AcceptDrop { get; set; } = acceptDrop;

    public string FullRelativePath => GetFullRelativePath();
    public string FullPath => GetFullPath();
    public FileOrFolderItem? Parent { get; set; } = parent;

    public bool IsFolder { get; private set; } = isFolder;

    private bool _isEditing = isEditing;

    public bool IsEditing
    {
        get => _isEditing;
        set => Set(ref _isEditing, value);
    }


    private bool _canRename = canRename;

    public bool CanEdit
    {
        get => _canRename;
        set => Set(ref _canRename, value);
    }


    public AssetType AssetType => IoC.Get<IAssetTypeService>().GetAssetType(extension);


    private BindableCollection<FileOrFolderItem>? _children = children;

    public BindableCollection<FileOrFolderItem>? Children
    {
        get => _children;
        set => Set(ref _children, value);
    }

    private BitmapImage? _icon = icon;
    public BitmapImage? Icon
    {
        get => _icon;
        set=>Set(ref _icon, value);
    }

    public string GetFullRelativePath()
    {
        if (Parent == null)
            return Name;

        return Path.Combine(Parent.GetFullRelativePath(), Name + Extension);
    }
    


    public void SetName(string name,bool notify = true)
    {
        if (notify)
        {
            if (!FileUtils.IsFileOrFolderNameValid(name))
            {
                MessageBox.Show("The file or directory name is invalid");
                return;
            }

            try
            {
                FileUtils.Rename(FullPath, name+extension);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return;
            }
        }


        Set(ref _name, name, nameof(Name));
    }

    /// <summary>
    /// 判断是不是另一个item的子项
    /// </summary>
    /// <param name="parent"></param>
    /// <returns></returns>
    public bool IsSubItem(FileOrFolderItem parent)
    {
        if(parent.FullPath!= FullPath) 
            Console.WriteLine();
        return FileUtils.IsSubfolder(parent.FullPath, FullPath);
    }
    private string GetFullPath()
    {
        return Path.Combine(ProjectInfo.ProjectPath,FullRelativePath);
    }
}