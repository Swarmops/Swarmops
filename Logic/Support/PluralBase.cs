using System;
using System.Collections.Generic;
using Swarmops.Common.Interfaces;

namespace Swarmops.Logic.Support
{
    public class PluralBase<TPlural, TSingular, TBasic> : List<TSingular>, IHasIdentities, IHasSingularPluralTypes
        where TPlural : PluralBase<TPlural, TSingular, TBasic>, new()
        where TBasic : IHasIdentity
        where TSingular : IHasIdentity
    {
        public int[] Identities
        {
            get
            {
                List<int> result = new List<int>();

                foreach (TSingular singular in this)
                {
                    result.Add (singular.Identity);
                }

                return result.ToArray();
            }
        }

        public static TPlural FromArray (TBasic[] basicArray)
        {
            TPlural result = new TPlural {Capacity = basicArray.Length*11/10};

            foreach (TBasic basic in basicArray)
            {
                result.Add ((TSingular) SingularFactory.FromBasic (basic));
            }

            return result;
        }

        public static TPlural FromArray (TSingular[] logicObjectArray)
        {
            TPlural result = new TPlural {Capacity = logicObjectArray.Length*11/10};

            foreach (TSingular logicObject in logicObjectArray)
            {
                result.Add (logicObject);
            }

            return result;
        }


        public static TPlural FromSingle (TSingular singular)
        {
            TPlural result = new TPlural();
            result.Add (singular);
            return result;
        }


        public static TPlural FromSingle (TBasic basic)
        {
            TPlural result = new TPlural();

            result.Add ((TSingular) SingularFactory.FromBasic (basic));

            return result;
        }


        public static TPlural LogicalOr (TPlural set1, TPlural set2)
        {
            // If either set is invalid, return the other
            // (a null is different from an empty set)

            if (set1 == null)
            {
                return set2;
            }

            if (set2 == null)
            {
                return set1;
            }

            // Build table, eliminating duplicates

            Dictionary<int, TSingular> table = new Dictionary<int, TSingular>();

            foreach (TSingular singular in set1)
            {
                table[singular.Identity] = singular;
            }

            foreach (TSingular singular in set2)
            {
                table[singular.Identity] = singular;
            }

            // Assemble result

            TPlural result = new TPlural();

            foreach (TSingular singular in table.Values)
            {
                result.Add (singular);
            }

            return result;
        }


        public static TPlural LogicalAnd (TPlural set1, TPlural set2)
        {
            // If either set is invalid, return the other
            // (a null is different from an empty set)

            if (set1 == null)
            {
                return set2;
            }

            if (set2 == null)
            {
                return set1;
            }

            Dictionary<int, bool> set2Lookup = new Dictionary<int, bool>();

            // Build set2's lookup table

            foreach (TSingular singular in set2)
            {
                set2Lookup[singular.Identity] = true;
            }

            // Build result

            TPlural result = new TPlural();
            foreach (TSingular singular in set1)
            {
                if (set2Lookup.ContainsKey (singular.Identity))
                {
                    result.Add (singular);
                }
            }

            return result;
        }

        public new void Remove (TSingular objectToRemove)
        {
            for (int index = 0; index < Count; index++)
            {
                if (this[index].Identity == objectToRemove.Identity)
                {
                    RemoveAt (index);
                    index--;
                }
            }
        }

        public Type SingularType { get { return typeof(TSingular); } }
        public Type PluralType { get { return typeof(TPlural); } }
    }
}