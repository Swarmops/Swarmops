using System;
using System.Collections.Generic;
using System.Text;

namespace Swarmops.Interface.Localization
{
    public class LocalizationManager
    {
        public LocalizationManager()
        {
        }

		static public string GetLocalString (string stringName, string defaultValue)
		{
			// Determine current culture

			return GetLocalString(System.Threading.Thread.CurrentThread.CurrentCulture.Name, stringName, defaultValue);
		}

        static public string GetLocalString (string locale, string stringName, string defaultValue)
        {
			locale = locale.Substring (0, 2).ToLower();

			switch (stringName)
			{
				case "Interface.Security.Login.WelcomeHeader":
					switch (locale)
					{
						case "sv":
							return "V�lkommen till PirateWeb!";
						case "es":
							return "Bienvenido en PirateWeb!";
						case "fr":
							return "Bienvenue � PirateWeb!";
						case "de":
							return "Willkommen zu PirateWeb!";
						case "ru":
							return "????? ?????????? ?? PirateWeb!";
						case "ja":
							return "PirateWeb????!";
					}
					break;

				case "Interface.Security.Login.WelcomeParagraph":
					switch (locale)
					{
						case "sv":
							return "PirateWeb �r ett system f�r att hantera medlemmar f�r piratpartierna runt om i v�rlden. F�r att logga in, s� skriv ditt namn, medlemsnummer eller e-mail, tillsammans med ditt l�senord.";
						case "es":
							return "PirateWeb es un sistema de gerencia del miembro para los Partidos Piratos alrededor del mundo. A la conexi�n, escribir por favor su nombre, n�mero del miembro, o direcci�n d'email, junto con su contrase�a.";
						case "fr":
							return "PirateWeb est un syst�me de gestion de membre pour les Partis Pirates autour du monde. � l'ouverture, �crire svp votre nom, nombre de membre, ou adresse d'email, avec votre mot de passe.";
						case "de":
							return "PirateWeb ist ein internationales Mitgliedermanagementsystem f�r Piraten. Zum Login geben Sie bitte ihren Namen, Mitgliedsnummer oder e-mail Adresse, zusammen mit ihrem Kennwort ein.";
						case "ru":
							return "PirateWeb ???????? ?????? ??????? ??????????? ????????? ?????? ?? ???? ????. ????? ??????????, ??????????, ??????? ???? ???, ?????????? ??????, ??? ????? ??????????? ?????, ? ????? ??????.";
						case "ja":
							return "PirateWeb??????????????????????????? ????????????????????????????????????????E???????????????";
					}
					break;

                case "Interface.Security.Login.NewPasswordLink":
					switch (locale)
					{
						case "sv":
							return "Jag har gl�mt mitt l�senord...";
					}
					break;
			}

            return defaultValue;
        }
    }
}
