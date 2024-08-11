namespace SQS
{
    public class CustomerCreated
    {
        public required Guid ID { get; init; }
        public required string FullName { get; init; }
        public required int  Age { get; init; }
        public required string UserName { get; init; }
        public required DateTime DOB { get; init; }
    }
}
