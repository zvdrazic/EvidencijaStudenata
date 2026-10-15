using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using EvidencijaStudenata.Data;
using EvidencijaStudenata.Models;
using Microsoft.EntityFrameworkCore;

namespace EvidencijaStudenata.ViewModels;

/// <summary>
/// ViewModel glavne stranice aplikacije, odgovara opisu iz poglavlja 4.6.
/// završnog rada ("Implementacija poslovne logike"). Sadrži kolekciju
/// studenata, podatke trenutačno unesenog/odabranog studenta te naredbe za
/// dodavanje, izmjenu i brisanje zapisa. Klasa ne sadrži nijednu referencu
/// na konkretnu platformsku kontrolu ni API, zbog čega se, kako je opisano u
/// poglavlju 4.7., bez izmjena koristi i na Windows i na Android izdanju
/// aplikacije.
/// </summary>
public class StudentViewModel : INotifyPropertyChanged
{
    private readonly StudentDbContext _context;

    public ObservableCollection<Student> Students { get; } = new();

    private Student _noviStudent = new();
    public Student NoviStudent
    {
        get => _noviStudent;
        set { _noviStudent = value; OnPropertyChanged(); }
    }

    private Student? _selectedStudent;
    public Student? SelectedStudent
    {
        get => _selectedStudent;
        set
        {
            _selectedStudent = value;
            OnPropertyChanged();
            ((RelayCommand)ObrisiStudentaCommand).RaiseCanExecuteChanged();

            // Odabirom postojećeg zapisa unosna se polja (povezana s
            // NoviStudent, vidi MainPage.xaml) pune trenutačnim vrijednostima
            // odabranog studenta, čime se ista polja koriste i za izmjenu
            // postojećeg zapisa, ne samo za unos novog.
            if (value is not null)
            {
                NoviStudent = new Student
                {
                    Ime = value.Ime,
                    Prezime = value.Prezime,
                    Jmbag = value.Jmbag,
                    GodinaStudija = value.GodinaStudija,
                    ProsjekOcjena = value.ProsjekOcjena,
                    Email = value.Email,
                };
            }

            // Promjena odabira mijenja i koji zapis treba izuzeti iz provjere
            // jedinstvenosti JMBAG-a (vidi OsvjeziValidaciju), pa i naredba
            // Dodaj treba ponovnu provjeru, ne samo Spremi izmjene.
            OsvjeziValidaciju();
        }
    }

    public ICommand DodajStudentaCommand { get; }
    public ICommand SpremiIzmjeneCommand { get; }
    public ICommand ObrisiStudentaCommand { get; }

    private string _poruka = string.Empty;
    /// <summary>
    /// Kratka poruka prikazana korisniku kad JMBAG unesen u obrazac već
    /// pripada drugom studentu (vidi poglavlje 4.4. rada - jedinstvenost
    /// JMBAG-a). Prazan niz kad nema greške.
    /// </summary>
    public string Poruka
    {
        get => _poruka;
        private set { _poruka = value; OnPropertyChanged(); }
    }

    public StudentViewModel() : this(new StudentDbContext())
    {
    }

    public StudentViewModel(StudentDbContext context)
    {
        _context = context;
        // Pojednostavljeno stvaranje baze podataka za potrebe demonstracijske
        // aplikacije (umjesto migracija spomenutih u teorijskom dijelu rada).
        _context.Database.EnsureCreated();

        foreach (var student in _context.Students.ToList())
            Students.Add(student);

        DodajStudentaCommand = new RelayCommand(
        _ => DodajStudenta(),
        _ => !string.IsNullOrWhiteSpace(NoviStudent.Ime) &&
             !string.IsNullOrWhiteSpace(NoviStudent.Prezime) &&
             NoviStudent.ProsjekOcjena >= 0 && NoviStudent.ProsjekOcjena <= 5 &&
             !JmbagJeZauzet(NoviStudent.Jmbag, isključiId: 0));

        SpremiIzmjeneCommand = new RelayCommand(
            _ => SpremiIzmjene(),
            _ => SelectedStudent is not null &&
                 NoviStudent.ProsjekOcjena >= 0 && NoviStudent.ProsjekOcjena <= 5 &&
                 !JmbagJeZauzet(NoviStudent.Jmbag, SelectedStudent.Id));

        ObrisiStudentaCommand = new RelayCommand(
            _ => ObrisiStudenta(),
            _ => SelectedStudent is not null);
    }

