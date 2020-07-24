using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Otc.DomainBase.Exceptions;
using Otc.Messaging.Abstractions;
using Otc.Messaging.Subscriber.HW;

namespace SubscriberHostedWorker.Template
{
    public class MessageHandler : IMessageHandler<Pesquisa>
    {
        private readonly IFilmesService filmesService;
        private readonly ILogger logger;

        public MessageHandler(IFilmesService filmesService,
            ILoggerFactory loggerFactory)
        {
            this.filmesService = filmesService ??
                throw new ArgumentNullException(nameof(filmesService));

            logger = loggerFactory?.CreateLogger<MessageHandler>() ??
                throw new ArgumentNullException(nameof(loggerFactory));
        }

        public async Task HandleAsync(Pesquisa message,
            IMessageContext messageContext)
        {
            logger.LogInformation($"Mensagem {messageContext.Id} " +
                $"recebida via queue {messageContext.Queue}.");

            try
            {
                await filmesService.ObterFilmesAsync(message);
            }
            catch (CoreException ex)
            {
                // Quando uma exception é propagada para fora deste método, o
                // mecanismo de fila entende que a mensagem NÃO foi processada
                // e daí coloca esta mensagem para retentativa (ou conforme o
                // caso move a mensagem para uma fila morta).
                // 
                // No entanto, as exceptions de negócio (CoreException), em
                // geral, não serão resolvidas apenas com retentativas pois
                // geralmente precisam ser corrigidas em outros sistemas ou
                // processos, portanto, o ideal é capturar essas exceptions
                // e não deixar que subam para gerar novas tentativas.
                // 
                // Capturando estas exceptions de negócio aqui, o mecanismo
                // de fila irá entender que a mensagem foi processada e irá
                // retirar esta mensagem da fila.
                // 
                // Contudo, para não "perder" esta mensagem, visto que contém
                // erros de negócio, a aplicação deverá tratá-la em algum outro
                // fluxo conforme a necessidade, por exemplo, armazenando-a em
                // banco de dados para então ser analisada e corrigida no
                // contexto do negócio.

                logger.LogWarning(ex, "Falha ao pesquisar filmes para os " +
                    "critérios informados: {@Pesquisa}.", message);

                // Chamar aqui o servico da aplicacao que irá tratar mensagens
                // com erro de negócio.
            }
        }
    }

    /// <summary>
    /// 
    /// TODO: Esta classe deve ser removida.
    /// 
    /// Está aqui apenas representando o objeto do domínio que será
    /// recebido da fila.
    /// 
    /// </summary>
    public class Pesquisa
    {
    }

    /// <summary>
    /// 
    /// TODO: Esta classe deve ser removida.
    /// 
    /// Está aqui apenas representando um serviço do domínio que irá
    /// processar o objeto recebido da fila.
    /// 
    /// </summary>
    public interface IFilmesService
    {
        Task ObterFilmesAsync(Pesquisa pesquisa);
    }
}