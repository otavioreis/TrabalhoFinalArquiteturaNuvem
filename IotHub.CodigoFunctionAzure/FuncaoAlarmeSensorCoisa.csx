#r "Newtonsoft.Json"

using System;
using Newtonsoft.Json;

public class BatimentoObj{
    public double batimentoCardiaco { get; set; } 
    public bool alertaBatimentoCardiacoAbaixoDoNormal { get; set; }
    public bool alertaBatimentoCardiacoAcimaDoNormal { get; set; }
}

public static void Run(string myEventHubMessage, ILogger log)
{
    log.LogInformation($"C# IoT Hub trigger function processou uma mensagem:");

    if(myEventHubMessage != "Test Message")
    {
        var batimentoCardiacoObj = JsonConvert.DeserializeObject<BatimentoObj>(myEventHubMessage);

        var mensagemRecebidaFormatada = JsonConvert.SerializeObject(batimentoCardiacoObj, Formatting.Indented);
        log.LogInformation(mensagemRecebidaFormatada);

        if(batimentoCardiacoObj.alertaBatimentoCardiacoAbaixoDoNormal || 
        batimentoCardiacoObj.alertaBatimentoCardiacoAcimaDoNormal)
        {
            //Aqui poderia ser disparado o e-mail para o usuário alertando do problema no paciente

            log.LogInformation($"Foi detectada uma anomalia no coração do paciente!!!");

            var mensagemCoracao = batimentoCardiacoObj.alertaBatimentoCardiacoAbaixoDoNormal ? 
                        "O coração do paciente está batendo abaixo do normal." :
                        "O coração do paciente está batendo acima do normal.";


            var mensagemBatimento = batimentoCardiacoObj.alertaBatimentoCardiacoAbaixoDoNormal ? 
                        $"O menor batimento deveria ser 50 porém está em {batimentoCardiacoObj.batimentoCardiaco}" :
                        $"O maior batimento deveria ser 150 porém está em {batimentoCardiacoObj.batimentoCardiaco}";

            log.LogInformation(mensagemCoracao); 
            log.LogInformation(mensagemBatimento); 

            log.LogInformation("==============Fim Processamento Alerta==============")       ;
        }
    }
        
    
    
}
