using System;
using System.Threading.Tasks;
using EventHandlerA;

namespace ConsumingProjectA
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            Console.WriteLine("Monolith event hub example.");
            Console.WriteLine("\"normal\" - Run normal demo.");
            Console.WriteLine("\"nosub\" - Run demo of hub throwing exceptions on no event subscribers.");
            Console.WriteLine("\"throws\" - Run demo of hub throwing exceptions for handle result failures.");
            Console.Write("> ");
            var choice = Console.ReadLine();

            switch (choice)
            {
                case "normal":
                    await RunNormalDemo();
                    break;
                case "nosub":
                    await RunNoSubDemo();
                    break;
                case "throws":
                    await RunThrowsDemo();
                    break;
            }
        }

        private static async Task RunThrowsDemo()
        {
            var builder = new EventHubBuilder();
            builder.ThrowsOnResultFailure(evt =>
                new BusinessException($"Business logic exception, event is \"{evt}\""));
            builder.AddSubscriber(new FailingEventHandler());
            var hub = builder.CreateHub();

            await hub.Dispatch("Eh?");
        }

        private static async Task RunNoSubDemo()
        {
            var builder = new EventHubBuilder();
            builder.ThrowsOnNoSubscriber();
            var hub = builder.CreateHub();

            await hub.Dispatch(42);
        }

        private static async Task RunNormalDemo()
        {
            var builder = new EventHubBuilder();
            builder.AddSubscriber(new PrintingEventHandler<int>(Console.Out));
            builder.AddSubscriber(new PrintingEventHandler<string>(Console.Out));
            builder.AddSubscriber(new PrintingEventHandler<string>(Console.Out, "Pretend I'm sending an email."));
            builder.AddSubscriber(
                new PrintingEventHandler<string>(Console.Out, "Pretend I'm sending a Discord message."));
            var hub = builder.CreateHub();

            await hub.Dispatch(42);
            await hub.Dispatch("Hello world.");
            await hub.Dispatch("Hello Kai!");
            await hub.Dispatch(84L);
        }
    }
}