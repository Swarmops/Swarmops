using System;
using System.Collections.Generic;
using System.Text;
using Activizr.Basic.Enums;
using Activizr.Basic.Interfaces;


namespace Activizr.Basic.Types
{
 

    [Serializable]
    public class BasicPerson : IEmailPerson, IHasIdentity
    {
        public BasicPerson (string name, string email)
        {
            this.Name = name;
            this.Email = email;
        }

        public BasicPerson (int personId, string passwordHash, string name, string email, string street,
                            string postalCode, string cityName, int countryId, string phone, int geographyId,
                            DateTime birthdate, PersonGender gender)
        {
            this.PersonId = personId;
            this.PasswordHash = passwordHash;
            this.Name = name;
            this.Email = email;
            this.Street = street;
            this.PostalCode = postalCode;
            this.CityName = cityName;
            this.CountryId = countryId;
            this.Phone = phone;
            this.GeographyId = geographyId;
            this.Birthdate = birthdate;
            this.Gender = gender;
        }

        public BasicPerson (BasicPerson original)
            : this (original.PersonId, original.PasswordHash, original.Name, original.Email, original.Street, 
                    original.PostalCode, original.CityName, original.CountryId, original.Phone, 
                    original.GeographyId, original.Birthdate, original.Gender)
        {
        }


        public bool IsMale
        {
            get { return Gender == PersonGender.Male; }
        }

        public bool IsFemale
        {
            get { return Gender == PersonGender.Female; }
        }


        public int PersonId { get; private set; }
        public string Name { get; protected set; }
        public string Email { get; protected set; }
        public string Street { get; protected set; }
        public string PostalCode { get; protected set; }
        public string CityName { get; protected set; }
        public string Phone { get; protected set; }
        public DateTime Birthdate { get; protected set; }
        public string PasswordHash { get; protected set; }
        public int GeographyId { get; protected set; }
        public int CountryId { get; protected set; }
        public PersonGender Gender { get; protected set; }


        public int Identity
        {
            get { return PersonId; }
        }



    }
}