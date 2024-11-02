using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Windows;
using Caliburn.Micro;
using Gemini.Framework;
using Gemini.Framework.Commands;
using Gemini.Framework.Services;
using GongSolutions.Wpf.DragDrop;
using Microsoft.Win32;
using NotEngine.Editor.Modules.ContentExplorer.Commands;
using NotEngine.Editor.Modules.ContentExplorer.Imports;
using NotEngine.Editor.Modules.ContentExplorer.Models;
using NotEngine.Editor.Modules.ContentExplorer.Models.ContextMenu;
using NotEngine.Editor.Modules.ContentExplorer.Utils;

namespace NotEngine.Editor.Modules.ContentExplorer.ViewModels;

[Export]
public class ContentExplorerViewModel :
    Tool,
    IDropTarget,
    ICommandHandler<RenameCommandDefinition>,
    ICommandHandler<OpenInFileExplorerCommandDefinition>,
    ICommandHandler<ImportTextureCommandDefinition>
{
    private readonly IContextMenuBuilder _contextMenuBuilder;

    #region Tool Properties

    public override string DisplayName { get; set; } = "ContentExplorer";
    public override PaneLocation PreferredLocation => PaneLocation.Bottom;

    #endregion

    #region Constructor

    [ImportingConstructor]
    public ContentExplorerViewModel(IContextMenuBuilder contextMenuBuilder)
    {
        _contextMenuBuilder = contextMenuBuilder;
        FolderItems = [new FileOrFolderItem(ProjectInfo.AssetPath, true)];
        FolderItems[0].IsExpanded = true;
        FolderItems[0].CanEdit = false;
        if (CurrentSelectFolderItems != null && CurrentSelectFolderItems.Count > 0)
        {
            ContextMenuItems = _contextMenuBuilder.BuildMenuBar(CurrentSelectFolderItems[0].FileType.GetType());
        }
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


    public BindableCollection<FileOrFolderItem> CurrentSelectFolderItems { get; set; } = [];


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
        if (CurrentSelectFolderItems != null && !CurrentSelectFolderItems[0].CanEdit)
            command.Enabled = false;
        else
        {
            command.Enabled = true;
        }
    }

    Task ICommandHandler<ImportTextureCommandDefinition>.Run(Command command)
    {
        if (CurrentSelectFolderItems != null)
        {
            // 创建打开文件对话框
            OpenFileDialog openFileDialog = new OpenFileDialog();

            // 设置文件过滤器，用户可以选择特定类型的文件
            openFileDialog.Filter =
                "Image files (*.jpg;*.jpeg;*.png;*.bmp)|*.jpg;*.jpeg;*.png;*.bmp|All files (*.*)|*.*";

            // 显示对话框并检查用户是否选择了文件
            if (openFileDialog.ShowDialog() == true)
            {
                TextureImporter.Import(openFileDialog.FileName,
                    Path.Combine(CurrentSelectFolderItems[0].FullName,
                        Path.GetFileNameWithoutExtension(openFileDialog.SafeFileName)));
            }
        }

        return Task.CompletedTask;
    }


    void ICommandHandler<ImportTextureCommandDefinition>.Update(Command command)
    {
    }

    Task ICommandHandler<RenameCommandDefinition>.Run(Command command)
    {
        if (CurrentSelectFolderItems != null && CurrentSelectFolderItems[0].CanEdit)
            CurrentSelectFolderItems[0].IsEditing = true;
        return Task.CompletedTask;
    }


    void ICommandHandler<OpenInFileExplorerCommandDefinition>.Update(Command command)
    {
    }


    Task ICommandHandler<OpenInFileExplorerCommandDefinition>.Run(Command command)
    {
        if (CurrentSelectFolderItems != null)
        {
            string folderPath = "";
            if (CurrentSelectFolderItems[0].IsFolder)
                folderPath = CurrentSelectFolderItems[0].GetFullName();
            else
                folderPath = Path.GetDirectoryName(CurrentSelectFolderItems[0].GetFullName());


            Process.Start("explorer.exe", folderPath);
        }

        return Task.CompletedTask;
    }
}