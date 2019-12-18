using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DOCVIDEO.PayPalWrapper
{
    public class BasicInformationEventArgs : EventArgs
    {
        private BasicInformation _BasicInformation;


        public BasicInformationEventArgs(BasicInformation e)
        {
            this._BasicInformation = e;
        }

        public override string ToString()
        {
            return this._BasicInformation.ToString();
        }

        public BasicInformation BasicInformation
        {
            get
            {
                return this._BasicInformation;
            }
        }
    }
}
