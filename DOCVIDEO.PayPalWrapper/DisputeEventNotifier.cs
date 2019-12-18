using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Collections;
using Microsoft.VisualBasic.CompilerServices;

namespace DOCVIDEO.PayPalWrapper
{
    [Serializable, StructLayout(LayoutKind.Sequential)]
    public struct DisputeEventNotifier
    {
        private PayPalIPN _IPN;
        public PayPalIPN IPN
        {
            get
            {
                return this._IPN;
            }
            set
            {
                this._IPN = value;
            }
        }
        public DisputeEventNotifier(ref PayPalIPN PayPalIPN)
        {
            this = new DisputeEventNotifier();
            this.IPN = PayPalIPN;
        }

        public Dispute_Notification_Variables txn_type
        {
            get
            {
                bool flag = false;
                IEnumerator enumerator = null;
                try
                {
                    enumerator = this.IPN.AllInformation.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        if (Conversions.ToString(enumerator.Current) == "txn_type")
                        {
                            flag = true;
                            goto returnNotifications;
                        }
                    }
                }
                finally
                {
                    if (enumerator is IDisposable)
                    {
                        (enumerator as IDisposable).Dispose();
                    }
                }
            returnNotifications:
                if (!flag)
                {
                    return Dispute_Notification_Variables.none;
                }
                Type enumType = typeof(Dispute_Notification_Variables);
                return (Dispute_Notification_Variables)Conversions.ToInteger(Enum.Parse(enumType, this.IPN.AllInformation["txn_type"]));
            }
        }

        public enum Dispute_Notification_Variables
        {
            none,
            new_case,
            adjustment
        }
    }
}
