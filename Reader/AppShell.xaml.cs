
using Reader.Views;

namespace Reader
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute("ReadView", typeof(ReadView));
        }
    }
}
