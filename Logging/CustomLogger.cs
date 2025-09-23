namespace APICatalogo.Logging;

public class CustomLogger : ILogger
{
    readonly string loggerName;

    private readonly CustomLoggerProviderConfiguration loggerConfig;

    public CustomLogger(string name, CustomLoggerProviderConfiguration _loggerConfig)
    {
        loggerName = name;
        loggerConfig = _loggerConfig;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return logLevel == loggerConfig.LogLevel;
    }
    
    // Nao estamos utilizando essa funcao
    public IDisposable BeginScope<TState>(TState state)
    {
        return null;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        string msg = $"{logLevel.ToString()}: {eventId.Id} - {formatter(state, exception)}";

        EscreverTextoNoArquivo(msg);
    }

    private void EscreverTextoNoArquivo(string msg)
    {
        string caminhoArquivoLog = @"C:\Users\Everton-SSD\Desktop\Logs.txt";

        using (StreamWriter streamWriter = new StreamWriter(caminhoArquivoLog, true))
        {
            try
            {
                streamWriter.WriteLine(msg);
                streamWriter.Close();
            }
            catch (Exception)
            {
                throw;
            }
        } 
    }
}