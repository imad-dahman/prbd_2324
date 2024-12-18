using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.EntityFrameworkCore.Metadata;
using prbd_2324_a07.Model;
using prbd_2324_a07.ViewModel;
using PRBD_Framework;

namespace prbd_2324_a07.View;

/// <summary>
/// Logique d'interaction pour TricountDetailView.xaml
/// </summary>
public partial class TricountDetailView : UserControlBase
{
    private readonly TricountDetailViewModel _vm;

    public TricountDetailView(Tricount tricount,bool isNew) {
        InitializeComponent();
        //Register<Operation>(App.Messages.MSG_NEW_MEMBER,
        //   operation => DoDisplayMember(operation, true));

        //Register<Operation>(App.Messages.MSG_DISPLAY_MEMBER,
        //    operation => DoDisplayMember(operation, false));
        DataContext = _vm = new TricountDetailViewModel(tricount, isNew);
    }
    //private void DoDisplayMember(Member member, bool isNew) {
    //    if (member != null)
    //        OpenTab(isNew ? "<New Member>" : member.Pseudo, member.Pseudo, () => new MemberDetailView(member, isNew));
    //}

    //private void DoDisplayComponentization() {
    //    OpenTab("Componentization", "Componentization", () => new ComponentizationView());
    //}

    //private void OpenTab(string header, string tag, Func<UserControlBase> createView) {
    //    var tab = tabControl.FindByTag(tag);
    //    if (tab == null)
    //        tabControl.Add(createView(), header, tag);
    //    else
    //        tabControl.SetFocus(tab);
    //}

    private void calendar1_Loaded(object sender, RoutedEventArgs e) {
        DatePicker cal = sender as DatePicker;
        cal.DisplayDateEnd = DateTime.Today;
    }
   

}
