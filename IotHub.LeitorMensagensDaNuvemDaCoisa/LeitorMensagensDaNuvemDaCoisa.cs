using System;
using Microsoft.Azure.EventHubs;
using System.Threading.Tasks;
using System.Threading;
using System.Text;
using System.Collections.Generic;

namespace read_d2c_messages
{
    class LeitorMensagensDaNuvemDaCoisa
    {
        //Configurações para conexão
        private static readonly string _eventHubsEndpointCompativel = "sb://ihsuprodblres060dednamespace.servicebus.windows.net/";
        private static readonly string _eventHubsCaminhoCompativel = "iothub-ehub-iothubarqu-941370-cd95239ad6";
        private static readonly string _iotHubChaveSas = "oYxCZz/AIq0sTxXk4Wnl9Aubo5C6sudgkjKKNzZyVw8=";
        private static readonly string _iotHubNomeChaveSasKey = "iothubowner";

        private static EventHubClient _leitorHubClient;

        private static async Task ReceberMensagensDaCoisaAsync(string particao, CancellationToken ct)
        {
            var eventHubReceiver = _leitorHubClient.CreateReceiver("$Default", particao, EventPosition.FromEnqueuedTime(DateTime.Now));
            Console.WriteLine("Criar leitor de mensagens na partição: " + particao);
            while (true)
            {
                if (ct.IsCancellationRequested) break;
                Console.WriteLine("Escutando por mensagens em: " + particao);
                var eventos = await eventHubReceiver.ReceiveAsync(100);

                // Se tiver dados no batch, então processe
                if (eventos == null) continue;

                foreach(EventData dadosEvento in eventos)
                { 
                  string data = Encoding.UTF8.GetString(dadosEvento.Body.Array);
                  Console.WriteLine("Mensagem recebida na partição {0}:", particao);
                  Console.WriteLine("  {0}:", data);
                  Console.WriteLine("Propriedades da aplicação (setado pela coisa):");
                  foreach (var prop in dadosEvento.Properties)
                  {
                    Console.WriteLine("  {0}: {1}", prop.Key, prop.Value);
                  }
                  Console.WriteLine("Propriedades do sistema (setado pelo IoT Hub):");
                  foreach (var prop in dadosEvento.SystemProperties)
                  {
                    Console.WriteLine("  {0}: {1}", prop.Key, prop.Value);
                  }
                }
            }
        }

        private static async Task Main(string[] args)
        {
            Console.WriteLine("Lendo mensagens da coisa. Ctrl-C para sair.\n");

            var connectionString = new EventHubsConnectionStringBuilder(new Uri(_eventHubsEndpointCompativel), _eventHubsCaminhoCompativel, _iotHubNomeChaveSasKey, _iotHubChaveSas);
            _leitorHubClient = EventHubClient.CreateFromConnectionString(connectionString.ToString());

            var runtimeInfo = await _leitorHubClient.GetRuntimeInformationAsync();
            var d2cPartitions = runtimeInfo.PartitionIds;

            CancellationTokenSource cts = new CancellationTokenSource();

            Console.CancelKeyPress += (s, e) =>
            {
                e.Cancel = true;
                cts.Cancel();
                Console.WriteLine("Exiting...");
            };

            var tasks = new List<Task>();
            foreach (string partition in d2cPartitions)
            {
                tasks.Add(ReceberMensagensDaCoisaAsync(partition, cts.Token));
            }

            Task.WaitAll(tasks.ToArray());
        }
    }
}
