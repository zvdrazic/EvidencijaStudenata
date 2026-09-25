# Evidencija studenata — .NET MAUI

Demonstracijska multiplatformna aplikacija izrađena u sklopu završnog rada
"Izrada GUI aplikacija u .NET Coreu" (VSITE, 2026., Zvonimir Dražić).
Aplikacija korisniku omogućuje unos, pregled, izmjenu i brisanje (CRUD)
podataka o studentima, uz trajnu pohranu putem SQLite baze podataka.

## Tehnologije

- .NET 8 (LTS)
- .NET MAUI
- C#
- Entity Framework Core + SQLite
- MVVM arhitekturalni obrazac (vlastita implementacija RelayCommand)

## Testirane platforme

- Windows (desktop)
- Android — emulator Pixel 7, API 34.0 (Android 14)

## Pokretanje

1. Visual Studio 2026 s instaliranim radnim opterećenjem ".NET Multi-platform App UI development"
2. .NET 8 SDK (verzija pinana u `global.json`)
3. Otvoriti `EvidencijaStudenata.slnx`, odabrati ciljnu platformu (Windows Machine / Android Emulator) i pokrenuti (F5)

## Napomena

Aplikacija je izrađena kao ilustrativan primjer za potrebe završnog rada,
a ne kao potpuno proizvodno rješenje.
