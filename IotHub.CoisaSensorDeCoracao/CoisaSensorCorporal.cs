using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;

namespace CoisaSensorDeCoracao
{
    class CoisaSensorCoracao
    {
        private static DeviceClient _coisaSensorCoracao;

        // A "Coisa" deve estar autenticada para acessar o IoT Hub. Abaixo a credencial para acesso ao Azure.
        private static readonly string _connectionString = "HostName=IotHubArquiteturaNuvem.azure-devices.net;DeviceId=CoisaSensorDeCoracao;SharedAccessKey=2+Gg71pC7OXGw8ie+P7PNSgUTAcL3c0vBfc40IKZzh0=";

        // Método asincrono para enviar telemetria de coração simulada
        private static async void EnviarMensagemTelemetriaParaCloudAsync()
        {
            // Valores máximos para o batimento
            int batimentoMinimo = 20;
            int batimentoMaximo = 200;

            Random rand = new Random();

            while (true)
            {
                double batimentoAtual = rand.Next(batimentoMinimo, batimentoMaximo);

                // Criar mensagem Json para envio
                var dadosTelemetria = new
                {
                    batimentoCardiaco = batimentoAtual,
                    alertaBatimentoCardiacoAbaixoDoNormal = batimentoAtual < 50,
                    alertaBatimentoCardiacoAcimaDoNormal = batimentoAtual > 150
                };

                var mensagemString = JsonConvert.SerializeObject(dadosTelemetria);
                var mensagem = new Message(Encoding.ASCII.GetBytes(mensagemString));

                // Envia uma mensagem de telemetria
                await _coisaSensorCoracao.SendEventAsync(mensagem);
                Console.WriteLine("{0} > Enviando telemetria: {1}", DateTime.Now, mensagemString);

                await Task.Delay(10000);
            }
        }
        private static void Main(string[] args)
        {
            Console.WriteLine("IoT Hub - \"Coisa Simulada enviando telemetria do coração\". Aperte Ctrl-C para sair.\n");

            // Conectar ao IoT Hub utilizando o protocolo MQTT
            _coisaSensorCoracao = DeviceClient.CreateFromConnectionString(_connectionString, TransportType.Mqtt);
            EnviarMensagemTelemetriaParaCloudAsync();
            Console.ReadLine();
        }
    }
}
