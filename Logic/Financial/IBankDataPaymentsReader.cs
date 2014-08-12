namespace Swarmops.Logic.Financial
{
    public interface IBankDataPaymentsReader
    {
        void GetParameterList(); // TODO - need to find types of params and return for this guy
        void SetParameters(); // TODO - ditto
        string GetDescriptionRegex();
        string GenerateUniquePaymentGroupId(); // parameters here?
        void ImportPaymentsFile(); // Input parameter(s)?
    }
}
