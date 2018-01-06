namespace FluentImposter.Core.Entities
{
    public class Response
    {
        private const int DEFAULT_STATUS_CODE_OK = 200;

        internal Response()
        {
            
        }

        public string Content { get; set; } = string.Empty;
        public int StatusCode { get; set; } = DEFAULT_STATUS_CODE_OK;
    }
}
