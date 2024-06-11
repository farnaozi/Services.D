using Services.D.Core.Dtos;
using Services.D.Core.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.D.Core.Helpers
{
    public class ServiceExceptions : Dictionary<string, string>
    {
        #region *** private methods

        private void InitDictionary()
        {
            Add(DEFAULT_ERROR, "Unknow error!");
        }

        #endregion
        #region *** ctor

        public ServiceExceptions()
        {
            InitDictionary();
        }

        #endregion
        #region *** public

        public async Task<ServiceResponseBase> HandleException(Exception err, ILoggerRepo loggerRepo)
        {
            var retValue = new ServiceResponseBase();

            if (ContainsKey(err.Message))
            {
                retValue.ExceptionCode = err.Message;
                retValue.ExceptionMessage = this[err.Message];

                await loggerRepo.LogError($"Error: {retValue.ExceptionMessage}");
            }
            else
            {
                retValue.ExceptionCode = DEFAULT_ERROR;
                retValue.ExceptionMessage = this[DEFAULT_ERROR];

                var errorSerialized = JsonConvert.SerializeObject(err);

                Console.WriteLine(errorSerialized);

                await loggerRepo.LogError($"Error: {errorSerialized}");
            }

            return retValue;
        }

        #endregion
        #region *** public constants

        public const string DEFAULT_ERROR = "DEFAULT_ERROR";

        #endregion
    }
}
