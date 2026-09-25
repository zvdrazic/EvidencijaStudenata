using EvidencijaStudenata.ViewModels;
using System.Globalization;

namespace EvidencijaStudenata;

public partial class MainPage : ContentPage
{
    private readonly StudentViewModel _viewModel;

    public MainPage()
    {
        InitializeComponent();
        _viewModel = new StudentViewModel();
        BindingContext = _viewModel;
    }

    private void OnNoviStudentPoljePromijenjeno(object? sender, TextChangedEventArgs e)
    {
        ((RelayCommand)_viewModel.DodajStudentaCommand).RaiseCanExecuteChanged();
    }

    /// <summary>
    /// Ograničava unos u polje Prosjek na znamenke i jedan zarez kao decimalni
    /// znak (izbjegava da se točka pogrešno protumači kao razdjelnik
    /// tisućica, vidi poglavlje 4.7. rada)
    /// </summary>
    private void OnProsjekTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (sender is not Entry entry) return;

        var noviTekst = e.NewTextValue ?? string.Empty;
        var procisceno = string.Empty;
        var zarezVecUnesen = false;

        foreach (var znak in noviTekst)
        {
            if (char.IsDigit(znak))
            {
                procisceno += znak;
            }
            else if (znak == ',' && !zarezVecUnesen)
            {
                procisceno += znak;
                zarezVecUnesen = true;
            }
        }

        if (procisceno != noviTekst)
            entry.Text = procisceno;

        ((RelayCommand)_viewModel.DodajStudentaCommand).RaiseCanExecuteChanged();
        ((RelayCommand)_viewModel.SpremiIzmjeneCommand).RaiseCanExecuteChanged();
    }
}