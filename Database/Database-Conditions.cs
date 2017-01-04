using System;
using System.Collections.Generic;
using System.Linq;
using Swarmops.Common.Attributes;
using Swarmops.Common.Enums;
using Swarmops.Common.Interfaces;

namespace Swarmops.Database
{
    public partial class SwarmDb
    {
        /// <summary>
        ///     Constructs a WHERE clause for a table.
        /// </summary>
        /// <param name="tableName">The table name, exactly as in the database.</param>
        /// <param name="conditionParameters">A sequence of IHasIdentity and/or DatabaseCondition objects.</param>
        /// <returns>A WHERE clause that ANDs all conditions.</returns>
        //
        //  Example: ConstructWhereClause (Organization.FromIdentity (1), Database.OpenTrue) makes
        //           a WHERE clause that matches OrganizationId=1 and Open=1.
        // 
        private static string ConstructWhereClause (string tableName, params object[] conditionParameters)
        {
            List<string> conditionList = ConstructWhereClauseRecurse (tableName, conditionParameters);

            string conditions = JoinWhereClauses (conditionList.ToArray());

            if (conditions.Trim().Length > 5)
            {
                return " WHERE " + conditions;
            }

            return string.Empty;
        }


        /// <summary>
        ///     Internal. Recurses for ConstructWhereClause.
        /// </summary>
        private static List<string> ConstructWhereClauseRecurse (string tableName, params object[] conditionParameters)
        {
            List<string> result = new List<string>();

            foreach (object condition in conditionParameters)
            {
                if (condition is IHasIdentities && condition is IHasSingularPluralTypes)
                {
                    result.Add (GetWhereClauseForObjectList (condition as IHasIdentities));
                }
                if (condition is IHasIdentity)
                {
                    result.Add (GetWhereClauseForObject (condition as IHasIdentity));
                }
                else if (condition is DatabaseCondition)
                {
                    if ((DatabaseCondition) condition != DatabaseCondition.None)
                    {
                        result.Add (tableName + "." + GetWhereClauseForCondition ((DatabaseCondition) condition));
                    }
                }
                else if (condition is string)
                {
                    result.Add (condition as string);
                }
                else if (condition is object[]) // params object inherited and added to -- recurse
                {
                    // We're being passed an array as one of the arguments. This is a bit odd but we can handle that too.

                    foreach (object partialCondition in (object[]) condition)
                    {
                        result = result.Concat (ConstructWhereClauseRecurse (tableName, partialCondition)).ToList();
                    }
                }
                else if (!(condition is RecordLimit)) // iterate through attributes to find final hope of clue
                {
                    foreach (object attribute in condition.GetType().GetCustomAttributes (typeof (DbEnumField), true))
                    {
                        if (attribute is DbEnumField)
                        {
                            result.Add (tableName + "." + ((DbEnumField) attribute).FieldName + "=" + (int) condition);
                        }
                    }
                }
                else if (condition is RecordLimit)
                {
                    result.Add(" LIMIT " + (condition as RecordLimit).Limit);
                }
            }

            return result;
        }


        private static string GetWhereClauseForObject (IHasIdentity foreignObject)
        {
            // This is kind of dangerous and assumes that foreignObject has the same TYPE NAME,
            // including casing, as part of the DATABASE FIELD used for primary key.

            // Also, it shifts the burden of error from the logic layer with hard function 
            // typing ("no such function: ForOrganization") at compile time, to the database layer 
            // ("No such field: OrganizationId") at runtime.

            // As such, it is not a good design choice for robustness, but a good design choice
            // for quick developability and expandability, as it reduces plumbing considerably.

            // Example: for an object of type Person with Identity 4711, this returns " PersonId=4711 ".

            if (foreignObject == null)
            {
                return string.Empty;
            }

            return " " + GetForeignTypeString (foreignObject) + "Id=" + foreignObject.Identity + " ";
        }


        private static string GetWhereClauseForObjectList (IHasIdentities foreignObjects)
        {
            // As above, but with a List<IHasIdentity> object.

            // Example: for an object of type People with Identities { 1337, 4711 }, this returns " PersonId IN (1337,4711) ".

            if (foreignObjects == null)
            {
                return string.Empty;
            }

            int[] identities = foreignObjects.Identities;

            if (identities.Length == 0)
            {
                // No ids to match. Make sure that no record is returned by asking for identity 0, which doesn't exist

                identities = new int[1] {0};
            }

            if (!(foreignObjects is IHasSingularPluralTypes))
            {
                throw new ArgumentException("IHasIdentities object passed with missing IHasSingularPluralTypes interface");
            }

            return " " + GetForeignTypeString (((IHasSingularPluralTypes) foreignObjects).SingularType) + "Id IN (" +
                   JoinIds (identities) + ") ";
        }


        private static string GetForeignTypeString (IHasIdentity foreignObject)
        {
            return GetForeignTypeString (foreignObject.GetType());
        }


        private static string GetForeignTypeString (Type foreignObjectType)
        {
            string typeString = string.Empty;
            foreach (object attribute in foreignObjectType.GetCustomAttributes (typeof (DbRecordType), true))
            {
                if (attribute is DbRecordType)
                {
                    typeString = ((DbRecordType) attribute).TypeName;
                }
            }
            if (string.IsNullOrEmpty(typeString))
            {
                typeString = foreignObjectType.Name;
            }

            return typeString;
        }


        private static string GetWhereClauseForCondition (DatabaseCondition condition)
        {
            switch (condition)
            {
                case DatabaseCondition.OpenTrue:
                    return "Open=1";
                case DatabaseCondition.OpenFalse:
                    return "Open=0";
                case DatabaseCondition.AttestedFalse:
                    return "Attested=0";
                case DatabaseCondition.AttestedTrue:
                    return "Attested=1";
                case DatabaseCondition.ActiveFalse:
                    return "Active=0";
                case DatabaseCondition.ActiveTrue:
                    return "Active=1";
                default:
                    throw new InvalidOperationException (
                        "Undefined or unimplemented DatabaseCondition in GetWhereClauseForCondition: " +
                        condition);
            }
        }


        private static string JoinWhereClauses (params string[] clauses)
        {
            string result = string.Empty;

            foreach (string clause in clauses)
            {
                if (!String.IsNullOrEmpty (clause))
                {
                    if (!clause.StartsWith("LIMIT"))
                    {
                        result += "AND " + clause;
                    }
                    else
                    {
                        result += clause;
                    }
                }
            }

            if (result.Length > 0)
            {
                return result.Substring (3) + " "; // strips first AND; ensures space on both sides in returned string
            }

            return string.Empty;
        }
    }


    public enum DatabaseCondition
    {
        /// <summary>
        ///     Undefined
        /// </summary>
        Unknown,

        /// <summary>
        ///     Retrieve only records where Open is true.
        /// </summary>
        OpenTrue,

        /// <summary>
        ///     Retrieve only records where Open is false (closed records).
        /// </summary>
        OpenFalse,

        /// <summary>
        ///     Retrieve only records where Attested is true.
        /// </summary>
        AttestedTrue,

        /// <summary>
        ///     Retreieve only records where Attested is false.
        /// </summary>
        AttestedFalse,

        /// <summary>
        ///     Retrieve only records where activists, memberships etc. are active.
        /// </summary>
        ActiveTrue,

        /// <summary>
        ///     Retrieve only records where activsts, memberships are closed/deactivated.
        /// </summary>
        ActiveFalse,

        /// <summary>
        ///     No condition
        /// </summary>
        None
    }
}