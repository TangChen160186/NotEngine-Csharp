using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Windows;
using Caliburn.Micro;
using Gemini.Framework;
using Gemini.Framework.Commands;
using Gemini.Framework.Services;
using GongSolutions.Wpf.DragDrop;
using Module.ContentExplorer.Commands;
using Module.ContentExplorer.Models;
using Module.ContentExplorer.Models.ContextMenu;
using Module.ContentExplorer.Utils;


namespace Module.ContentExplorer.ViewModels;

[Export]
public class ContentExplorerViewModel : Tool, IDropTarget, ICommandHandler<RenameCommandDefinition>,ICommandHandler<OpenInFileExplorerCommandDefinition>
{
    #region Tool Properties
    public override string DisplayName { get; set; } = "ContentExplorer";
    public override PaneLocation PreferredLocation => PaneLocation.Bottom;

    #endregion
    #region Constructor
    [ImportingConstructor]
    public ContentExplorerViewModel(IContextMenuBuilder contextMenuBuilder)
    {
        FolderItems = [new FileOrFolderItem(@"G:\OpenTK-W", true)];
        FolderItems[0].IsExpanded = true;
        FolderItems[0].CanEdit = false;
        ContextMenuItems = contextMenuBuilder.BuildMenuBar();
    }


    #endregion


    private BindableCollection<ContextMenuItemBase> _contextMenuItems;

    public BindableCollection<ContextMenuItemBase> ContextMenuItems
    {
        get => _contextMenuItems;
        set => Set(ref _contextMenuItems, value);
    }



    private BindableCollection<FileOrFolderItem> _folderItems;
    public BindableCollection<FileOrFolderItem> FolderItems
    {
        get => _folderItems;
        set => Set(ref _folderItems, value);
    }

    private FileOrFolderItem? _currentSelectFolderItem;

    public FileOrFolderItem? CurrentSelectFolderItem
    {
        get => _currentSelectFolderItem;
        set => Set(ref _currentSelectFolderItem, value);
    }




    public void DragOver(IDropInfo dropInfo)
    {
        if (dropInfo.Data is FileOrFolderItem sourceItem && dropInfo.TargetItem is FileOrFolderItem targetItem &&
            sourceItem != targetItem &&
            !FileUtils.IsSubfolder(sourceItem.FullName,
                targetItem.FullName) && targetItem.AcceptDrop)
        {
            dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
            dropInfo.Effects = DragDropEffects.All;
        }
    }

    public void Drop(IDropInfo dropInfo)
    {
        if (dropInfo.TargetItem is FileOrFolderItem targetItem)
        {
            if (dropInfo.Data is FileOrFolderItem sourceItem)
            {
                if (targetItem != sourceItem.Parent)
                {
                    try
                    {
                        Directory.Move(sourceItem.FullName, Path.Combine(targetItem.FullName, sourceItem.Name));
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);
                        return;
                    }
                    targetItem.Children.Add(sourceItem);
                    if (sourceItem.Parent != null)
                    {
                        sourceItem.Parent.Children.Remove(sourceItem);
                        sourceItem.Parent = targetItem;
                    }
                }
            }
        }
    }


    void ICommandHandler<RenameCommandDefinition>.Update(Command command)
    {
        if (CurrentSelectFolderItem != null && !CurrentSelectFolderItem.CanEdit)
            command.Enabled = false;
        else
        {
            command.Enabled = true;
        }
    }


    Task ICommandHandler<RenameCommandDefinition>.Run(Command command)
    {
        if (CurrentSelectFolderItem != null && CurrentSelectFolderItem.CanEdit)
            CurrentSelectFolderItem.IsEditing = true;
        return Task.CompletedTask;
    }


    void ICommandHandler<OpenInFileExplorerCommandDefinition>.Update(Command command)
    {

    }


    Task ICommandHandler<OpenInFileExplorerCommandDefinition>.Run(Command command)
    {
        if (CurrentSelectFolderItem != null)
        {
            string folderPath = "";
            if (CurrentSelectFolderItem.IsFolder)
                folderPath = CurrentSelectFolderItem.GetFullName();
            else
                folderPath = Path.GetDirectoryName(CurrentSelectFolderItem.GetFullName());
            

            Process.Start("explorer.exe", folderPath);
    
        }
        return Task.CompletedTask;
    }
}