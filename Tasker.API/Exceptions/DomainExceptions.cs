namespace Tasker.API.Exceptions
{
    // Base domain exception (optional)
    public abstract class DomainException : Exception
    {
        protected DomainException(string message) : base(message) { }
    }

    // Thrown when a requested entity is not found
    public class EntityNotFoundException : DomainException
    {
        public string EntityName { get; set; }
        public EntityNotFoundException(string entityName, string key)
            : base($"{entityName} with id '{key}' was not found.")
        { EntityName = entityName; }
    }

    // Thrown when a duplicate entity creation is attempted
    public class DuplicateEntityException : DomainException
    {
        public string EntityName { get; set; }

        public DuplicateEntityException(string name, string message) : base(message)
        {
            EntityName = name;
        }
    }

    // Thrown when an operation is not valid in the domain context
    public class InvalidDomainOperationException : DomainException
    {
        public InvalidDomainOperationException(string message) : base(message) { }
    }
}
