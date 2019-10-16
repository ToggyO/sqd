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
        /// User entered name
        /// </summary>
        UsernameEntered = 4,
        /// <summary>
        /// User accepted agreement with terms and privacy policy 
        /// </summary>
        Agreement = 5,
        /// <summary>
        /// User created company
        /// </summary>
        CompanyCreated = 6,
        /// <summary>
        /// User created team
        /// </summary>
        TeamCreated = 7,
        /// <summary>
        /// User created project
        /// </summary>
        ProjectCreated = 8,
        Done = 9
    }
}