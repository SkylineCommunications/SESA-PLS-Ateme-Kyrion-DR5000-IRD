namespace SES
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Newtonsoft.Json;
    using SESSA_SRM_PLS_Ateme_Kyrion_DR5000.IRD_1.SES;
    using Skyline.DataMiner.Automation;
    using Skyline.DataMiner.Library;
    using Skyline.DataMiner.Library.Solutions.SRM.LifecycleServiceOrchestration;
    using Skyline.DataMiner.Net.Exceptions;
    using Skyline.DataMiner.Net.Messages;

    public class KyrionDR5000Pls
    {
        private const string ScriptName = "SESSA_SRM_PLS_Ateme_Kyrion_DR5000_IRD";
        private readonly Engine engine;
        private readonly ProfileParameterEntryHelper helper;
        private readonly Dictionary<string, SrmParameterConfiguration> srmParameters;

        public KyrionDR5000Pls(Engine engine)
        {
            this.HasError = false;
            this.engine = engine;

            var configurationInfo = LoadResourceConfigurationInfo(engine);

            var nodeProfileConfiguration = LoadNodeProfileConfiguration(engine);

            this.helper = new ProfileParameterEntryHelper(engine, configurationInfo?.OrchestrationLogger);

            this.srmParameters = helper
            .GetNodeSrmParametersConfiguration(configurationInfo, nodeProfileConfiguration)
            .ToDictionary(x => x.ProfileParameterName);

            helper.Log("Script " + ScriptName + " started.", Skyline.DataMiner.Library.Solutions.SRM.Logging.Orchestration.LogEntryType.Info);
        }

        public bool HasError { get; private set; }

        public void SetServiceId()
        {
        }

        public void SetInputType()
        {
            const string profileParameterName = "Input Type";

            try
            {
                var srmParameter = GetProfileParameter(profileParameterName);
                var value = srmParameter.Value.StringValue;
                int setValue;
                switch (value)
                {
                    case "ASI":
                        setValue = 2;
                        break;

                    case "DVB-S2":
                        setValue = 3;
                        break;

                    default:
                        throw new DataMinerException($"Unsupported value '{value}' for profile parameter '{profileParameterName}'.");
                }

                var element = srmParameter.ResourceElement;
                SetParameter(element, ParameterDR5000.InputTypeWrite, new ParameterValue(setValue));
                ValidateParameter(element, profileParameterName, ParameterDR5000.InputTypeRead, setValue);
            }
            catch (Exception e)
            {
                HasError = true;
                this.helper.LogFailure($"Exception while setting '{profileParameterName}':\n{e}");
            }
        }

        public void SetServiceName()
        {
        }

        public void SetBissMode()
        {
        }

        public void SetBissKey()
        {
        }

        private static SrmResourceConfigurationInfo LoadResourceConfigurationInfo(IEngine engine)
        {
            var infoPlaceHolder = engine.GetScriptParam("Info");
            if (infoPlaceHolder == null)
            {
                throw new ArgumentException("There is no input parameter named Info");
            }

            try
            {
                var resourceConfiguration = JsonConvert.DeserializeObject<SrmResourceConfigurationInfo>(infoPlaceHolder.Value);
                if (resourceConfiguration == null)
                {
                    throw new ArgumentException(
                        string.Format(
                            "Could not effectively deserialize the 'Info' parameter {0}.",
                            infoPlaceHolder.Value));
                }

                return resourceConfiguration;
            }
            catch (Exception)
            {
                // Whenever an invalid or empty JSON is passed, we should support the basic flow and retrieve parameters straight from the profile instance.
                return new SrmResourceConfigurationInfo();
            }
        }

        private static NodeProfileConfiguration LoadNodeProfileConfiguration(IEngine engine)
        {
            var instancePlaceHolder = engine.GetScriptParam("ProfileInstance");
            if (instancePlaceHolder == null)
            {
                throw new ArgumentException("There is no input parameter named Info");
            }

            try
            {
                var data = JsonConvert.DeserializeObject<Dictionary<string, Guid>>(instancePlaceHolder.Value);

                return new NodeProfileConfiguration(data);
            }
            catch (Exception ex)
            {
                throw new ArgumentException(string.Format("Invalid input parameter 'ProfileInstance': \r\n{0}", ex));
            }
        }

        private SrmParameterConfiguration GetProfileParameter(string parameterName)
        {
            if (srmParameters.TryGetValue(parameterName, out SrmParameterConfiguration srmParameter))
            {
                return srmParameter;
            }
            else
            {
                throw new DataMinerException($"Profile parameter '{parameterName}' not found.");
            }
        }

        private void SetParameter(Element element, int pid, ParameterValue value)
        {
            var message = new SetParameterMessage
            {
                DataMinerID = element.DmaId,
                ElId = element.ElementId,
                ParameterId = pid,
                Value = value,
            };

            engine.SendSLNetMessage(message);
        }

        private void ValidateParameter<T>(Element element, string parameterName, int pid, T expectedValue)
        {
            T currentValue = default(T);
            Func<bool> verification = () =>
            {
                currentValue = (T)Convert.ChangeType(element.GetParameter(pid), typeof(T));
                return expectedValue.Equals(currentValue);
            };

            var success = verification.RetryUntilSuccessOrTimeout(5000);
            if (!success)
            {
                helper.LogFailureResult(element.ElementName, parameterName, expectedValue.ToString(), Convert.ToString(currentValue));
                throw new DataMinerException($"Failed to set parameter '{parameterName}' (PID {pid}) to '{expectedValue}'.");
            }
            else
            {
                helper.LogSuccessResult(element.ElementName, parameterName, expectedValue.ToString());
            }
        }
    }
}