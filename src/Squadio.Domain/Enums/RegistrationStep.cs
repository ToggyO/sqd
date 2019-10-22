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
        /// User entered name
        /// </summary>
        UsernameEntered = 3,
        /// <summary>
        /// User accepted agreement with terms and privacy policy 
        /// </summary>
        Agreement = 4,
        /// <summary>
        /// User created company
        /// </summary>
        CompanyCreated = 5,
        /// <summary>
        /// User created team
        /// </summary>
        TeamCreated = 6,
        /// <summary>
        /// User created project
        /// </summary>
        ProjectCreated = 7,
        Done = 8
    }
}