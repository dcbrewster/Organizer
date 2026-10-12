using System;
using System.Collections.Generic;
using System.Text;

namespace Organizer.Models
{
    /// <summary>
    /// Represents a contact in the organizer application, including details such as name, company, email,
    /// phone number, address, and notes.
    /// </summary>
    public sealed class Contact
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Company { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
    };
}