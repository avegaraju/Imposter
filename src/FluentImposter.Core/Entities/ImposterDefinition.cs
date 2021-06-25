namespace FluentImposter.Core.Entities
{
    public class ImposterDefinition
    {
        private readonly string _imposterName;

        public ImposterDefinition(string imposterName)
        {
            _imposterName = imposterName;
        }

        public RestResource ForRest()
        {
            return new RestResource(_imposterName);
        }

        public SmtpServer ForSmtp()
        {
            return new SmtpServer(_imposterName);
        }
    }
}
