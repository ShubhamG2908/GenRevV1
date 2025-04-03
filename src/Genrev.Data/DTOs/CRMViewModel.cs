using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Genrev.Data.DTOs
{
    public class CRMViewModel
    {
        public int Id { get; set; }
        public int SalesPersonId { get; set; }
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(255, ErrorMessage = "Name cannot exceed 255 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(255, ErrorMessage = "Title cannot exceed 255 characters")]
        public string Title { get; set; }

        [StringLength(255, ErrorMessage = "Area of Responsibility cannot exceed 255 characters")]
        public string AreaOfResponsibility { get; set; }

        public string Strategy { get; set; }

        [DataType(DataType.Date, ErrorMessage = "Invalid date format")]
        public DateTime? StrategyDate { get; set; }

        [StringLength(500, ErrorMessage = "LinkedIn URL cannot exceed 500 characters")]
        public string LinkedInUrl { get; set; }
        public string Concern { get; set; }
        public List<CRMFileViewModel> UploadedFiles { get; set; } = new List<CRMFileViewModel>();
        public List<CRMRecordDTO> CRMListItems { get; set; }= new List<CRMRecordDTO>();
        public List<CRMContactViewModel> Contacts { get; set; } = new List<CRMContactViewModel>();
        public List<AddressViewModel> Addresses { get; set; } = new List<AddressViewModel>();

        // Derived properties for easier access
        public List<string> Phones
        {
            get => Contacts.Where(c => c.ContactType == "Phone").Select(c => c.ContactValue).ToList();
            set => Contacts.AddRange(value.Select(v => new CRMContactViewModel { ContactType = "Phone", ContactValue = v }));
        }

        public List<string> Emails
        {
            get => Contacts.Where(c => c.ContactType == "Email").Select(c => c.ContactValue).ToList();
            set => Contacts.AddRange(value.Select(v => new CRMContactViewModel { ContactType = "Email", ContactValue = v }));
        }

        public List<string> Notes
        {
            get => Contacts.Where(c => c.ContactType == "Notes").Select(c => c.ContactValue).ToList();
            set => Contacts.AddRange(value.Select(v => new CRMContactViewModel { ContactType = "Notes", ContactValue = v }));
        }
    }
    public class CRMContactViewModel
    {
        public int CRMId { get; set; }

        [Required(ErrorMessage = "Contact Type is required")]
        [RegularExpression("^(Phone|Email|Notes)$", ErrorMessage = "Invalid Contact Type")]
        public string ContactType { get; set; } // 'Phone', 'Email', 'Notes'

        [Required(ErrorMessage = "Contact Value is required")]
        [StringLength(500, ErrorMessage = "Contact Value cannot exceed 500 characters")]
        public string ContactValue { get; set; }
    }
    public class AddressViewModel
    {
        [StringLength(250, ErrorMessage = "Address Line 1 cannot exceed 250 characters")]
        public string AddressLine1 { get; set; }

        [StringLength(250, ErrorMessage = "Address Line 2 cannot exceed 250 characters")]
        public string AddressLine2 { get; set; }

        [StringLength(50, ErrorMessage = "City cannot exceed 50 characters")]
        public string City { get; set; }

        [StringLength(50, ErrorMessage = "State cannot exceed 50 characters")]
        public string State { get; set; }

        [StringLength(10, ErrorMessage = "Pincode cannot exceed 10 characters")]
        public string Pincode { get; set; }

        [StringLength(50, ErrorMessage = "Country cannot exceed 50 characters")]
        public string Country { get; set; }
    }
    public class CRMRecordDTO
    {
        public int Id { get; set; }
        public int SalesPersonId { get; set; }
        public int CustomerId { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string AreaOfResponsibility { get; set; }
        public string LinkedInUrl { get; set; }
        public string Strategy { get; set; }
        public DateTime? StrategyDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Notes { get; set; }
        public string FirstAddress { get; set; }
        public string SalesPersonName { get; set; }
        public string CustomerName { get; set; }
        public string Concern { get; set; }
        public string IndustryName { get; set; }
    }
    public class CRMFileViewModel
    {
        public int Id { get; set; }
        public int CRMId { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public DateTime CreatedAt { get; set; }
    }

}
