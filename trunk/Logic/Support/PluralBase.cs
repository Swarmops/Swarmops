

using System.Collections.Generic;
using Swarmops.Basic.Interfaces;

namespace Swarmops.Logic.Support
{
    public class PluralBase<TPlural,TSingular,TBasic>: List<TSingular>, IHasIdentities
        where TPlural: PluralBase<TPlural, TSingular, TBasic>, new()
        where TBasic: IHasIdentity
        where TSingular: IHasIdentity
    {
        static public TPlural FromArray (TBasic[] basicArray)
        {
            var result = new TPlural {Capacity = basicArray.Length*11/10};

            foreach (TBasic basic in basicArray)
            {
                result.Add((TSingular) SingularFactory.FromBasic(basic));
            }

            return result;
        }


        public int[] Identities
        {
            get
            {
                List<int> result = new List<int>();

                foreach (TSingular singular in this)
                {
                    result.Add(singular.Identity);
                }

                return result.ToArray();
            }
        }


        static public TPlural FromSingle (TSingular singular)
        {
            var result = new TPlural();
            result.Add(singular);
            return result;
        }


        static public TPlural FromSingle (TBasic basic)
        {
            var result = new TPlural();

            result.Add((TSingular) SingularFactory.FromBasic(basic));

            return result;
        }



        public static TPlural LogicalOr(TPlural set1, TPlural set2)
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

            var table = new Dictionary<int, TSingular>();

            foreach (TSingular singular in set1)
            {
                table[singular.Identity] = singular;
            }

            foreach (TSingular singular in set2)
            {
                table[singular.Identity] = singular;
            }

            // Assemble result

            var result = new TPlural();

            foreach (TSingular singular in table.Values)
            {
                result.Add(singular);
            }

            return result;
        }



        public static TPlural LogicalAnd(TPlural set1, TPlural set2)
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

            var set2Lookup = new Dictionary<int, bool>();

            // Build set2's lookup table

            foreach (TSingular singular in set2)
            {
                set2Lookup[singular.Identity] = true;
            }

            // Build result

            var result = new TPlural();
            foreach (TSingular singular in set1)
            {
                if (set2Lookup.ContainsKey(singular.Identity))
                {
                    result.Add(singular);
                }
            }

            return result;
        }

        public new void Remove (TSingular objectToRemove)
        {
            for (int index = 0; index < this.Count; index++)
            {
                if (this[index].Identity == objectToRemove.Identity)
                {
                    this.RemoveAt(index);
                    index--;
                }
            }
        }
    }
}