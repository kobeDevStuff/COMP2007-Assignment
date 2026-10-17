# C# report: code

This is the codebase for the COMP2007 final assignment. Every code block in the report comes from these files.

The example task is an order processor. It reads `data/orders.csv`, rejects bad lines, groups lines into orders,
applies a 10% discount to orders over 500.00 and prints the totals.

| Folder | What it is | Report section |
|---|---|---|
| `csharp1-style` | the order processor in C# 1.0 | 2.1, 2.2 |
| `csharp1-analysis` | C# 1.0 demos for the nine characteristics | 2.3 |
| `kotlin-comparison` | the same ideas in Kotlin | 2.3, 2.5 |
| `modernisation-prototype` | units of measure for money, in modern C# | 2.6 |

## Dependencies

- .NET SDK 8.0 or later
- Kotlin compiler and a Java runtime (tested with Kotlin 1.3.31, Java 21)

## How to run it

For the three C# folders:
```
dotnet run
```

For the Kotlin folder:
```
kotlinc AnalysisDemos.kt -include-runtime -d demos.jar && java -jar demos.jar
```

The project uses C# version 1.0 because it was what was originally released in 2001.

## Expected output

- `csharp1-style`: 4 rejected lines, then accepted 6, rejected 4, grand total 1536.34
- `modernisation-prototype`: 76.98 (wrong on purpose), then 64.98 and 468.00

## Code that should not compile

Some report examples are compile errors on purpose, use the following commands to test them:

```
dotnet build -p:DefineConstants=ERROR_ASSIGN_IN_IF # csharp1-analysis, error CS0029
dotnet build -p:DefineConstants=ERROR_ADD_QUANTITY # modernisation-prototype, error CS0019
dotnet build -p:DefineConstants=ERROR_MIX_CURRENCY # modernisation-prototype, error CS0019
```

## Proposed syntax (section 2.6)

Note: This does NOT work with the actual compiler.

```csharp
[Measure] struct AUD { }
[Measure] struct Each { }

decimal<AUD/Each> price = 24.99m<AUD/Each>;
int<Each> quantity = 2<Each>;
decimal<AUD> total = price * quantity; // 49.98 AUD
total = total + quantity; // compile error: cannot add AUD and Each
decimal plain = (decimal)total; // an explicit cast removes the unit
```
