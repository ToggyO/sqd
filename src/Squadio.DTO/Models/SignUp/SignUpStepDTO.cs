namespace Squadio.DTO.SignUp
{
    public class SignUpStepDTO
    {
        public UserRegistrationStepDTO RegistrationStep { get; set; }
    }
    public class SignUpStepDTO<T>
    {
        public UserRegistrationStepDTO RegistrationStep { get; set; }
        public T Data { get; set; }
    }
}