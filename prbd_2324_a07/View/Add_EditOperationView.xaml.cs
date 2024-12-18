using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using prbd_2324_a07.Model;
using prbd_2324_a07.ViewModel;
using PRBD_Framework;


namespace prbd_2324_a07.View;

public partial class Add_EditOperationView: DialogWindowBase
{
    Tricount tricount1;
    public Add_EditOperationView(Tricount tricount,int user,Operation operation) {
        InitializeComponent();
        tricount1 = tricount;
        DataContext = new Add_EditOperationViewModel(tricount, user,operation) ;
        
    }
    public Add_EditOperationView(Tricount tricount, Operation operation) {
        InitializeComponent();
        tricount1 = tricount;
        this.Title = "Edit Operation";
        DataContext = new Add_EditOperationViewModel(tricount,operation);
    }
    public Add_EditOperationView() {
        InitializeComponent();
        DataContext = new Add_EditOperationViewModel();
    }
    



    
    private void NumericTextBox(object sender, TextCompositionEventArgs e) {
        Regex regex = new Regex("[^0-9,]+");
        e.Handled = regex.IsMatch(e.Text);
    }





}

