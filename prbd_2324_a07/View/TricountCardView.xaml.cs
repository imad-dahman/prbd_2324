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
using PRBD_Framework;

namespace prbd_2324_a07.View
{
    /// <summary>
    /// Logique d'interaction pour TricountCardView.xaml
    /// </summary>
    public partial class TricountCardView : UserControlBase
    {
        public TricountCardView() {
            InitializeComponent();
        }

        private void StackPanel_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e) {

        }
    }
}
