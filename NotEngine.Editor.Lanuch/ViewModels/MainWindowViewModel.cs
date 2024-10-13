using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using NotEngine.Editor.Lanuch.Models;

namespace NotEngine.Editor.Lanuch.ViewModels;

internal partial class MainWindowViewModel: ObservableObject
{
    public ObservableCollection<UserProject> Projects { get; set; }
    [ObservableProperty] public string _newProjectPath;


    public MainWindowViewModel()
    {
        Projects  = new ObservableCollection<UserProject>();
        
    }
}