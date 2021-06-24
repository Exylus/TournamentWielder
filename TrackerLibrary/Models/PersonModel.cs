using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackerLibrary.Models
{
    public class PersonModel
    {
        public int Id { get; set; }
        /// <summary>
        /// Represents the first name of user.
        /// </summary>
        public string FirstName { get; set; }
        /// <summary>
        /// Represents the last name of user.
        /// </summary>
        public string LastName { get; set; }
        /// <summary>
        /// Represents the email of user.
        /// </summary>
        public string EmailAddress { get; set; }
        /// <summary>
        /// Represents the user's phone number.
        /// </summary>
        public string CellphoneNumber { get; set; }

        public string FullName{ get
            {
                return $"{FirstName} {LastName}";

            }
        }
    }
}
