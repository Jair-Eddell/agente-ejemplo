using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace JuegoAdivinaElNumero
{
    /// <summary>
    /// Clase principal del juego Adivina el Número
    /// Versión: 1.3.0 - Persistencia de puntuaciones y estadísticas
    /// </summary>
    class Program
    {
        // NUEVO: Enum para niveles de dificultad
        enum Dificultad
        {
            Facil = 1,
            Normal = 2,
            Dificil = 3
        }

        // Archivo donde se guardan las puntuaciones
        const string ScoresFile = "scores.txt";

        /// <summary>
        /// Solicita al usuario la dificultad del juego.
        /// </summary>
        static Dificultad SeleccionarDificultad()
        {
            Console.WriteLine("\n📊 Selecciona la dificultad:");
            Console.WriteLine("1. Fácil (1-50, 10 intentos)");
            Console.WriteLine("2. Normal (1-100, 7 intentos)");
            Console.WriteLine("3. Difícil (1-200, 5 intentos)");

            while (true)
            {
                Console.Write("Opción: ");
                var entrada = Console.ReadLine();
                if (int.TryParse(entrada, out int opcion) &&
                    Enum.IsDefined(typeof(Dificultad), opcion))
                {
                    return (Dificultad)opcion;
                }
                Console.WriteLine("⚠️ Por favor, selecciona 1, 2 o 3");
            }
        }

        /// <summary>
        /// Calcula la puntuación en función de intentos restantes y dificultad.
        /// </summary>
        static int CalcularPuntuacion(int intentosRestantes, Dificultad dificultad)
        {
            // fórmula simple: base por dificultad * intentos restantes
            int baseFactor = dificultad == Dificultad.Facil ? 1 :
                             dificultad == Dificultad.Normal ? 2 : 3;
            return intentosRestantes * baseFactor * 100;
        }

        /// <summary>
        /// Guarda la puntuación del jugador en un archivo local (scores.txt).
        /// Formato: nombre|puntuacion|fecha
        /// </summary>
        static void GuardarPuntuacion(string jugador, int puntuacion)
        {
            try
            {
                var linea = $"{jugador}|{puntuacion}|{DateTime.UtcNow:o}";
                File.AppendAllLines(ScoresFile, new[] { linea });
            }
            catch
            {
                // no bloquear el juego si falla la persistencia
            }
        }

        /// <summary>
        /// Muestra las mejores puntuaciones registradas (top 5).
        /// </summary>
        static void MostrarEstadisticas()
        {
            if (!File.Exists(ScoresFile))
            {
                Console.WriteLine("\nNo hay puntuaciones registradas aún.");
                return;
            }

            try
            {
                var entries = File.ReadAllLines(ScoresFile)
                    .Select(l => l.Split('|'))
                    .Where(p => p.Length >= 2)
                    .Select(p => new { Name = p[0], Score = int.TryParse(p[1], out var s) ? s : 0, Date = (p.Length >= 3 ? p[2] : "") })
                    .OrderByDescending(x => x.Score)
                    .Take(5)
                    .ToList();

                Console.WriteLine("\n🏅 Mejores puntuaciones (Top 5):");
                foreach (var e in entries)
                {
                    Console.WriteLine($" - {e.Name}: {e.Score} pts ({e.Date})");
                }
            }
            catch
            {
                Console.WriteLine("No se pudieron leer las estadísticas.");
            }
        }

        static void Main(string[] args)
        {
            Console.Title = "🎲 Adivina el número v1.3 🎲";
            Console.ForegroundColor = ConsoleColor.Cyan;

            // NUEVO: selección de dificultad por el jugador
            var dificultad = SeleccionarDificultad();

            // Configuración según dificultad
            var random = new Random();
            var maxNumero = dificultad == Dificultad.Facil ? 50 :
                            dificultad == Dificultad.Normal ? 100 : 200;
            var maxIntentos = dificultad == Dificultad.Facil ? 10 :
                              dificultad == Dificultad.Normal ? 7 : 5;

            var numeroSecreto = random.Next(1, maxNumero + 1);
            var intentos = 0;
            var adivinado = false;

            Console.WriteLine($"\n🎯 Nivel: {dificultad}");
            Console.WriteLine($"🔢 Rango: 1-{maxNumero}");
            Console.WriteLine($"❤️ Intentos: {maxIntentos}");
            Console.WriteLine("---------------------------------------------");

            while (!adivinado && intentos < maxIntentos)
            {
                Console.Write($"\nIntento {intentos + 1}/{maxIntentos}. Tu número: ");
                string entrada = Console.ReadLine();
                if (!int.TryParse(entrada, out int intento))
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("⚠️ Por favor, escribe un número válido.");
                    Console.ResetColor();
                    continue;
                }

                intentos++;

                if (intento == numeroSecreto)
                {
                    adivinado = true;
                }
                else if (intentos >= maxIntentos)
                {
                    // fin del juego sin acierto
                }
                else if (intento < numeroSecreto)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Demasiado bajo. 📉 Intenta con un número más grande.");
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Demasiado alto. 📈 Intenta con un número más pequeño.");
                    Console.ResetColor();
                }
            }

            // Resultado final y puntuación
            if (adivinado)
            {
                var puntuacion = CalcularPuntuacion(maxIntentos - intentos, dificultad);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\n🏆 ¡Victoria! Puntuación: {puntuacion} puntos");
                Console.ResetColor();

                // Guardar puntuación con nombre por defecto "Jugador"
                GuardarPuntuacion("Jugador", puntuacion);

                Console.WriteLine($"📊 Intentos usados: {intentos}/{maxIntentos}");
                Console.WriteLine($"🔐 Número secreto: {numeroSecreto}");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n😢 ¡Game Over! El número era {numeroSecreto}");
                Console.ResetColor();
            }

            // Mostrar estadísticas guardadas (si hay)
            MostrarEstadisticas();

            Console.WriteLine("\nGracias por jugar. ¡Hasta la próxima! 👋");
            Console.WriteLine("Presiona cualquier tecla para salir...");
            Console.ReadKey();
        }
    }
}