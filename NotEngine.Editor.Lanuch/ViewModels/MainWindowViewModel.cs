using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using NotEngine.Editor.Lanuch.Models;

namespace NotEngine.Editor.Lanuch.ViewModels;

public partial class MainWindowViewModel : ObservableValidator
{
    private static readonly Regex FolderNameRegex = new(@"^[^<>:""/\\|?*\n]+$", RegexOptions.Compiled);
    public static void CreateFileWithDirectories(string filePath)
    {
        if (!File.Exists(filePath))
        {
            string directoryPath = Path.GetDirectoryName(filePath);

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            using FileStream fs = File.Create(filePath);
        }
    }

    public ObservableCollection<UserProject> Projects { get; set; }


    [ObservableProperty] 
    [CustomValidation(typeof(MainWindowViewModel), nameof(ValidateProjectPath))]
    [NotifyDataErrorInfo]
    public string _projectPath = "";

    [ObservableProperty]
    [CustomValidation(typeof(MainWindowViewModel), nameof(ValidateProjectName))]
    [NotifyDataErrorInfo]
    public string _projectName = "";  

    [ObservableProperty]
    [CustomValidation(typeof(MainWindowViewModel), nameof(ValidateProjectLocation))]
    [NotifyDataErrorInfo]
    public string _projectLocation = "";


    [ObservableProperty] 
    public UserProject _selectProject;
    private readonly string _appDataEngineInfoPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "NotEngine", "projects.json");
    public MainWindowViewModel()
    {

        CreateFileWithDirectories(_appDataEngineInfoPath);
        Projects = JsonHelper.DeserializeFromFile<ObservableCollection<UserProject>>(_appDataEngineInfoPath) ?? [];
        ProjectName = "New Project";
        var projectLocation =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "NotEngine");
        if (!Directory.Exists(projectLocation)) Directory.CreateDirectory(projectLocation);
        ProjectLocation = projectLocation;
    }

    public static  ValidationResult? ValidateProjectName(string projectName, ValidationContext context)
    {
        if (string.IsNullOrWhiteSpace(projectName))
        {
            return new ValidationResult("Project name cannot be empty.");
        }

        if (!FolderNameRegex.IsMatch(projectName))
        {
            return new ValidationResult("Project name contains invalid characters.");
        }

        return ValidationResult.Success;
    }

    public static  ValidationResult? ValidateProjectLocation(string projectLocation, ValidationContext context)
    {

        if (string.IsNullOrWhiteSpace(projectLocation))
        {
            return new ValidationResult("Project location cannot be empty.");
        }

        if (!Directory.Exists(projectLocation))
        {
            return new ValidationResult("Project location does not exist.");
        }

        return ValidationResult.Success;
    }
    public static ValidationResult? ValidateProjectPath(string projectPath, ValidationContext context)
    {

        if (Path.Exists(projectPath))
        {
            return new ValidationResult("Project path is already exist");
        }


        return ValidationResult.Success;
    }
    [RelayCommand]
    public void Create()
    {
        ClearErrors();
        ValidateAllProperties();

        if (!HasErrors)
        {
            var desDirectorPath = Path.Combine(ProjectLocation, ProjectName);
            Directory.CreateDirectory(desDirectorPath);
            string templateDirectoryPath = Path.Combine(Environment.CurrentDirectory, "ProjectTemplate");

            // 复制所有文件
            foreach (var file in Directory.GetFiles(templateDirectoryPath))
            {
                string destFile = Path.Combine(desDirectorPath, Path.GetFileName(file));
                File.Copy(file, destFile, true); // 设置 overwrite 参数为 true，以覆盖同名文件
            }

            // 递归复制所有子文件夹及其内容
            foreach (var dir in Directory.GetDirectories(templateDirectoryPath))
            {
                string destDir = Path.Combine(desDirectorPath, Path.GetFileName(dir));
                CopyDirectory(dir, destDir);
            }

            var item = new UserProject
            {
                LastEditTime = DateTime.Now,
                ProjectName = ProjectName,
                ProjectPath = desDirectorPath
            };
            Projects.Add(item);
            JsonHelper.SerializeToFile(Projects, _appDataEngineInfoPath);

            OnSelectProjectChanged(item);
        }

        ValidateAllProperties();
    }


    // 递归复制文件夹
    private void CopyDirectory(string sourceDir, string destinationDir)
    {
        Directory.CreateDirectory(destinationDir);

        foreach (var file in Directory.GetFiles(sourceDir))
        {
            string destFile = Path.Combine(destinationDir, Path.GetFileName(file));
            File.Copy(file, destFile, true);
        }

        foreach (var dir in Directory.GetDirectories(sourceDir))
        {
            string destDir = Path.Combine(destinationDir, Path.GetFileName(dir));
            CopyDirectory(dir, destDir); // 递归复制子文件夹
        }
    }

    [RelayCommand]
    public void Delete()
    {
        Projects.Remove(SelectProject);
        JsonHelper.SerializeToFile(Projects, _appDataEngineInfoPath);
    }
    [RelayCommand]
    public void SelectLocation()
    {
        
        var openFileDialog = new OpenFolderDialog()
        {
            Title = "Select a project location",
        };
        if (Directory.Exists(ProjectLocation))
        {
            openFileDialog.DefaultDirectory = ProjectLocation;
        }
        if (openFileDialog.ShowDialog() == true)
        {
            ProjectLocation = openFileDialog.FolderName;
        }
    }

    partial void OnProjectLocationChanged(string value)
    {
        ProjectPath = Path.Combine(ProjectLocation, ProjectName);
    }

    partial void OnProjectNameChanged(string value)
    {
        ProjectPath = Path.Combine(ProjectLocation, ProjectName);
    }


    partial void OnSelectProjectChanged(UserProject value)
    {
        // 指定要启动的进程（例如Notepad.exe）
        string fileName = Path.Combine(Environment.CurrentDirectory, "NotEngine.Editor.exe");
        // 要传递的参数（例如文件路径）
        string arguments = value.ProjectPath;

        StartProcess(fileName, arguments);
    }

    private void StartProcess(string fileName, string arguments)
    {
        try
        {
            // 配置进程启动信息
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                UseShellExecute = true // 使用外壳程序执行，便于打开默认应用
            };

            // 启动进程
            Process process = Process.Start(startInfo);
            Application.Current.Shutdown(); ;
        }
        catch (System.ComponentModel.Win32Exception ex)
        {
            MessageBox.Show($"无法启动进程：{ex.Message}");
        }

        finally
        {
           
        }

    }
}