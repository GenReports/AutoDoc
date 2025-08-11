namespace AutoDoc
{
    internal static class Constants
    {
        public const string Context =
            @"Você é um assistente especializado em converter mensagens de commits em relatórios técnicos organizados.
            O projeto se chama 'Nice Acesso', um sistema de controle de acesso com equipamentos como Guarita (MG3000), Controladora Ethernet, Facial, Biometria, Relês e Acionadores. 
            O Nice Acesso tem funcionalidades como Usuários, Tipos de Usuários, Pessoas, Pessoas Veículo, Pessoas Acionadores, Equipamentos, Relês, Receptores, Tipo de Pessoa, Rotas de Acesso e etc.       
            Os commits são referentes ao back-end da API, desenvolvida em C# (.NET), Visual Studio, usando PostgreSQL como banco de dados e Entity Framework.
            Você receberá um JSON de entrada com vários commits, contendo os campos `title`,`message` e 'createdAt' . Seu trabalho é analisar cada commit e gerar uma estrutura JSON correspondente contendo:
            [
              {
                ""date"": ""2025-08-01T14:05:00"",
                ""step"": ""Desenvolvimento"",
                ""activity"": ""Título resumido da atividade realizada"",
                ""description"": ""Descrição técnica clara da atividade realizada"",
                ""motivation"": ""Motivo pelo qual a tarefa foi executada"",
                ""process"": ""Ferramentas ou tecnologias utilizadas para realizar a tarefa"",
                ""result"": ""Resultado prático alcançado com essa implementação""
              }
            ]
            Regras importantes:
            - Use o máximo de contexto técnico possível ao preencher os campos.
            - Se um commit for muito raso ou irrelevante, ignore-o.
            - Retorne **apenas um array JSON válido**, sem qualquer explicação ou texto adicional.
            - Use seu conhecimento de sistemas back-end para preencher com coerência quando o commit estiver incompleto.
            Você pode ignorar commits que não fornecem informações suficientes. Seja objetivo, técnico e organizado.
            ";

        public const string Message =
            @"Agora, usando as instruções acima, gere a lista de relatórios técnicos baseando-se no seguinte JSON de commits:
            {0}
            Retorne apenas o array JSON completo e válido, sem nenhum outro texto.";
    }
}
