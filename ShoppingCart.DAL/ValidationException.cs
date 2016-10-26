namespace ShoppingCart.DAL
{/// <summary>
/// User friendly-exception. An error message may be displayed to the user.
/// </summary>
    public class ValidationException : RepositoryException
    {
        public ValidationException() { }

        public ValidationException(string message) : base(message) { }
    }
}
