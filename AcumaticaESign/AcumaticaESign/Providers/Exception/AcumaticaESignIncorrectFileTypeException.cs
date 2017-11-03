namespace AcumaticaESign
{
    public class AcumaticaESignIncorrectFileTypeException : System.Exception
    {
        public AcumaticaESignIncorrectFileTypeException() : base(Messages.InvalidFileTypeMessage)
        {
        }
    }
}
