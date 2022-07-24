
using RabbitMQ.Client;
using System.Text;

class Program
{
    private static readonly List<string> cars = new List<string> { "BMW", "Ford", "Volvo", "Lada" };
    private static readonly List<string> colors = new List<string> { "red", "black", "blue", "green" };
    private static readonly Random random = new Random();

    static void Main(string[] args)
    {
        var counter = 0;

        do
        {
            int timeToSleep = random.Next(1000, 2000);
            Thread.Sleep(timeToSleep);

            var factory = new ConnectionFactory() { HostName = "localhost" };

            var connection = factory.CreateConnection();

            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare("topic_logs", ExchangeType.Topic);

                string routingKey = counter % 4 == 0 
                    ?"Lada.black.fast.sport" : counter % 5 == 0
                    ? "Volvo.exclusive.expasive.sport"
                    :GenerateRoutingKey();

                string message = $"здороува мужики, сообщение[{counter}] типа:[{routingKey}]";
                var body = Encoding.UTF8.GetBytes(message);
                channel.BasicPublish(
                    exchange: "topic_logs",
                    routingKey: routingKey,
                    basicProperties: null,
                    body: body);

                Console.WriteLine($"Сообщение [{counter++}] типа:{routingKey} - отправлено в Topic Exchange");
            }
        }
        while (true);
    }

    private static string GenerateRoutingKey()
    {
        return $"{cars[random.Next(0, 3)]}.{colors[random.Next(0, 3)]}";
    }
}
