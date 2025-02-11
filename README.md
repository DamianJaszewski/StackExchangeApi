# Tag Management API

Tag Management API to aplikacja umożliwiająca zarządzanie tagami, z funkcjami generowania danych, obliczania procentowych udziałów oraz stronicowania wyników.

## Funkcje API

Plik OpenApi - [OpenApiFile](./swagger.json)

### 1. **Populate Data**
Endpoint do uzupełniania danych przykładowych.
- **URL**: `/populate`
- **Metoda**: `POST`
- **Opis**: Wypełnia bazę danych przykładowymi tagami.
- **Odpowiedź**: 
  - `200 OK`: `Data populated successfully`

### 2. **Get Tags Percentage**
Endpoint do obliczenia udziału procentowego każdego z tagów.
- **URL**: `/percentage`
- **Metoda**: `GET`
- **Opis**: Zwraca listę tagów z obliczonym procentowym udziałem liczby wystąpień.
- **Odpowiedź**:
  - `200 OK`: Lista tagów i ich procentowy udział.

### 3. **Get Paginated Tags**
Endpoint do pobierania stronicowanej listy tagów.
- **URL**: `/paginate`
- **Metoda**: `GET`
- **Parametry zapytania**:
  - `Page`: numer strony (domyślnie: 1)
  - `PageSize`: liczba tagów na stronie (domyślnie: 10)
  - `OrderBy`: sortowanie wyników (`count` lub `name`)
  - `IsAscending`: kierunek sortowania (wartość logiczna)
- **Opis**: Pobiera stronicowaną listę tagów z możliwością sortowania.
- **Odpowiedź**:
  - `200 OK`: Stronicowana lista tagów.

## Wersje i użyte technologie

- **Platforma**: .NET 8
- **Baza danych**: Microsoft SQL Server (MSSQL)
- **Konteneryzacja**: Projekt zawiera konfigurację Dockera (W trakcie, została do skofigurowania baza dancych).

## Paczki

W projekcie wykorzystano następujące paczki NuGet:

- **Entity Framework Core**:
  - `Microsoft.EntityFrameworkCore`
  - `Microsoft.EntityFrameworkCore.SqlServer`
  - `Microsoft.EntityFrameworkCore.Tools`
- **Testowanie**:
  - `xUnit`
  - `xUnit.runner.visualstudio`
  - `Microsoft.NET.Test.Sdk`
  - `Moq.AutoMock`
  - `Microsoft.EntityFrameworkCore.InMemory`

## Jak uruchomić projekt

1. **Wymagania**:
   - [.NET 8 SDK](https://dotnet.microsoft.com/)
   - SQL Server (lub InMemoryDatabase do testów).

2. **Instalacja**:
   - Sklonuj repozytorium:
     ```bash
     git clone <repo_url>
     cd <repo_folder>
     ```
   - Przygotuj bazę danych (konfiguracja w `appsettings.json`).
   - Uruchom projekt:
     ```bash
     dotnet run
     ```

3. **Testowanie**:
   - Wykonaj testy jednostkowe i integracyjne:
     ```bash
     dotnet test
     ```

## Struktura Kodu

### Kontroler: `TagController`
Obsługuje zapytania HTTP i komunikuje się z serwisem `TagService`. Odpowiada za takie operacje, jak generowanie danych, obliczanie procentów i stronicowanie tagów.

### Serwis: `TagService`
Zarządza logiką biznesową API:
- **`PopulateDataAsync`**: Wypełnia dane.
- **`CalculateTagsPercentageAsync`**: Oblicza procentowy udział tagów.
- **`GetPaginatedTagsAsync`**: Pobiera stronicowaną listę tagów.

### Model: `Item`
Reprezentuje pojedynczy tag z właściwościami:
- `Name`
- `Count`
- `HasSynonyms`
- `IsModeratorOnly`

## Testy

### 1. **Testy Integracyjne**
Testy integrują kontroler `TagController` z serwisem `TagService` i bazą danych:
- Test poprawności obliczeń udziałów procentowych.
- Test poprawności stronicowania.

### 2. **Testy Jednostkowe**
Testy logiki biznesowej w `TagService`:
- Test poprawnego obliczenia procentowych udziałów.
- Test poprawnego stronicowania listy.

Przykładowy test:
```csharp
[Fact]
public async Task GetTagsPercentage_GetRequest_ReturnsCorrectPercentages()
{
    // Arrange
    var items = new List<Item>
    {
        CreateExampleItem(10),
        CreateExampleItem(30),
        CreateExampleItem(60)
    };

    _context.Items.AddRange(items);
    await _context.SaveChangesAsync();

    // Act
    var result = await _tagController.GetTagsPercentage() as OkObjectResult;
    var tagsPercentage = result?.Value as List<TagsPercentage>;

    // Assert
    Assert.NotNull(tagsPercentage);
    Assert.Equal(3, tagsPercentage.Count);
    Assert.Equal("10%", tagsPercentage.First(x => x.TagId == 1).Percentage);
}
