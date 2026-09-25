using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace EvidencijaStudenata.ViewModels;

/// <summary>
/// Jednostavna vlastita implementacija sučelja ICommand, korištena umjesto
/// pisanja događaja u pozadinskom kodu (code-behind), u skladu s MVVM
/// obrascem opisanim u poglavlju 3.4. završnog rada. Za razliku od WPF-a,
/// .NET MAUI nema statičku klasu CommandManager koja automatski ponovno
/// provjerava CanExecute, pa se to ovdje radi eksplicitnim pozivom metode
/// RaiseCanExecuteChanged nakon promjene stanja koje utječe na naredbu -
/// isto se ponašanje ostvaruje na svakoj ciljnoj platformi bez izmjena koda.
/// </summary>
public class RelayCommand : ICommand
{
    private readonly Action<object?> _execute;
    private readonly Predicate<object?>? _canExecute;

    public RelayCommand(Action<object?> execute, Predicate<object?>? canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    public bool CanExecute(object? parameter) => _canExecute?.Invoke(parameter) ?? true;

    public void Execute(object? parameter) => _execute(parameter);

    public event EventHandler? CanExecuteChanged;

    /// <summary>Ručno pokretanje ponovne provjere CanExecute (npr. nakon promjene stanja).</summary>
    public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}
