using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace JuegoAdivinaElNumero
{
    /// <summary>
    /// Clase principal del juego Adivina el Número
    /// Versión: 1.4.0 - Menú, nombre de jugador, historial de partidas y opción de reset de puntuaciones
    /// </summary>
    class Program
    {
        // Enum para niveles de dificultad
        enum Dificultad
        {
            Facil = 1,
            Normal = 2,
            Dificil = 3
        }

        // Archivos para puntuaciones y registro de partidas
        const string ScoresFile = "scores.txt";
        const string GamesLogFile = "games.log";

        static void Main(string[] args)
        {
            Console.Title = "🎲 Adivina el número v1.4 🎲";
            Console.ForegroundColor = ConsoleColor.Cyan;

            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== Adivina el Número ===");
                Console.WriteLine("1) Jugar");
                Console.WriteLine("2) Ver estadísticas");
                Console.WriteLine("3) Resetear puntuaciones");
                Console.WriteLine("4) Salir");
                Console.Write("\nSelecciona una opción: ");

                var opt = Console.ReadLine();
                if (!int.TryParse(opt, out int opcionMenu)) opcionMenu = 0;

                if (opcionMenu == 1)
                {
                    Jugar();
                }
                else if (opcionMenu == 2)
                {
                    MostrarEstadisticas();
                    Pausa();
                }
                else if (opcionMenu == 3)
                {
                    ResetearPuntuaciones();
                    Pausa("Puntuaciones reseteadas. Presiona una tecla para continuar...");
                }
                else if (opcionMenu == 4)
                {
                    Console.WriteLine("Saliendo... ¡hasta la próxima!");
                    break;
                }
                else
                {
                    Console.WriteLine("Opción inválida.");
                    Pausa();
                }
            }
        }

        static void Jugar()
        {
            Console.Write("\nEscribe tu nombre (enter para 'Jugador'): ");
            var jugador = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(jugador)) jugador = "Jugador";

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

                // Guardar puntuación con el nombre del jugador
                GuardarPuntuacion(jugador, puntuacion, dificultad);

                Console.WriteLine($"📊 Intentos usados: {intentos}/{maxIntentos}");
                Console.WriteLine($"🔐 Número secreto: {numeroSecreto}");

                // Registrar partida en el log
                RegistrarPartida(jugador, dificultad, puntuacion, true);
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n😢 ¡Game Over! El número era {numeroSecreto}");
                Console.ResetColor();

                // Registrar partida fallida con puntuación 0
                RegistrarPartida("Jugador", dificultad, 0, false);
            }

            // Mostrar estadísticas guardadas (si hay)
            Console.WriteLine();
            MostrarTopLocal(3);
            Pausa();
        }

        static void Pausa(string mensaje = "Presiona cualquier tecla para continuar...")
        {
            Console.WriteLine();
            Console.WriteLine(mensaje);
            Console.ReadKey(true);
        }

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
            int baseFactor = dificultad == Dificultad.Facil ? 1 :
                             dificultad == Dificultad.Normal ? 2 : 3;
            return Math.Max(0, intentosRestantes) * baseFactor * 100;
        }

        /// <summary>
        /// Guarda la puntuación del jugador en un archivo local (scores.txt).
        /// Formato: nombre|puntuacion|fecha|dificultad
        /// </summary>
        static void GuardarPuntuacion(string jugador, int puntuacion, Dificultad dificultad)
        {
            try
            {
                var linea = $"{jugador}|{puntuacion}|{DateTime.UtcNow:o}|{(int)dificultad}";
                File.AppendAllLines(ScoresFile, new[] { linea });
            }
            catch
            {
                // no bloquear el juego si falla la persistencia
            }
        }

        /// <summary>
        /// Registra la partida en games.log (historial simple).
        /// </summary>
        static void RegistrarPartida(string jugador, Dificultad dificultad, int puntuacion, bool victoria)
        {
            try
            {
                var resultado = victoria ? "WIN" : "LOSE";
                var linea = $"{DateTime.UtcNow:o}|{jugador}|{(int)dificultad}|{puntuacion}|{resultado}";
                File.AppendAllLines(GamesLogFile, new[] { linea });
            }
            catch
            {
                // ignorar errores de log
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
                    .Where(p => p.Length >= 3)
                    .Select(p => new { Name = p[0], Score = int.TryParse(p[1], out var s) ? s : 0, Date = (p.Length >= 3 ? p[2] : ""), Difficulty = (p.Length >= 4 ? p[3] : "0") })
                    .OrderByDescending(x => x.Score)
                    .Take(5)
                    .ToList();

                Console.WriteLine("\n🏅 Mejores puntuaciones (Top 5):");
                foreach (var e in entries)
                {
                    var diffText = e.Difficulty == "1" ? "Fácil" : e.Difficulty == "2" ? "Normal" : "Difícil";
                    Console.WriteLine($" - {e.Name}: {e.Score} pts ({diffText}) - {e.Date}");
                }
            }
            catch
            {
                Console.WriteLine("No se pudieron leer las estadísticas.");
            }
        }

        /// <summary>
        /// Muestra el top N local (utilizado tras cada partida).
        /// </summary>
        static void MostrarTopLocal(int topN)
        {
            if (!File.Exists(ScoresFile)) return;
            try
            {
                var entries = File.ReadAllLines(ScoresFile)
                    .Select(l => l.Split('|'))
                    .Where(p => p.Length >= 2)
                    .Select(p => new { Name = p[0], Score = int.TryParse(p[1], out var s) ? s : 0 })
                    .OrderByDescending(x => x.Score)
                    .Take(topN)
                    .ToList();

                Console.WriteLine($"\nTop {topN}:");
                foreach (var e in entries)
                {
                    Console.WriteLine($" - {e.Name}: {e.Score} pts");
                }
            }
            catch { }
        }

        /// <summary>
        /// Borra el fichero de puntuaciones.
        /// </summary>
        static void ResetearPuntuaciones()
        {
            try
            {
                if (File.Exists(ScoresFile)) File.Delete(ScoresFile);
                if (File.Exists(GamesLogFile)) File.Delete(GamesLogFile);
            }
            catch
            {
                Console.WriteLine("No se pudieron resetear las puntuaciones.");
            }
        }
    }
}