namespace FluentImposter.Core.Entities
{
    public class Response
    {
        private const int DEFAULT_STATUS_CODE_OK = 200;

        internal Response()
        {
            
        }

        public string Content { get; internal set; } = string.Empty;
        public int StatusCode { get; internal set; } = DEFAULT_STATUS_CODE_OK;
    }
}
