﻿using prbd_2324_a07.Model;
using PRBD_Framework;
using System.Windows.Input;

namespace prbd_2324_a07.ViewModel;

public class MainViewModel : ViewModelCommon
{
    public ICommand ReloadDataCommand { get; set; }
    public ICommand ResetDataCommand { get; set; }



    public MainViewModel() : base() {
        ReloadDataCommand = new RelayCommand(() => {
            // refuser un reload s'il y a des changements en cours
            if (Context.ChangeTracker.HasChanges()) return;
            // permet de renouveller le contexte EF
            App.ClearContext();

            // notifie tout le monde qu'il faut rafraîchir les données
            NotifyColleagues(ApplicationBaseMessages.MSG_REFRESH_DATA);
        });

        ResetDataCommand = new RelayCommand(() => {

            if (Context.ChangeTracker.HasChanges()) return;

            // permet de renouveller le contexte EF
            App.ClearContext();
            Context.Database.EnsureDeleted();
            Context.Database.EnsureCreated();

            // dans le cas User n existe pas dans la SeeData
           var user  = Context.Users.Find(CurrentUser.Id);
            if (user == null)
                NotifyColleagues(App.Messages.MSG_LOGOUT);


            // notifie tout le monde qu'il faut rafraîchir les données
            NotifyColleagues(App.Messages.MSG_CLOSE_TAB,new Tricount());
            NotifyColleagues(ApplicationBaseMessages.MSG_REFRESH_DATA);

        });
    }
    public string Title { get; } ="MyTricount"+ "("+MainViewModel.CurrentUser.Email+")";

    protected override void OnRefreshData() {
        // pour plus tard
    }
}