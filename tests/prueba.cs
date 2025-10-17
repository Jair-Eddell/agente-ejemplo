using System;

namespace JuegoAdivinaElNumero
{
    /// <summary>
    /// Clase principal del juego Adivina el Número
    /// Versión: 1.2.0 - Agregado sistema de dificultad y puntuación
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

        // NUEVO: Método para seleccionar dificultad
        static Dificultad SeleccionarDificultad()
        {
            Console.WriteLine("\n📊 Selecciona la dificultad:");
            Console.WriteLine("1. Fácil (1-50, 10 intentos)");
            Console.WriteLine("2. Normal (1-100, 7 intentos)");
            Console.WriteLine("3. Difícil (1-200, 5 intentos)");
            
            while (true)
            {
                if (int.TryParse(Console.ReadLine(), out int opcion) && 
                    Enum.IsDefined(typeof(Dificultad), opcion))
                {
                    return (Dificultad)opcion;
                }
                Console.WriteLine("⚠️ Por favor, selecciona 1, 2 o 3");
            }
        }

        // NUEVO: Método para calcular puntuación
        static int CalcularPuntuacion(int intentosRestantes, Dificultad dificultad)
        {
            return intentosRestantes * (int)dificultad * 100;
        }

        static void Main(string[] args)
        {
            Console.Title = "🎲 Adivina el número v1.2 🎲";
            Console.ForegroundColor = ConsoleColor.Cyan;

            // NUEVO: Selección de dificultad
            var dificultad = SeleccionarDificultad();
            
            // MODIFICADO: Configuración según dificultad
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

            // ...existing code for game loop...

            // MODIFICADO: Mensaje final con puntuación
            if (adivinado)
            {
                var puntuacion = CalcularPuntuacion(maxIntentos - intentos, dificultad);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\n🏆 ¡Victoria! Puntuación: {puntuacion} puntos");
                Console.WriteLine($"📊 Estadísticas:");
                Console.WriteLine($"   Nivel: {dificultad}");
                Console.WriteLine($"   Intentos usados: {intentos}/{maxIntentos}");
                Console.WriteLine($"   Número secreto: {numeroSecreto}");
            }

            Console.ResetColor();
            Console.WriteLine("\nGracias por jugar. ¡Hasta la próxima! 👋");
            Console.ReadKey();
        }
    }
}