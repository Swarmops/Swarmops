using System.IO;
using System.Text;

using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Remote;

// this namespace could be altered and several classes could be put into the same if you'd want to combine several plugins in one dll
namespace ccnet.SharedLabeller.CruiseControl.plugin
{
    [ReflectorType("sharedLabeller")]
    public class SharedLabeller : ILabeller
    {
        /// <summary>
        /// The path where the file that holds the shared label should be located
        /// </summary>
        /// <default>none</default>
        [ReflectorProperty("sharedLabelFilePath", Required = true)]
        public string SharedLabelFilePath { get; set; }

        /// <summary>
        /// Any string to be put in front of all labels. Mutually exclusive with PrefixFile.
        /// </summary>
        [ReflectorProperty("prefix", Required = false)]
        public string Prefix { get; set; }

        /// <summary>
        /// File with string to be put in front of all labels. Mutually exclusive with Prefix.
        /// </summary>
        [ReflectorProperty("prefixFile", Required = false)]
        public string PrefixFile { get; set; }

        /// <summary>
        /// If true, the label will be incremented even if the build fails. Otherwise it will only be incremented if the build succeeds. 
        /// </summary>
        [ReflectorProperty("incrementOnFailure", Required = false)]
        public bool IncrementOnFailure { get; set; }

        /// <summary>
        /// If false, the label will never be incremented when this project is builded. This is usefull for deployment builds that 
        /// should use the last successfull of two or more builds
        /// </summary>
        [ReflectorProperty("increment", Required = false)]
        public bool Increment { get; set; }

        /// <summary>
        /// Allows you to set the initial build number.
        /// This will only be used when on the first build of a project, meaning that when you change this value,
        /// you'll have to stop the CCNet service and delete the state file.
        /// </summary>
        /// <default>0</default>
        [ReflectorProperty("initialBuildLabel", Required = false)]
        public int InitialBuildLabel { get; set; }

        public SharedLabeller()
        {
            IncrementOnFailure = false;
            Increment = true;
            InitialBuildLabel = 768;
        }

        #region ILabeller Members

        public string Generate(IIntegrationResult integrationResult)
        {
            ThoughtWorks.CruiseControl.Core.Util.Log.Debug("About to read label prefix file. Filename: {0}", PrefixFile);
            string prefix = System.IO.File.ReadAllText(PrefixFile).Trim();

            if (ShouldIncrementLabel(integrationResult.LastIntegration))
            {
                return prefix + "+" + this.GetLabel();
            }
            else
            {
                return integrationResult.LastIntegration.Label;
            }
        }

        public void Run(IIntegrationResult integrationResult)
        {
            integrationResult.Label = Generate(integrationResult);
        }

        #endregion

        /// <summary>
        /// Get and increments the label, unless increment is false then it only gets the label
        /// </summary>
        /// <returns></returns>
        private string GetLabel()
        {
            ThoughtWorks.CruiseControl.Core.Util.Log.Debug("About to read build number file. Filename: {0}", SharedLabelFilePath);
            using (FileStream fileStream = File.Open(this.SharedLabelFilePath,
                     FileMode.OpenOrCreate,
                     FileAccess.ReadWrite,
                     FileShare.None))
            {
                // read last build number from file
                var bytes = new byte[fileStream.Length];
                fileStream.Read(bytes, 0, bytes.Length);

                string rawBuildNumber = Encoding.UTF8.GetString(bytes);

                // parse last build number
                int previousBuildNumber;
                if (!int.TryParse(rawBuildNumber, out previousBuildNumber))
                {
                    previousBuildNumber = InitialBuildLabel - 1;
                }

                if (!Increment)
                {
                    return previousBuildNumber.ToString();
                }

                int newBuildNumber = previousBuildNumber + 1;

                // increment build number and write back to file
                bytes = Encoding.UTF8.GetBytes(newBuildNumber.ToString());

                fileStream.Seek(0, SeekOrigin.Begin);
                fileStream.Write(bytes, 0, bytes.Length);

                return newBuildNumber.ToString();
            }
        }

        private bool ShouldIncrementLabel(IntegrationSummary integrationSummary)
        {
            return integrationSummary == null || integrationSummary.Status == IntegrationStatus.Success || IncrementOnFailure;
        }
    }
}