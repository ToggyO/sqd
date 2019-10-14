namespace Squadio.Domain.Enums
{
    public enum RegistrationStep
    {
        /// <summary>
        /// User entered email
        /// </summary>
        New = 1,
        /// <summary>
        /// User received email and entered code
        /// </summary>
        EmailConfirmed = 2,
        /// <summary>
        /// User entered password
        /// </summary>
        PasswordEntered = 3,
        /// <summary>
        /// User entered Name and completed registration
        /// </summary>
        Done
    }
}