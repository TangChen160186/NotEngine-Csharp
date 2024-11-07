using System.Collections.Specialized;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Threading;
using Caliburn.Micro;
using Gemini.Framework;
using Gemini.Framework.Commands;
using Gemini.Framework.Services;
using GongSolutions.Wpf.DragDrop;
using NotEngine.Editor.Modules.ContentExplorer.Commands;
using NotEngine.Editor.Modules.ContentExplorer.Models;
using NotEngine.Editor.Modules.ContentExplorer.Models.ContextMenu;

namespace NotEngine.Editor.Modules.ContentExplorer.ViewModels;

[Export]
public class ContentExplorerViewModel :
    Tool,
    IDropTarget,
    ICommandHandler<RenameCommandDefinition>,
    ICommandHandler<OpenInFileExplorerCommandDefinition>,
    ICommandHandler<ImportTextureCommandDefinition>,
    ICommandHandler<CreateFolderCommandDefinition>
{
    private string? _lastExtension = null;

    private readonly IContextMenuBuilder _contextMenuBuilder;
    private static IResourceManager _resourceManager;

    #region Tool Properties

    public override string DisplayName { get; set; } = "ContentExplorer";
    public override PaneLocation PreferredLocation => PaneLocation.Bottom;

    #endregion

    private BindableCollection<FileOrFolderItem> _masterFolderItems;

    public BindableCollection<FileOrFolderItem> MasterFolderItems
    {
        get => _masterFolderItems;
        set => Set(ref _masterFolderItems, value);
    }

    public BindableCollection<FileOrFolderItem> CurrentSelectFolderItems { get; set; } = [];


    private BindableCollection<FileOrFolderItem> _detailItems;

    public BindableCollection<FileOrFolderItem> DetailItems
    {
        get => _detailItems;
        set => Set(ref _detailItems, value);
    }

    public BindableCollection<FileOrFolderItem> CurrentSelectDetailItems { get; set; } = [];


    private BindableCollection<ContextMenuItemBase> _contextMenuItems;

    public BindableCollection<ContextMenuItemBase> ContextMenuItems
    {
        get => _contextMenuItems;
        set => Set(ref _contextMenuItems, value);
    }

    #region Constructor

    [ImportingConstructor]
    public ContentExplorerViewModel(IContextMenuBuilder contextMenuBuilder, IResourceManager resourceManager)
    {
        _contextMenuBuilder = contextMenuBuilder;
        _resourceManager = resourceManager;
        MasterFolderItems = [GetMasterFolderItems(ProjectInfo.AssetPath)];
        CurrentSelectFolderItems.CollectionChanged += CurrentSelectFolderItemsOnCollectionChanged;

    }

    #endregion
    private static FileOrFolderItem GetMasterFolderItems(string path)
    {
        DirectoryInfo directoryInfo = new DirectoryInfo(path);
        FileOrFolderItem fileOrFolderItem = new FileOrFolderItem(directoryInfo.Name,
            [], null, string.Empty, true, false,
            true, _resourceManager.GetBitmap("Resources/Icons/folder.png", "NotEngine.Editor"));
        fileOrFolderItem.Children!.AddRange(GetMasterFolderItems(path, fileOrFolderItem));
        return fileOrFolderItem;
    }

    private static BindableCollection<FileOrFolderItem> GetMasterFolderItems(string path, FileOrFolderItem? parent)
    {
        BindableCollection<FileOrFolderItem> res = [];
        DirectoryInfo directoryInfo = new DirectoryInfo(path);

        foreach (var item in directoryInfo.EnumerateDirectories())
        {
            BindableCollection<FileOrFolderItem> children = [];
            FileOrFolderItem fileOrFolderItem = new FileOrFolderItem(item.Name,
                children, parent, string.Empty, parent != null, parent != null,
                true, _resourceManager.GetBitmap("Resources/Icons/folder.png", "NotEngine.Editor"));

            children.AddRange(GetMasterFolderItems(item.FullName, fileOrFolderItem));
            res.Add(fileOrFolderItem);
        }

        return res;
    }

    private static BindableCollection<FileOrFolderItem> GetDetailItems(FileOrFolderItem masterFolderItem)
    {
        var fullPath = masterFolderItem.FullPath;
        DirectoryInfo directoryInfo = new DirectoryInfo(fullPath);
        BindableCollection<FileOrFolderItem> res = [];
        foreach (var item in directoryInfo.EnumerateDirectories())
        {

            FileOrFolderItem fileOrFolderItem = new FileOrFolderItem(item.Name,
                null, masterFolderItem, string.Empty, true, true,
                true, _resourceManager.GetBitmap("Resources/Icons/folder.png", "NotEngine.Editor"));
            res.Add(fileOrFolderItem);
        }

        foreach (var item in directoryInfo.EnumerateFiles())
        {
            FileOrFolderItem fileOrFolderItem = new FileOrFolderItem(Path.GetFileNameWithoutExtension(item.Name),
                null, masterFolderItem, item.Extension, false, true,
                false, null);
            res.Add(fileOrFolderItem);
        }
        return res;
    }

    private static FileOrFolderItem? FindFileOrFolderItemByFullPath(string relativePath, FileOrFolderItem rootItem)
    {
        if (rootItem.FullRelativePath == relativePath)
            return rootItem;

        if (rootItem.Children != null)
        {
            foreach (var child in rootItem.Children)
            {
                var foundItem = FindFileOrFolderItemByFullPath(relativePath, child);
                if (foundItem != null) // 如果找到匹配项，则立即返回
                    return foundItem;
            }
        }

        return null; // 没有匹配项时返回 null
    }

    private void CurrentSelectFolderItemsOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (CurrentSelectFolderItems.Any())
        {
            var extension = CurrentSelectFolderItems[0].Extension;
            if (extension != _lastExtension)
            {
                ContextMenuItems = _contextMenuBuilder.BuildMenuBar(CurrentSelectFolderItems[0].Extension);
                _lastExtension = extension;
            }
            DetailItems = GetDetailItems(CurrentSelectFolderItems[0]);
        }
    }

    #region Drag Drop
    public void DragOver(IDropInfo dropInfo)
    {

        if (dropInfo.DragInfo.SourceItem is FileOrFolderItem &&
            dropInfo.TargetItem is FileOrFolderItem targetItem &&
            targetItem is { AcceptDrop: true, IsFolder: true })
        {
            var sourceItems = dropInfo.DragInfo.SourceItems.Cast<FileOrFolderItem>().ToList();

            if (sourceItems.Count > 0 &&
                sourceItems.All(e => e.FullPath != targetItem.FullPath) &&
                !targetItem.IsSubItem(sourceItems[0]))
            {
                dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
                dropInfo.Effects = DragDropEffects.All;
            }

        }
    }

    public void Drop(IDropInfo dropInfo)
    {
        if (dropInfo.DragInfo.SourceItem is FileOrFolderItem &&
            dropInfo.TargetItem is FileOrFolderItem targetItem &&
            targetItem is { AcceptDrop: true, IsFolder: true })
        {
            List<FileOrFolderItem> sourceItems = [];
            sourceItems = CurrentSelectDetailItems.Count > 0 ? dropInfo.DragInfo.SourceItems.Cast<FileOrFolderItem>().ToList() : FindRootFileOrFolderItems(CurrentSelectFolderItems);

            if (sourceItems.Count>0 && sourceItems[0].Parent != targetItem)
            {
                for (int i = sourceItems.Count - 1; i >= 0; i--)
                {
                    var sourceItem = sourceItems[i];
                    try
                    {
                        Directory.Move(sourceItem.FullPath, Path.Combine(targetItem.FullPath, sourceItem.NameWithExtension));
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);
                        return;
                    }

                    if (targetItem.IsFolder && sourceItem.IsFolder)
                    {
                        var sourceMasterItem =
                            FindFileOrFolderItemByFullPath(sourceItem.FullRelativePath, MasterFolderItems[0])!;
                        targetItem.Children?.Add(sourceMasterItem);
                        sourceMasterItem.Parent?.Children?.Remove(sourceMasterItem);
                        sourceMasterItem.Parent = targetItem;
                    }
                }
            }
            DetailItems = GetDetailItems(CurrentSelectFolderItems[0]);
        }
        CurrentSelectFolderItems.Clear();
    }

    private static List<FileOrFolderItem> FindRootFileOrFolderItems(IList<FileOrFolderItem> items)
    {
        List<FileOrFolderItem> res = new List<FileOrFolderItem>();
        foreach (var item in items)
        {
            bool isRoot = true;
            foreach (var item1 in items)
            {
                if (item.IsSubItem(item1))
                {
                    isRoot = false;
                    break;
                }
            }
            if (isRoot)
                res.Add(item);
        }

        return res;
    }
    #endregion

    #region Command Rename
    void ICommandHandler<RenameCommandDefinition>.Update(Command command)
    {
        if (CurrentSelectDetailItems.Any()|| CurrentSelectFolderItems.Any() && CurrentSelectFolderItems[0].CanEdit)
            command.Enabled = true;
        else
        {
            command.Enabled = false;
        }
    }

    private string? oldRelativePath = null;
    Task ICommandHandler<RenameCommandDefinition>.Run(Command command)
    {
        if (CurrentSelectDetailItems.Any())
        {
            if (CurrentSelectDetailItems[0].IsFolder)
            {
                oldRelativePath = CurrentSelectDetailItems[0].FullRelativePath;
            }

            CurrentSelectDetailItems[0].IsEditing = true;
   
        }
        else if (CurrentSelectFolderItems.Any() && CurrentSelectFolderItems[0].CanEdit)
            CurrentSelectFolderItems[0].IsEditing = true;
        return Task.CompletedTask;
    }

    public void RenameTextboxLostFocus(string name)
    {
        if (oldRelativePath != null)
        {
            var masterItem = FindFileOrFolderItemByFullPath(oldRelativePath, MasterFolderItems[0])!;
            masterItem.SetName(CurrentSelectDetailItems[0].Name,false);
            var s = MasterFolderItems[0];
            oldRelativePath = null;
        }
    }
    #endregion

    #region Command CreateFolder

    void ICommandHandler<CreateFolderCommandDefinition>.Update(Command command)
    {

    }
    Task ICommandHandler<CreateFolderCommandDefinition>.Run(Command command)
    {
        if (CurrentSelectFolderItems.Any())
        {
            var baseFolderName = "New Folder";
            var folderIndex = 0;
            string newFolderPath;
            do
            {
                var newFolderName = $"{baseFolderName} ({folderIndex})";
                newFolderPath = Path.Combine(CurrentSelectFolderItems[0].FullPath, newFolderName);
                folderIndex++;
            } while (Directory.Exists(newFolderPath)); // 检查是否存在相同名称的文件夹

            // 创建文件夹
            var directoryInfo = Directory.CreateDirectory(newFolderPath);

            // 创建文件夹对象并添加到父项的子项列表中
            var item = new FileOrFolderItem(
                directoryInfo.Name, null, CurrentSelectFolderItems[0], string.Empty,
                true, true, true,
                _resourceManager.GetBitmap("Resources/Icons/folder.png", "NotEngine.Editor"), true,false);

            CurrentSelectFolderItems[0].Children!.Add(item);
            DetailItems = GetDetailItems(CurrentSelectFolderItems[0]);
           
            var s = DetailItems.First(e => e.FullRelativePath == item.FullRelativePath);
            CurrentSelectDetailItems.Clear();
            CurrentSelectDetailItems.Add(s);
           Dispatcher.CurrentDispatcher.Invoke(() => s.IsEditing = true,DispatcherPriority.Render);
            oldRelativePath = s?.FullRelativePath;
        }

        return Task.CompletedTask;
    }

    #endregion

    #region Command OpenInFileExplorer
    void ICommandHandler<OpenInFileExplorerCommandDefinition>.Update(Command command)
    {
    }
    Task ICommandHandler<OpenInFileExplorerCommandDefinition>.Run(Command command)
    {
        if (CurrentSelectFolderItems.Any())
        {
            string folderPath = "";
            folderPath = CurrentSelectFolderItems[0].FullPath;
            Process.Start("explorer.exe", folderPath);
        }

        return Task.CompletedTask;
    }


    #endregion


    #region Command ImportTexture
    Task ICommandHandler<ImportTextureCommandDefinition>.Run(Command command)
    {
        //if (CurrentSelectFolderItems.Any())
        //{
        //    OpenFileDialog openFileDialog = new OpenFileDialog
        //    {
        //        Filter = "Image files (*.jpg;*.jpeg;*.png;*.bmp)|*.jpg;*.jpeg;*.png;*.bmp|All files (*.*)|*.*"
        //    };

        //    // 显示对话框并检查用户是否选择了文件
        //    if (openFileDialog.ShowDialog() == true)
        //    {
        //        TextureImporter.Import(openFileDialog.FileName,
        //            Path.Combine(CurrentSelectFolderItems[0].FullPath,
        //                Path.GetFileNameWithoutExtension(openFileDialog.SafeFileName)));
        //    }
        //}

        return Task.CompletedTask;
    }

    void ICommandHandler<ImportTextureCommandDefinition>.Update(Command command)
    {

    }


    #endregion






}