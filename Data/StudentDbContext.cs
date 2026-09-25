using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvidencijaStudenata.Models;
using Microsoft.EntityFrameworkCore;

namespace EvidencijaStudenata.Data;

/// <summary>
/// Kontekst baze podataka (Entity Framework Core), odgovara Kôdu 6 iz
/// poglavlja 4.4. završnog rada. Za razliku od jednoplatformske WPF verzije,
/// putanja do SQLite datoteke ne smije biti proizvoljna relativna putanja
/// (na Androidu i iOS-u takva mapa iz sigurnosnih razloga uopće ne mora biti
/// dostupna za pisanje), pa se koristi multiplatformni API
/// FileSystem.AppDataDirectory, opisan u poglavlju 3.1. i 3.5. rada, koji na
/// svakoj platformi vraća ispravnu, zapisivu putanju.
/// </summary>
public class StudentDbContext : DbContext
{
    private readonly string _connectionString;

    public StudentDbContext()
    {
        var dbPath = Path.Combine(
            FileSystem.AppDataDirectory, "studenti.db");
        _connectionString = $"Data Source={dbPath}";
    }

    public DbSet<Student> Students => Set<Student>();

    protected override void OnConfiguring(
        DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite(_connectionString);
    }
}
