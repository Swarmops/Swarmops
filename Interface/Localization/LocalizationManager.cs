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
							return "Välkommen till PirateWeb!";
						case "es":
							return "Bienvenido en PirateWeb!";
						case "fr":
							return "Bienvenue à PirateWeb!";
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
							return "PirateWeb är ett system för att hantera medlemmar för piratpartierna runt om i världen. För att logga in, så skriv ditt namn, medlemsnummer eller e-mail, tillsammans med ditt lösenord.";
						case "es":
							return "PirateWeb es un sistema de gerencia del miembro para los Partidos Piratos alrededor del mundo. A la conexión, escribir por favor su nombre, número del miembro, o dirección d'email, junto con su contraseña.";
						case "fr":
							return "PirateWeb est un système de gestion de membre pour les Partis Pirates autour du monde. À l'ouverture, écrire svp votre nom, nombre de membre, ou adresse d'email, avec votre mot de passe.";
						case "de":
							return "PirateWeb ist ein internationales Mitgliedermanagementsystem für Piraten. Zum Login geben Sie bitte ihren Namen, Mitgliedsnummer oder e-mail Adresse, zusammen mit ihrem Kennwort ein.";
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
							return "Jag har glömt mitt lösenord...";
					}
					break;
			}

            return defaultValue;
        }
    }
}