    /// <summary>
    /// Provjerava postoji li već student s istim JMBAG-om (osim studenta čiji
    /// je Id jednak isključiId - koristi se pri izmjeni postojećeg zapisa,
    /// koji smije zadržati vlastiti JMBAG). Prazan JMBAG se ne smatra sukobom
    /// - to već sprječava zasebna provjera obveznih polja.
    /// </summary>
    private bool JmbagJeZauzet(string jmbag, int isključiId) =>
        !string.IsNullOrWhiteSpace(jmbag) &&
        Students.Any(s => s.Jmbag == jmbag && s.Id != isključiId);

    /// <summary>
    /// Ponovno provjerava jedinstvenost unesenog JMBAG-a i prema potrebi
    /// postavlja korisniku vidljivu poruku, te osvježava CanExecute stanje
    /// naredbi Dodaj i Spremi izmjene. Poziva se iz MainPage.xaml.cs pri
    /// svakoj promjeni relevantnih polja obrasca (vidi poglavlje 4.4. rada).
    /// </summary>
    public void OsvjeziValidaciju()
    {
        var sukobPriDodavanju = JmbagJeZauzet(NoviStudent.Jmbag, isključiId: 0);
        var sukobPriIzmjeni = SelectedStudent is not null &&
            JmbagJeZauzet(NoviStudent.Jmbag, SelectedStudent.Id);

        Poruka = (sukobPriDodavanju || sukobPriIzmjeni)
            ? "Student s unesenim JMBAG-om već postoji."
            : string.Empty;

        ((RelayCommand)DodajStudentaCommand).RaiseCanExecuteChanged();
        ((RelayCommand)SpremiIzmjeneCommand).RaiseCanExecuteChanged();
    }

    private void DodajStudenta()
    {
        NoviStudent.ProsjekOcjena = Math.Clamp(NoviStudent.ProsjekOcjena, 0, 5);

        _context.Students.Add(NoviStudent);
        try
        {
            _context.SaveChanges();
        }
        catch (DbUpdateException)
        {
            // Jedinstveni indeks nad Jmbag (StudentDbContext.OnModelCreating)
            // odbio je zapis na razini baze - krajnja sigurnosna mjera za
            // slučaj da provjera iznad (JmbagJeZauzet) bude zaobiđena.
            _context.Entry(NoviStudent).State = EntityState.Detached;
            Poruka = "Student s unesenim JMBAG-om već postoji.";
            return;
        }

        Students.Add(NoviStudent);
        NoviStudent = new Student();
    }

    private void SpremiIzmjene()
    {
        if (SelectedStudent is null) return;

        NoviStudent.ProsjekOcjena = Math.Clamp(NoviStudent.ProsjekOcjena, 0, 5);

        // Vrijednosti unesene u obrazac (NoviStudent) prepisuju se na
        // odabrani zapis prije spremanja.
        SelectedStudent.Ime = NoviStudent.Ime;
        SelectedStudent.Prezime = NoviStudent.Prezime;
        SelectedStudent.Jmbag = NoviStudent.Jmbag;
        SelectedStudent.GodinaStudija = NoviStudent.GodinaStudija;
        SelectedStudent.ProsjekOcjena = NoviStudent.ProsjekOcjena;
        SelectedStudent.Email = NoviStudent.Email;

        try
        {
            _context.SaveChanges();
        }
        catch (DbUpdateException)
        {
            // Vidi odgovarajući catch blok u DodajStudenta - ista krajnja
            // sigurnosna mjera na razini baze.
            _context.Entry(SelectedStudent).Reload();
            Poruka = "Student s unesenim JMBAG-om već postoji.";
            return;
        }

        // Klasa Student ne implementira INotifyPropertyChanged, pa CollectionView
        // ne uočava izmjenu vrijednosti postojećeg objekta. Zamjena stavke NA
        // ISTOM mjestu istom referencom (Students[indeks] = SelectedStudent)
        // pokazala se nepouzdanom - CollectionView zna izostaviti ili odgoditi
        // ponovno iscrtavanje retka kad su "stara" i "nova" stavka doslovno isti
        // objekt. Pouzdanije je stavku ukloniti pa je odmah ponovno umetnuti na
        // isto mjesto - dvije jasne obavijesti umjesto jedne dvosmislene.
        var odabrani = SelectedStudent;
        var indeks = Students.IndexOf(odabrani);
        if (indeks < 0) return;

        Students.RemoveAt(indeks);
        Students.Insert(indeks, odabrani);
        SelectedStudent = odabrani;
    }

    private void ObrisiStudenta()
    {
        if (SelectedStudent is null) return;
        _context.Students.Remove(SelectedStudent);
        _context.SaveChanges();
        Students.Remove(SelectedStudent);
        SelectedStudent = null;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}