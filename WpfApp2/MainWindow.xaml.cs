using System.ComponentModel;
using System.Windows;
using Timer = System.Timers.Timer;


namespace WpfApp2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            // 示例对象
            var person = new Person {  Age = 30 };

            // 将该对象展示在 PropertyGrid 中
            PropertyGrid.SelectedObject = person;

            var _timer = new Timer(1000);
            _timer.Elapsed += (sender, args) =>
            {
                Console.WriteLine(person.Age);
            };
            
        }
    }

    // 示例类
    public class Person
    {
        [ExtenderProvidedProperty]
        public List<int> Lafa { get; set; } = new List<int>(){1,2,3,4};
        private string _name = "FAFA";

        public string Name
        {
            get => _name;
            set => SetName(value);
        }

        public void SetName(string name)
        {
            Console.WriteLine("set name");
            _name = name;
        }

     
        public int Age { get; set; }
    }
}