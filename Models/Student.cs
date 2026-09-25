using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvidencijaStudenata.Models;

/// <summary>
/// Model klase Student - odgovara Kôdu 5 iz teorijskog dijela završnog rada
/// (poglavlje 4.4., "Model podataka i baza podataka"). Model je jednak bez
/// obzira na to na kojoj se platformi aplikacija pokreće.
/// </summary>
public class Student
{
    public int Id { get; set; }
    public string Ime { get; set; } = string.Empty;
    public string Prezime { get; set; } = string.Empty;
    public string Jmbag { get; set; } = string.Empty;
    public int GodinaStudija { get; set; }
    public double ProsjekOcjena { get; set; }
    public string Email { get; set; } = string.Empty;
}
