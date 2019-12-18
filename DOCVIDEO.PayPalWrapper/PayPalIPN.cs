using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Web;
using System.Collections.Specialized;

namespace DOCVIDEO.PayPalWrapper
{
    
    [Serializable]
    public class PayPalIPN
    {
        private NameValueCollection _AllInformation;
        private BasicInformation _BasicInformation;
        private DisputeEventNotifier _Dipute_Notification;
        private PaymentVerification_Flags _PaymentVerification;
        private PayPalSandbox_Flags _PayPal_or_Sandbox;


        public event PaymentUnverifedEventHandler PaymentUnverifedEvent;
        public event PaymentVerifiedEventHandler PaymentVerifiedEvent;

        public PayPalIPN([Optional, DefaultParameterValue(false)] bool UsingSandbox)
        {
            PayPalIPN payPalIPN = this;
            this._BasicInformation = new BasicInformation(ref payPalIPN);
            PayPalIPN ipn2 = this;
            this._Dipute_Notification = new DisputeEventNotifier(ref ipn2);
            this._PayPal_or_Sandbox = PayPalSandbox_Flags.sandbox;
            this._PaymentVerification = PaymentVerification_Flags.INVALID;
            if (UsingSandbox)
            {
                this.PayPal_or_Sandbox = PayPalSandbox_Flags.sandbox;
            }
            else
            {
                this.PayPal_or_Sandbox = PayPalSandbox_Flags.paypal;
            }
            this._AllInformation = HttpContext.Current.Request.Form;
        }

        public PayPalIPN(NameValueCollection Form, [Optional, DefaultParameterValue(false)] bool UsingSandBox)
        {
            PayPalIPN payPalIPN = this;
            this._BasicInformation = new BasicInformation(ref payPalIPN);
            PayPalIPN ipn2 = this;
            this._Dipute_Notification = new DisputeEventNotifier(ref ipn2);
            this._PayPal_or_Sandbox = PayPalSandbox_Flags.sandbox;
            this._PaymentVerification = PaymentVerification_Flags.INVALID;
            this._AllInformation = Form;
            if (UsingSandBox)
            {
                this.PayPal_or_Sandbox = PayPalSandbox_Flags.sandbox;
            }
            else
            {
                this.PayPal_or_Sandbox = PayPalSandbox_Flags.paypal;
            }
        }

        public void VerifyTransaction()
        {
            string str3;
            if (this.PayPal_or_Sandbox == PayPalSandbox_Flags.paypal)
            {
                str3 = "https://www.paypal.com/cgi-bin/webscr";
            }
            else
            {
                str3 = "https://www.sandbox.paypal.com/cgi-bin/webscr";
            }
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(str3);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            byte[] bytes = HttpContext.Current.Request.BinaryRead(HttpContext.Current.Request.ContentLength);
            string str = Encoding.ASCII.GetString(bytes) + "&cmd=_notify-validate";
            request.ContentLength = str.Length;
            StreamWriter writer = new StreamWriter(request.GetRequestStream(), Encoding.ASCII);
            writer.Write(str);
            writer.Close();
            StreamReader reader = new StreamReader(request.GetResponse().GetResponseStream());
            string str2 = reader.ReadToEnd();
            reader.Close();
            if (str2 == "VERIFIED")
            {
                this.PaymentVerification = PaymentVerification_Flags.VERIFIED;
                BasicInformationEventArgs e = new BasicInformationEventArgs(this.BasicInformation);
                PaymentVerifiedEventHandler paymentVerifiedEvent = this.PaymentVerifiedEvent;
                if (paymentVerifiedEvent != null)
                {
                    paymentVerifiedEvent(this, e);
                }
            }
            else
            {
                PaymentUnverifedEventHandler paymentUnverifedEvent;
                if (str2 == "INVALID")
                {
                    this.PaymentVerification = PaymentVerification_Flags.INVALID;
                    paymentUnverifedEvent = this.PaymentUnverifedEvent;
                    if (paymentUnverifedEvent != null)
                    {
                        paymentUnverifedEvent(this, null);
                    }
                }
                else
                {
                    this.PaymentVerification = PaymentVerification_Flags.OTHER;
                    paymentUnverifedEvent = this.PaymentUnverifedEvent;
                    if (paymentUnverifedEvent != null)
                    {
                        paymentUnverifedEvent(this, null);
                    }
                }
            }
        }

        public NameValueCollection AllInformation
        {
            get
            {
                return this._AllInformation;
            }
        }

        public BasicInformation BasicInformation
        {
            get
            {
                return this._BasicInformation;
            }
        }

        public DisputeEventNotifier Dispute_Notifcation
        {
            get
            {
                return this._Dipute_Notification;
            }
        }

        public PaymentVerification_Flags PaymentVerification
        {
            get
            {
                return this._PaymentVerification;
            }
            set
            {
                this._PaymentVerification = value;
            }
        }

        public PayPalSandbox_Flags PayPal_or_Sandbox
        {
            get
            {
                return this._PayPal_or_Sandbox;
            }
            set
            {
                this._PayPal_or_Sandbox = value;
            }
        }

        public delegate void PaymentUnverifedEventHandler(object sender, EventArgs e);

        public delegate void PaymentVerifiedEventHandler(object sender, BasicInformationEventArgs e);
    }
}
