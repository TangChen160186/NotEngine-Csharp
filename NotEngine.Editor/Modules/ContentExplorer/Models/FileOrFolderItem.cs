using System.IO;
using System.Windows;
using Caliburn.Micro;
using NotEngine.Editor.Modules.ContentExplorer.Core.FileType;
using NotEngine.Editor.Modules.ContentExplorer.Servies;
using NotEngine.Editor.Modules.ContentExplorer.Utils;

namespace NotEngine.Editor.Modules.ContentExplorer.Models;
public class FileOrFolderItem : PropertyChangedBase
{
    protected string BaseRelativePath;
    private string _name;
    public string Name
    {
        get => _name;
        set => SetName(value);
    }

    private bool _isExpanded;

    public bool IsExpanded
    {
        get => _isExpanded;
        set => Set(ref _isExpanded, value);
    }

    public string? Extension { get; set; }
    public IFileType FileType { get; set; }
    public bool AcceptDrop { get; set; } = true;


    public string FullName => GetFullName();
    public FileOrFolderItem? Parent { get; set; }

    public bool IsFolder { get; private set; }

    private bool _isEditing = false;

    public bool IsEditing
    {
        get => _isEditing;
        set => Set(ref _isEditing, value);
    }


    private bool _canEdit = true;

    public bool CanEdit
    {
        get => _canEdit;
        set => Set(ref _canEdit, value);
    }

    private BindableCollection<FileOrFolderItem> _children;

    public BindableCollection<FileOrFolderItem> Children
    {
        get => _children;
        set => Set(ref _children, value);
    }

    public string GetFullName()
    {
        if (this.Parent == null)
            return this.BaseRelativePath;

        return Path.Combine(this.Parent.GetFullName(), this.Name + this.Extension);
    }

    public FileOrFolderItem(string path, bool topItem = false, FileOrFolderItem? parent = null)
    {
        if (topItem)
            BaseRelativePath = path;

        Parent = parent;

        if (File.Exists(path))
        {
            FileInfo info = new FileInfo(path);
            this._name = Path.GetFileNameWithoutExtension(info.Name);
            this.Extension = Path.GetExtension(info.Name);
            this.AcceptDrop = false;
            this.FileType = IoC.Get<IFileTypeService>().GetFileTypeByExtension(Extension);
            this.IsFolder = false;
        }

        else if (Directory.Exists(path))
        {
            var directoryInfo = new DirectoryInfo(path);
            FileType = new FolderItemType();
            IsFolder = true;
            Extension = string.Empty;
            _name = directoryInfo.Name;
            _children = [];
            foreach (var info in directoryInfo.EnumerateFileSystemInfos())
            {
                Children.Add(new FileOrFolderItem(info.FullName, false, this));
            }
        }
    }


    public void SetName(string name)
    {
        if (!FileUtils.IsFileOrFolderNameValid(name))
        {
            MessageBox.Show("The file or directory name is invalid");
            return;
        }

        try
        {

            FileUtils.Rename(GetFullName(), name);
        }
        catch (Exception e)
        {
            MessageBox.Show(e.Message);
            return;
        }

        Set(ref _name, name);
    }
}