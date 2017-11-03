namespace AcumaticaESign
{
    public abstract class BaseApiService
    {
        public RequestDependencies Dependencies { get; set; }

        protected BaseApiService(RequestDependencies dependencies)
        {
            Dependencies = dependencies;
            InitRestClient();
        }

        protected abstract void InitRestClient();
    }
}
