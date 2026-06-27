using System;
using System.Collections.Generic;

namespace Zeno.App.Services;

public static class QuoteService
{
    private static readonly List<string> WaterQuotes =
    [
        "O corpo é o templo da razão. Nutri-lo é um ato de disciplina, não de fraqueza.",
        "Água é a única coisa que o corpo nunca negocia.",
        "Cuida primeiro do que está em teu poder. Começar pelo corpo é começar pelo fundamento.",
        "Assim como o rio não questiona seu curso, o corpo não questiona sua necessidade.",
        "A disciplina começa nas pequenas escolhas. Um copo de cada vez.",
        "Não esperes sentir sede para agir. O sábio age antes da necessidade.",
    ];

    private static readonly Random _rng = new();

    public static string GetWaterQuote() =>
        WaterQuotes[_rng.Next(WaterQuotes.Count)];
}
