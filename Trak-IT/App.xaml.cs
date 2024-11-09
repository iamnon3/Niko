using System.Data;
using Trak_IT.Scripts;

namespace Trak_IT
{
    public partial class App : Application
    {
        public readonly LocalDBServer _dbServer;
        public App(LocalDBServer dbServer)
        {
            InitializeComponent();
            MainPage = new AppShell();
            _dbServer = dbServer;
        }

        protected override async void OnStart()
        {
            SheetService service = new SheetService();
            await service.CopyJsonKeyToAppDataDirectoryAsync();
            service.GetSheetsService();
            await _dbServer.Resets();
            await _dbServer.GetUser();
            await _dbServer.GetStudent();
        }
    }
}
