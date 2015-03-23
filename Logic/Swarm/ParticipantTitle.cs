using System;
using Swarmops.Common.Enums;
using Swarmops.Logic.Resources;

namespace Swarmops.Logic.Swarm
{
    public class Participant
    {
        public static string Localized (ParticipantTitle title, TitleVariant variant = TitleVariant.Generic)
        {
            return Logic_Swarm_ParticipantTitle.ResourceManager.GetString ("Title_" + title + "_" + variant);
        }

        public static string Localized (ParticipantTitle title, PersonGender gender)
        {
            string genderString = gender.ToString();
            if (gender == PersonGender.Unknown)
            {
                genderString = "Generic";
            }
            return Localized (title, genderString);
        }

        public static string Localized (ParticipantTitle title, string variantString)
        {
            return Localized (title, (TitleVariant) (Enum.Parse (typeof (TitleVariant), variantString)));
        }
    }

    public enum TitleVariant
    {
        Unknown = 0,
        Corps,
        Generic,
        Plural,
        Male,
        Female,
        Ship
    }

    public enum ParticipantTitle
    {
        Unknown = 0,
        Activist,
        Agent,
        Ambassador,
        Citizen,
        Conscript,
        Contributor,
        Customer,
        Employee,
        Member,
        Operative,
        Person,
        Regular,
        Resident,
        Sailor,
        Salesperson
    };
}