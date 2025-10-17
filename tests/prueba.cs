using System;

namespace JuegoAdivinaElNumero
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "🎲 Adivina el número 🎲";
            Console.ForegroundColor = ConsoleColor.Cyan;

            Random random = new Random();
            int numeroSecreto = random.Next(1, 101); // Número entre 1 y 100
            int intentos = 0;
            bool adivinado = false;

            Console.WriteLine("¡Bienvenido al juego de Adivina el Número!");
            Console.WriteLine("Estoy pensando en un número del 1 al 100...");
            Console.WriteLine("¿Podrás adivinarlo? 😎");
            Console.WriteLine("---------------------------------------------");

            while (!adivinado)
            {
                Console.Write("\nEscribe tu intento: ");
                string entrada = Console.ReadLine();
                int intento;

                // Validar que sea un número
                if (!int.TryParse(entrada, out intento))
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("⚠️ Por favor, escribe un número válido.");
                    Console.ResetColor();
                    continue;
                }

                intentos++;

                if (intento == numeroSecreto)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"\n🎉 ¡Felicidades! Adivinaste el número en {intentos} intentos.");
                    adivinado = true;
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

            Console.WriteLine("\nGracias por jugar. ¡Hasta la próxima! 👋");
            Console.ReadKey();
        }
    }
}
