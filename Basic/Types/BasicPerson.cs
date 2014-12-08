using System;
using Swarmops.Basic.Enums;
using Swarmops.Basic.Interfaces;

namespace Swarmops.Basic.Types
{
    [Serializable]
    public class BasicPerson : IEmailPerson, IHasIdentity
    {
        public BasicPerson (string name, string email)
        {
            Name = name;
            Email = email;
        }

        public BasicPerson (int personId, string passwordHash, string name, string email, string street,
            string postalCode, string cityName, int countryId, string phone, int geographyId,
            DateTime birthdate, PersonGender gender)
        {
            PersonId = personId;
            PasswordHash = passwordHash;
            Name = name;
            Email = email;
            Street = street;
            PostalCode = postalCode;
            CityName = cityName;
            CountryId = countryId;
            Phone = phone;
            GeographyId = geographyId;
            Birthdate = birthdate;
            Gender = gender;
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
        public string Street { get; protected set; }
        public string PostalCode { get; protected set; }
        public string CityName { get; protected set; }
        public string Phone { get; protected set; }
        public DateTime Birthdate { get; protected set; }
        public string PasswordHash { get; protected set; }
        public int GeographyId { get; protected set; }
        public int CountryId { get; protected set; }
        public PersonGender Gender { get; protected set; }
        public string Name { get; protected set; }
        public string Email { get; protected set; }


        public int Identity
        {
            get { return PersonId; }
        }
    }
}