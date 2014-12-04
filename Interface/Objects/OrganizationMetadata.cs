using System;
using System.Drawing;
using System.Web;

namespace Swarmops.Interface.Objects
{
    public class OrganizationMetadata
    {
        private readonly Color color;
        private readonly int organizationId;
        private readonly bool recursive;

        public OrganizationMetadata(int organizationId, bool recursive, Color color)
        {
            this.organizationId = organizationId;
            this.recursive = recursive;
            this.color = color;
        }


        public int OrganizationId
        {
            get { return this.organizationId; }
        }

        public bool Recursive
        {
            get { return this.recursive; }
        }

        public Color Color
        {
            get { return this.color; }
        }

        /// <summary>
        ///     Determines the organization accessed from the URL, like *.piratpartiet.se => orgid 1.
        /// </summary>
        /// <param name="url">The url of the webpage in question, like data.piratpartiet.se/something.</param>
        /// <returns>A new OrganizationMetadata instance.</returns>
        public static OrganizationMetadata FromUrl(string url)
        {
            // We could do this using regexps, but it would be messy and error-prone. So instead, I'm going to do it
            // the old fashioned readable way.

            // make lowercase
            url = url.ToLower();

            // cut off protocol part

            if (url.StartsWith("http://"))
            {
                url = url.Substring(7);
            }
            else if (url.StartsWith("https://"))
            {
                url = url.Substring(8);
            }

            // cut off the path part

            int indexOfFirstSlash = url.IndexOf('/');

            if (indexOfFirstSlash > -1)
            {
                url = url.Substring(0, indexOfFirstSlash);
            }

            // At this point, we have a string that looks like "data.piratpartiet.se" or "data.ungpirat.se". We look at
            // the end part to determine the organization.

            // TODO: fetch from database.

            if (url.EndsWith("ungpirat.se"))
            {
                return new OrganizationMetadata(2, true, Color.FromArgb(0xFF, 0x99, 0));
            }

            if (url.EndsWith("piratpartiet.se"))
            {
                return new OrganizationMetadata(1, false, Color.FromArgb(0x66, 0, 0x87));
            }

            if (url.EndsWith("localhost"))
            {
                return new OrganizationMetadata(1, false, Color.FromArgb(0x66, 0, 0x87)); // debug purposes
            }

            //HACK:
            // Added: use "linked in" under https://pirateweb.net/Pages/Public/data.piratpartiet.se/
            // wich is a virtual directory pointing into the data-site
            if (HttpContext.Current != null)
            {
                string fullUrl = HttpContext.Current.Request.Path.ToLower();
                if (fullUrl.StartsWith("/pages/public/data.piratpartiet.se"))
                {
                    return new OrganizationMetadata(1, false, Color.FromArgb(0x66, 0, 0x87));
                }
            }


            throw new ArgumentException("Unsupported organization");
        }
    }
}