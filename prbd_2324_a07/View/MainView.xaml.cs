using PRBD_Framework;
using prbd_2324_a07.Model;
using System.Windows.Controls;
using prbd_2324_a07.ViewModel;

namespace prbd_2324_a07.View;

public partial class MainView : WindowBase
{
    public MainView() {
        InitializeComponent();

        Register<Tricount>(App.Messages.MSG_NEW_MEMBER,
           tricount => DoDisplayTricount(tricount, true));
        Register<Tricount>(App.Messages.MSG_NEW_DETAIL,
          tricount => DoDisplayTricount());

        Register<Tricount>(App.Messages.MSG_DISPLAY_MEMBER,
            tricount => DoDisplayTricount(tricount, false));

        Register<Tricount>(App.Messages.MSG_PSEUDO_CHANGED,
            tricount => DoRenameTab(string.IsNullOrEmpty(tricount.Title) ? "<New Tricount>" : tricount.Title));
       

        Register<Tricount>(App.Messages.MSG_CLOSE_TAB,
            tricount => DoCloseTab(tricount));
    }

    private void MenuLogout_Click(object sender, System.Windows.RoutedEventArgs e) {
        NotifyColleagues(App.Messages.MSG_LOGOUT);
    }

    private void DoDisplayTricount(Tricount tricount, bool isNew) {
        if (tricount != null)
            OpenTab(isNew ? "<New Tricount>" : tricount.Title, tricount.Title, () => new TricountDetailView(tricount, isNew));
    }
    private void DoDisplayTricount() {
            OpenTab( "imad" , "", () => new detailView());
    }


    private void OpenTab(string header, string tag, Func<UserControlBase> createView) {
        var tab = tabControl.FindByTag(tag);
        if (tab == null)
            tabControl.Add(createView(), header, tag);
        else
            tabControl.SetFocus(tab);
    }

    private void DoRenameTab(string header) {
        if (tabControl.SelectedItem is TabItem tab) {
            MyTabControl.RenameTab(tab, header);
            tab.Tag = header;
        }
    }

    private void DoCloseTab(Tricount tricount) {
        tabControl.CloseByTag(string.IsNullOrEmpty(tricount.Title) ? "<New Tricount>" : tricount.Title);
        tabControl.CloseByTag(tricount.IsAdded ?  tricount.Title : "<New Tricount>");

    }
}