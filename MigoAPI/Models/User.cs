using System.ComponentModel.DataAnnotations;

namespace MigoAPI.Models
{
    /// <summary>
    /// An Author with Id, userName, password, firstName, lastName and Age fields
    /// </summary>
    public class User
    {
        /// <summary>
        /// The User's id
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// The User's user name
        /// </summary>
        [Required]
        public string UserName { get; set; }

        /// <summary>
        /// The User's password
        /// </summary>
        [Required]
        public string Password { get; set; }

        /// <summary>
        /// The User's first name
        /// </summary>
        [Required]
        public string FirstName { get; set; }

        /// <summary>
        /// The User's last name
        /// </summary>
        [Required]
        public string LastName { get; set; }

        /// <summary>
        /// The User's Age
        /// </summary>
        public int Age { get; set; }
    }
}
