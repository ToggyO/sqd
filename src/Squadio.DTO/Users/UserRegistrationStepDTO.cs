namespace Squadio.DTO.Users
{
    public class UserRegistrationStepDTO
    {
        public string StepName { get; set; }
        public int Step { get; set; }
    }
    public class UserRegistrationStepDTO<T>
    {
        public string StepName { get; set; }
        public int Step { get; set; }
        public T Data { get; set; }
    }
}