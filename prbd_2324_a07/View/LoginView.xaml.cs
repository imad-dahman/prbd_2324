﻿using PRBD_Framework;
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
using System.Windows.Shapes;

namespace prbd_2324_a07.View;

/// <summary>
/// Logique d'interaction pour LoginView.xaml
/// </summary>
public partial class LoginView : WindowBase
{
    public LoginView() {
        InitializeComponent();
    }

    private void btnCancel_Click(object sender, RoutedEventArgs e) {
        Close();
    }
   
}
