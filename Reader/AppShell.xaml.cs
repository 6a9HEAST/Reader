
using Reader.Views;

namespace Reader
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute("ReadView", typeof(ReadView));
            Routing.RegisterRoute("SearchView", typeof(SearchView)); 
            Routing.RegisterRoute("ContentView", typeof(Reader.Views.ContentView));
        }
    }
}
