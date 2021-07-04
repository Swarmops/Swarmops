﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Localization {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Pages_Security {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Pages_Security() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Localization.Pages.Security", typeof(Pages_Security).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Access to this page was denied.
        /// </summary>
        internal static string AccessDenied_Header {
            get {
                return ResourceManager.GetString("AccessDenied_Header", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to You tried to access a page you don&apos;t have permissions for. See the main panel for some reasons this may have happened..
        /// </summary>
        internal static string AccessDenied_Info {
            get {
                return ResourceManager.GetString("AccessDenied_Info", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Access Denied.
        /// </summary>
        internal static string AccessDenied_PageTitle {
            get {
                return ResourceManager.GetString("AccessDenied_PageTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;p&gt;The Swarmops security framework has denied you access to the page you were trying to go to. This usually means that its functions were above the access levels granted to you for the currently selected organization (&lt;em&gt;{0}&lt;/em&gt;). This has not been logged nor reported anywhere.&lt;/p&gt;&lt;p&gt;There are several reasons why this may have happened:&lt;/p&gt;.
        /// </summary>
        internal static string AccessDenied_Rant {
            get {
                return ResourceManager.GetString("AccessDenied_Rant", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Current organization.
        /// </summary>
        internal static string ChangeOrganizations_CurrentOrganization {
            get {
                return ResourceManager.GetString("ChangeOrganizations_CurrentOrganization", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to You can be taking part in several organizations on this Swarmops installation. Here is where you switch between them..
        /// </summary>
        internal static string ChangeOrganizations_Info {
            get {
                return ResourceManager.GetString("ChangeOrganizations_Info", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to New organization.
        /// </summary>
        internal static string ChangeOrganizations_NewOrganization {
            get {
                return ResourceManager.GetString("ChangeOrganizations_NewOrganization", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to You&apos;re not participating in any other organization on this Swarmops installation, so you can&apos;t switch to something else. Therefore, we&apos;re going back to Dashboard instead..
        /// </summary>
        internal static string ChangeOrganizations_NothingToChange {
            get {
                return ResourceManager.GetString("ChangeOrganizations_NothingToChange", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Change Organizations.
        /// </summary>
        internal static string ChangeOrganizations_PageTitle {
            get {
                return ResourceManager.GetString("ChangeOrganizations_PageTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Switch.
        /// </summary>
        internal static string ChangeOrganizations_Switch {
            get {
                return ResourceManager.GetString("ChangeOrganizations_Switch", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Database Upgrade Pending.
        /// </summary>
        internal static string DatabaseUpgradeRequired_Header {
            get {
                return ResourceManager.GetString("DatabaseUpgradeRequired_Header", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This page requires an upgraded Swarmops database. .
        /// </summary>
        internal static string DatabaseUpgradeRequired_Info {
            get {
                return ResourceManager.GetString("DatabaseUpgradeRequired_Info", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Awaiting Database Upgrade.
        /// </summary>
        internal static string DatabaseUpgradeRequired_PageTitle {
            get {
                return ResourceManager.GetString("DatabaseUpgradeRequired_PageTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;p&gt;There is a database upgrade pending that prevents your access to this page. &lt;strong&gt;This is a temporary condition&lt;/strong&gt; and the Swarmops servers should already be working on performing the necessary upgrade.&lt;/p&gt;
        ///&lt;p&gt;Retry loading the page in a few minutes. If the condition persists for more than fifteen minutes, contact your system administrator.&lt;/p&gt;.
        /// </summary>
        internal static string DatabaseUpgradeRequired_Rant {
            get {
                return ResourceManager.GetString("DatabaseUpgradeRequired_Rant", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The account &lt;strong&gt;was not locked down&lt;/strong&gt;. There is no such account, or the ticket was invalid or too old. If you still want to lock down this account, contact the administrators of this Swarmops installation..
        /// </summary>
        internal static string LockdownAccount_Failed {
            get {
                return ResourceManager.GetString("LockdownAccount_Failed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to If you&apos;re seeing this page, you&apos;re trying to lock down a user account for security reasons. If the link was valid and hasn&apos;t expired, the account is already locked down..
        /// </summary>
        internal static string LockdownAccount_Info {
            get {
                return ResourceManager.GetString("LockdownAccount_Info", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to User Account Lockdown.
        /// </summary>
        internal static string LockdownAccount_PageTitle {
            get {
                return ResourceManager.GetString("LockdownAccount_PageTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The account has been &lt;strong&gt;successfully locked down&lt;/strong&gt; and any active sessions are now terminated..
        /// </summary>
        internal static string LockdownAccount_Success {
            get {
                return ResourceManager.GetString("LockdownAccount_Success", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Lockdown Ticket.
        /// </summary>
        internal static string LockdownAccount_Ticket {
            get {
                return ResourceManager.GetString("LockdownAccount_Ticket", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;strong&gt;OPEN LEDGERS&lt;/strong&gt;&lt;br/&gt;&lt;br/&gt;{0} is running Open Ledgers. This means that anybody may inspect the financial reports and bookkeeping. You have been logged on to the organization&apos;s Operations to do just that..
        /// </summary>
        internal static string Login_AsOpenLedgers {
            get {
                return ResourceManager.GetString("Login_AsOpenLedgers", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;strong&gt;BitId two-factor authentication is now enabled.&lt;/strong&gt;&lt;br/&gt;&lt;br/&gt;From now on, you can use your bitcoin wallet to log in (and will &lt;strong&gt;need&lt;/strong&gt; to do so). Thank you for enabling added security.&lt;br/&gt;&lt;br/&gt;Please remember to keep your phone secure with a password, too..
        /// </summary>
        internal static string Login_BitIdEnabled {
            get {
                return ResourceManager.GetString("Login_BitIdEnabled", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;strong&gt;BITID LOGIN REQUIRED&lt;/strong&gt;&lt;br/&gt;&lt;br/&gt;This login requires BitId authentication, as enabled by the account holder. You cannot log on with a username/password combination..
        /// </summary>
        internal static string Login_BitIdRequired_Dialog {
            get {
                return ResourceManager.GetString("Login_BitIdRequired_Dialog", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 2FA.
        /// </summary>
        internal static string Login_GoogleAuthenticatorCode {
            get {
                return ResourceManager.GetString("Login_GoogleAuthenticatorCode", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Enter your code from Google Authenticator..
        /// </summary>
        internal static string Login_GoogleAuthenticatorCodeHelp {
            get {
                return ResourceManager.GetString("Login_GoogleAuthenticatorCodeHelp", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Login with Bitcoin Signature (BitId).
        /// </summary>
        internal static string Login_Header {
            get {
                return ResourceManager.GetString("Login_Header", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Help.
        /// </summary>
        internal static string Login_Help {
            get {
                return ResourceManager.GetString("Login_Help", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Login to Swarmops using your bitid-enabled bitcoin wallet. You do not need any funds in it. If you don&apos;t have a bitid-enabled bitcoin wallet, you can log in manually using a legacy username and password. (Your username is your name or email.).
        /// </summary>
        internal static string Login_Info {
            get {
                return ResourceManager.GetString("Login_Info", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Logging in, please wait....
        /// </summary>
        internal static string Login_LoggingIn {
            get {
                return ResourceManager.GetString("Login_LoggingIn", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Manual Password Login.
        /// </summary>
        internal static string Login_ManualLoginHeader {
            get {
                return ResourceManager.GetString("Login_ManualLoginHeader", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Login.
        /// </summary>
        internal static string Login_PageTitle {
            get {
                return ResourceManager.GetString("Login_PageTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Pass.
        /// </summary>
        internal static string Login_Password {
            get {
                return ResourceManager.GetString("Login_Password", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Help, I forgot my password!.
        /// </summary>
        internal static string Login_ResetPassword {
            get {
                return ResourceManager.GetString("Login_ResetPassword", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Sign me up for {0}!.
        /// </summary>
        internal static string Login_SelfSignup {
            get {
                return ResourceManager.GetString("Login_SelfSignup", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Seeking to Join?.
        /// </summary>
        internal static string Login_SelfSignupHeader {
            get {
                return ResourceManager.GetString("Login_SelfSignupHeader", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Swarmops doesn&apos;t recognize this address. Please log in manually and activate your address before using it to login..
        /// </summary>
        internal static string Login_UnknownBitIdAddress {
            get {
                return ResourceManager.GetString("Login_UnknownBitIdAddress", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Use BitID two-factor authentication?.
        /// </summary>
        internal static string Login_UseBitIdLogin {
            get {
                return ResourceManager.GetString("Login_UseBitIdLogin", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Use manual password login?.
        /// </summary>
        internal static string Login_UseManualLogin {
            get {
                return ResourceManager.GetString("Login_UseManualLogin", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to User.
        /// </summary>
        internal static string Login_Username {
            get {
                return ResourceManager.GetString("Login_Username", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to There is no valid password reset ticket for this mail address, the address does not exist, the password reset ticket is invalid, or it has expired.&lt;br/&gt;&lt;br/&gt;&lt;strong&gt;Password reset failed.&lt;/strong&gt;.
        /// </summary>
        internal static string ResetPassword_Failed {
            get {
                return ResourceManager.GetString("ResetPassword_Failed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to You&apos;re resetting the password for a user account. Enter your new password twice to set it and login. (If you&apos;re trying to start the password-reset procedure, you need a ticket to do so. &lt;a href=&quot;/Security/RequestPasswordReset&quot;&gt;Create a ticket.&lt;/a&gt;).
        /// </summary>
        internal static string ResetPassword_Info {
            get {
                return ResourceManager.GetString("ResetPassword_Info", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Enter your mail address to receive instructions how to proceed with the password reset procedure. The instructions will contain a unique and expiring code that you will need to use to proceed..
        /// </summary>
        internal static string ResetPassword_InfoRequest {
            get {
                return ResourceManager.GetString("ResetPassword_InfoRequest", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Enter your mail address.
        /// </summary>
        internal static string ResetPassword_Mail1 {
            get {
                return ResourceManager.GetString("ResetPassword_Mail1", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Re-enter your mail address.
        /// </summary>
        internal static string ResetPassword_Mail2 {
            get {
                return ResourceManager.GetString("ResetPassword_Mail2", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The two passwords do not match. Please retry..
        /// </summary>
        internal static string ResetPassword_NewPasswordsDontMatch {
            get {
                return ResourceManager.GetString("ResetPassword_NewPasswordsDontMatch", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The new password cannot be empty..
        /// </summary>
        internal static string ResetPassword_NoEmpty {
            get {
                return ResourceManager.GetString("ResetPassword_NoEmpty", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Reset Password.
        /// </summary>
        internal static string ResetPassword_PageTitle {
            get {
                return ResourceManager.GetString("ResetPassword_PageTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to New Password.
        /// </summary>
        internal static string ResetPassword_Password {
            get {
                return ResourceManager.GetString("ResetPassword_Password", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Repeat New Password.
        /// </summary>
        internal static string ResetPassword_PasswordRepeat {
            get {
                return ResourceManager.GetString("ResetPassword_PasswordRepeat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to You may reset the password associated with an mail address. A link with a ticket code will be sent to that mail address, which you must follow to reset your password..
        /// </summary>
        internal static string ResetPassword_RequestInfo {
            get {
                return ResourceManager.GetString("ResetPassword_RequestInfo", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Reset.
        /// </summary>
        internal static string ResetPassword_Reset {
            get {
                return ResourceManager.GetString("ResetPassword_Reset", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Your password was changed and you have been logged on to Swarmops using your new password..
        /// </summary>
        internal static string ResetPassword_Success {
            get {
                return ResourceManager.GetString("ResetPassword_Success", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Ticket.
        /// </summary>
        internal static string ResetPassword_Ticket {
            get {
                return ResourceManager.GetString("ResetPassword_Ticket", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to If the mail address was associated with an account on this Swarmops installation, a &lt;strong&gt;reset ticket has been sent.&lt;/strong&gt; Open that mail and click the link in the mail to reset your password. (If the email did not exist, nothing was sent.)&lt;br/&gt;&lt;br/&gt;You may close this browser tab. It is not needed anymore..
        /// </summary>
        internal static string ResetPassword_TicketSentMaybe {
            get {
                return ResourceManager.GetString("ResetPassword_TicketSentMaybe", resourceCulture);
            }
        }
    }
}
