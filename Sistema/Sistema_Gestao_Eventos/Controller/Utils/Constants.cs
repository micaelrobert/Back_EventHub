namespace GestaoEventos.API.Utils
{
  
    public static class Constants
    {
        public static class Messages
        {
            public const string EVENTO_NAO_ENCONTRADO = "Evento não encontrado";
            public const string INGRESSO_NAO_ENCONTRADO = "Ingresso não encontrado";
            public const string EVENTO_ESGOTADO = "Evento esgotado";
            public const string EVENTO_JA_REALIZADO = "Evento já foi realizado";
            public const string INGRESSO_JA_DEVOLVIDO = "Ingresso já foi devolvido";
            public const string SUCESSO_CRIAR_EVENTO = "Evento criado com sucesso";
            public const string SUCESSO_ATUALIZAR_EVENTO = "Evento atualizado com sucesso";
            public const string SUCESSO_DELETAR_EVENTO = "Evento deletado com sucesso";
            public const string SUCESSO_COMPRAR_INGRESSO = "Ingresso comprado com sucesso";
            public const string SUCESSO_DEVOLVER_INGRESSO = "Ingresso devolvido com sucesso";
            public const string ERRO_INTERNO = "Erro interno do servidor";
            public const string DADOS_INVALIDOS = "Dados inválidos";
        }

        public static class Validation
        {
            public const int NOME_MAX_LENGTH = 200;
            public const int DESCRICAO_MAX_LENGTH = 1000;
            public const int LOCAL_MAX_LENGTH = 300;
            public const int EMAIL_MAX_LENGTH = 150;
            public const int TELEFONE_MAX_LENGTH = 20;
            public const int MOTIVO_DEVOLUCAO_MAX_LENGTH = 500;

            public const decimal PRECO_MINIMO = 0.01m;
            public const int CAPACIDADE_MINIMA = 1;
        }

        public static class Cors
        {
            public const string POLICY_NAME = "AllowAngularApp";
            public static readonly string[] DEFAULT_ORIGINS = { "http://localhost:4200", "https://localhost:4200" };
        }

        public static class Database
        {
            public const string IN_MEMORY_NAME = "EventosDB";
        }
    }
}