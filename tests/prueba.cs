using System;

namespace JuegoAdivinaElNumero
{
    /// <summary>
    /// Clase principal del juego Adivina el Número
    /// Versión: 1.1.0 - Agregado sistema de intentos y mejoras UI
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            // Configuración inicial de la consola con emojis
            Console.Title = "🎲 Adivina el número 🎲";
            Console.ForegroundColor = ConsoleColor.Cyan;

            // Inicialización de variables del juego
            Random random = new Random();
            int numeroSecreto = random.Next(1, 101); // Genera número aleatorio entre 1-100
            int intentos = 0;
            int maxIntentos = 7; // NUEVO: Límite máximo de intentos permitidos
            bool adivinado = false;

            // NUEVO: Mensajes de bienvenida mejorados
            Console.WriteLine("¡Bienvenido al juego de Adivina el Número!");
            Console.WriteLine($"Tienes {maxIntentos} intentos para adivinar..."); // NUEVO: Aviso de límite
            Console.WriteLine("Estoy pensando en un número del 1 al 100...");
            Console.WriteLine("¿Podrás adivinarlo? 😎");
            Console.WriteLine("---------------------------------------------");

            // MODIFICADO: Agregada condición de maxIntentos al while
            while (!adivinado && intentos < maxIntentos)
            {
                // NUEVO: Muestra intentos restantes
                Console.Write($"\nIntento {intentos + 1}/{maxIntentos}. Tu número: ");
                string entrada = Console.ReadLine();
                int intento;

                // Validación de entrada numérica
                if (!int.TryParse(entrada, out intento))
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("⚠️ Por favor, escribe un número válido.");
                    Console.ResetColor();
                    continue;
                }

                intentos++;

                // MODIFICADO: Lógica de verificación con nuevos mensajes
                if (intento == numeroSecreto)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"\n🎉 ¡Felicidades! Adivinaste el número en {intentos} intentos.");
                    adivinado = true;
                }
                else if (intentos >= maxIntentos) // NUEVO: Verifica si se acabaron los intentos
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"\n😢 ¡Game Over! El número era {numeroSecreto}");
                }
                else if (intento < numeroSecreto)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Demasiado bajo. 📉 Intenta con un número más grande.");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Demasiado alto. 📈 Intenta con un número más pequeño.");
                }

                Console.ResetColor();
            }

            // NUEVO: Mensaje de despedida mejorado
            Console.WriteLine("\nGracias por jugar. ¡Hasta la próxima! 👋");
            Console.WriteLine("Presiona cualquier tecla para salir...");
            Console.ReadKey();
        }
    }
}